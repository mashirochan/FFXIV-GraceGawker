using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace GraceGawker.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Configuration config;
    private readonly ConfigWindow configWindow;

    public MainWindow(Plugin plugin, ConfigWindow _configWindow)
        : base("Grace Gawker##gracegawker_main", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoFocusOnAppearing)
    {
        Size = new Vector2(250, 66);
        SizeCondition = ImGuiCond.Once;
        RespectCloseHotkey = false;
        DisableWindowSounds = true;

        config = plugin.Config;
        configWindow = _configWindow;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (!config.UseDalamudBackground)
        {
            var newOpacity = config.BackgroundOpacity / 100f;
            ImGui.SetNextWindowBgAlpha(newOpacity);
        }

        if (config.IsMainWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
            Flags &= ~ImGuiWindowFlags.NoMouseInputs;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
            Flags |= ImGuiWindowFlags.NoMouseInputs;
        }
    }

    public override void Draw()
    {
        if (!Flags.HasFlag(ImGuiWindowFlags.NoMove) && configWindow.IsOpen)
        {
            var windowPos = ImGui.GetWindowPos();
            var windowSize = ImGui.GetWindowSize();

            // Get the foreground draw list to draw outside of the window's boundaries
            var drawList = ImGui.GetForegroundDrawList();

            // Define the color (red) and thickness of the outline
            var outlineColor = ImGui.GetColorU32(new Vector4(255f / 255f, 98f / 255f, 98f / 255f, 0.5f));  // Red color
            var thickness = 1.0f;  // Thickness of the outline

            // Adjust the position and size to account for the outline thickness
            var outlinePos = new Vector2(windowPos.X - thickness * 0.5f, windowPos.Y - thickness * 0.5f);
            var outlineSize = new Vector2(windowPos.X + windowSize.X + thickness * 0.5f, windowPos.Y + windowSize.Y + thickness * 0.5f);

            // Draw the rectangle (outline) around the window using the foreground draw list
            drawList.AddRect(outlinePos, outlineSize, outlineColor, 0.0f, ImDrawFlags.None, thickness);
        }

        if (config.CurrentManual != null && config.CurrentManual.MaxExp != 0)
        {
            if (config.ShowManualName)
            {
                ImGui.Text(config.CurrentManual.Name);
                ImGui.Spacing();
            }

            var progress = (float)config.RemainingExp / config.CurrentManual.MaxExp;
            var progressPercent = $"{(int)Math.Round(progress * 100f)}%";
            var progressText = config.ShowPercentOnBar ? progressPercent : $"{config.RemainingExp:n0} / {config.CurrentManual.MaxExp:n0}";
            ImGui.ProgressBar(progress, new Vector2(196f, 0f), progressText);

            if (config.ShowPercent)
            {
                ImGui.SameLine(250 - ImGui.CalcTextSize(progressPercent).X);
                if (config.ColorPercent && Plugin.ClientState.LocalPlayer != null && Plugin.ClientState.LocalPlayer.Level >= config.CurrentManual.PenaltyLevel)
                {
                    ImGui.TextColored(KnownColor.Yellow.Vector(), progressPercent + '%');
                }
                else
                {
                    ImGui.Text(progressPercent + '%');
                }
            }
        }
    }
}
