using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using GraceGawker.Windows;
using System.Linq;
using System;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Inventory.InventoryEventArgTypes;
using Dalamud.Game.Inventory;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace GraceGawker;

public unsafe class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPluginLog Logger { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;
    [PluginService] internal static IFlyTextGui FlyText { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IGameInventory Inventory { get; private set; } = null!;

    public readonly WindowSystem WindowSystem = new("GraceGawker");

    public Configuration Config { get; init; }
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    public DataModels.PlayerState playerState { get; set; } = DataModels.PlayerState.Inactive;
    public static bool ShouldBeOpen { get; private set; } = false;
    private bool WaitingForItemDeletion { get; set; } = false;
    private DataModels.Manual? item { get; set; } = null;

    public Plugin()
    {
        Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, ConfigWindow);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler("/pgrace", new CommandInfo(OnCommand)
        {
            HelpMessage = "Opens the configuration menu for Grace Gawker."
        });

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        FlyText.FlyTextCreated += OnFlyText;
        Condition.ConditionChange += OnConditionChange;
        ClientState.Login += OnLogin;
        Framework.Update += OnUpdate;
        Inventory.ItemChanged += OnItemChanged;

        if (ClientState.IsLoggedIn == true)
            InitManual();

        SetUpHooks();
    }

    private void SetUpHooks()
    {
        useActionHook = GameInteropProvider.HookFromAddress<ActionManager.Delegates.UseAction>((nint)ActionManager.MemberFunctionPointers.UseAction, UseActionDetour);
        useActionHook.Enable();
        Logger.Information("Hooked into UseAction successfully.");

        var actorControlSelfPtr = SigScanner.ScanText(ActorControlSig);
        actorControlSelfHook = GameInteropProvider.HookFromAddress<ActorControlSelfDelegate>(actorControlSelfPtr, ActorControlSelf);
        actorControlSelfHook.Enable();
        Logger.Information("Hooked into ActorControlSelf successfully.");
    }

    private Hook<ActionManager.Delegates.UseAction>? useActionHook;

    private bool UseActionDetour(ActionManager* actionManager, ActionType actionType, uint actionID, ulong targetID, uint a4, ActionManager.UseActionMode a5, uint a6, bool* a7)
    {
        try
        {
            Logger.Debug($"Action Used: Type = {actionType}, ID = {actionID}, Target ID = {targetID}");
            if (DataModels.Manuals.Any(item => item.Id == actionID)) {
                item = DataModels.Manuals.FirstOrDefault(item => item.Id == actionID);
                Logger.Info($"Detected the use of {item!.Name}!");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occured when handling a UseAction event.");
        }

        var useActionReturn = useActionHook!.Original(actionManager, actionType, actionID, targetID, a4, a5, a6, a7);
        if (item != null)
        {
            WaitingForItemDeletion = true;
            Logger.Debug("WaitingForItemDeletion set to: True");
        }

        return useActionReturn;
    }

    private void OnItemChanged(GameInventoryEvent type, InventoryEventArgs data)
    {
        if (!WaitingForItemDeletion || item == null || data.Item.ItemId != item.Id)
            return;

        Config.CurrentManual = item;
        Config.RemainingExp = Config.CurrentManual.MaxExp;
        Logger.Info($"{item!.Name} was used successfully!");

        ShouldBeOpen = true;
        item = null;
        WaitingForItemDeletion = false;
    }

    private const string ActorControlSig = "E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64";
    private delegate void ActorControlSelfDelegate(uint category, uint eventId, uint param1, uint param2, uint param3, uint param4, uint param5, uint param6, ulong targetId, byte param7);
    private Hook<ActorControlSelfDelegate>? actorControlSelfHook;

    private void ActorControlSelf(uint category, uint eventId, uint param1, uint param2, uint param3, uint param4, uint param5, uint param6, ulong targetId, byte param7)
    {
        actorControlSelfHook!.Original(category, eventId, param1, param2, param3, param4, param5, param6, targetId, param7);

        if (eventId != 7 || Config.CurrentManual == null)
            return;

        try
        {
            if (param3 < 75)
                return;

            if ((Config.CurrentManual.Type == DataModels.ManualType.Gathering && playerState == DataModels.PlayerState.Gathering) ||
                (Config.CurrentManual.Type == DataModels.ManualType.Crafting && playerState == DataModels.PlayerState.Crafting))
            {
                var expTotal = (float)param2;
                var expMult = (float)param3;

                Config.RemainingExp -= CalculateExpToRemove(expTotal, expMult);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while parsing actor control packet!");
        }
    }

    private int CalculateExpToRemove(float expTotal, float expMult)
    {
        if (Config.CurrentManual == null)
            return 0;

        var manualMult = Config.CurrentManual.ExpBoost / (ClientState.LocalPlayer!.Level >= Config.CurrentManual.PenaltyLevel ? 2 : 1);
        expMult = (expMult / 100f) + 1f;
        var baseExp = expTotal / expMult;
        var expToRemove = (int)(manualMult * baseExp);
        Logger.Debug($"expToRemove: {expToRemove}");

        return expToRemove;
    }

    private void OnLogin()
    {
        InitManual();
    }

    private void InitManual()
    {
        Logger.Debug("Initializing current manual...");

        if (ClientState.LocalPlayer == null)
        {
            Logger.Error("LocalPlayer is null!");
            return;
        }

        foreach (var status in ClientState.LocalPlayer.StatusList)
        {
            if (status.StatusId == 45 || status.StatusId == 46)
            {
                var type = status.StatusId == 45 ? "Crafter" : "Gatherer";
                if (Config.CurrentManual == null)
                {
                    Logger.Warning($"{type}'s Grace buff found, but no current manual is set!");
                }
                else
                {
                    Logger.Info($"Found {type}'s Grace buff, setting current manual to '{Config.CurrentManual.Name}' with {Config.RemainingExp:n0} EXP remaining!");
                    ShouldBeOpen = true;
                }
                return;
            }
        }

        if (Config.CurrentManual != null)
        {
            Logger.Warning("Current manual is set, but no Grace buff found! Clearing current manual...");
            Config.CurrentManual = null;
            Config.RemainingExp = 0;
            return;
        }
        else
        {
            Logger.Debug("No manual set and no grace buff found, ignoring initialization!");
        }
    }

    private void OnUpdate(IFramework framework)
    {
        if ((Config.PluginEnabled && ShouldBeOpen && PassesJobCheck())
            || ConfigWindow.IsOpen)
        {
            MainWindow.IsOpen = true;
        }
        else
        {
            MainWindow.IsOpen = false;
        }
    }

    private bool PassesJobCheck()
    {
        if (ClientState.LocalPlayer == null || Config.CurrentManual == null)
            return false;

        if (Config.HideForWrongJobs)
        {
            if (Config.CurrentManual.Type == DataModels.ManualType.Gathering && DataModels.GathererJobs.Contains(ClientState.LocalPlayer.ClassJob.Id))
                return true;

            if (Config.CurrentManual.Type == DataModels.ManualType.Crafting && DataModels.CrafterJobs.Contains(ClientState.LocalPlayer.ClassJob.Id))
                return true;

            return false;
        }

        return true;
    }

    private void OnFlyText(ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2, ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (kind == FlyTextKind.BuffFading)
        {
            if (text1.TextValue != "- Gatherer's Grace" && text1.TextValue != "- Crafter's Grace")
                return;

            Config.CurrentManual = null;
            Config.RemainingExp = 0;
            Logger.Info("Grace buff has either expired or been removed!");

            ShouldBeOpen = false;
        }
    }

    private void OnConditionChange(ConditionFlag flag, bool value)
    {
        if (flag == ConditionFlag.Crafting)
        {
            if (value == true)
            {
                playerState = DataModels.PlayerState.Crafting;
                Logger.Debug("Player has started crafting!");
            } else
            {
                playerState = DataModels.PlayerState.Inactive;
                Logger.Debug("Player has stopped crafting.");
            }
        }
        else if (flag == ConditionFlag.Gathering)
        {
            if (value == true)
            {
                playerState = DataModels.PlayerState.Gathering;
                Logger.Debug("Player has started gathering!");
            }
            else
            {
                playerState = DataModels.PlayerState.Inactive;
                Logger.Debug("Player has stopped gathering.");
            }
        }
    }

    public void Dispose()
    {
        useActionHook?.Disable();
        useActionHook?.Dispose();
        actorControlSelfHook?.Disable();
        actorControlSelfHook?.Dispose();
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler("/pgrace");
        Config.Save();
    }

    private void OnCommand(string command, string args)
    {
        ToggleConfigUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
