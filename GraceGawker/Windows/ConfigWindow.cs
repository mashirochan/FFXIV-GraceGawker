using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace GraceGawker.Windows;

public class ConfigWindow : Window, IDisposable
{
    private Configuration config;

    public ConfigWindow(Plugin plugin) : base("Grace Gawker Config##gracegawker_config")
    {
        Flags = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(232, 90);
        SizeCondition = ImGuiCond.Once;

        config = plugin.Config;
    }

    public void Dispose() { }

    public override void Draw()
    {
        var pluginEnabled = config.PluginEnabled;
        if (ImGui.Checkbox("Enabled", ref pluginEnabled))
        {
            config.PluginEnabled = pluginEnabled;
            Plugin.Logger.Debug($"PluginEnabled set to: {(pluginEnabled ? "True" : "False")}");
            config.Save();
        }

        ImGui.SameLine();

        var movable = config.IsMainWindowMovable;
        if (ImGui.Checkbox("Movable", ref movable))
        {
            config.IsMainWindowMovable = movable;
            Plugin.Logger.Debug($"IsMainWindowMovabe set to: {(movable ? "True" : "False")}");
            config.Save();
        }

        ImGui.Spacing();
        ImGui.Separator();
        CenteredText("Display Settings");
        ImGui.Separator();
        ImGui.Spacing();

        var hideForWrongJobs = config.HideForWrongJobs;
        if (ImGui.Checkbox("Hide for Wrong Jobs", ref hideForWrongJobs))
        {
            config.HideForWrongJobs = hideForWrongJobs;
            Plugin.Logger.Debug($"HideForWrongJobs set to: {(hideForWrongJobs ? "True" : "False")}");
            config.Save();
        }

        ImGui.Spacing();
        ImGui.Separator();
        CenteredText("Format Settings");
        ImGui.Separator();
        ImGui.Spacing();

        var showManualName = config.ShowManualName;
        if (ImGui.Checkbox("Show Manual Name", ref showManualName))
        {
            config.ShowManualName = showManualName;
            Plugin.Logger.Debug($"ShowManualName set to: {(showManualName ? "True" : "False")}");
            config.Save();
        }

        ImGui.Spacing();

        var showPercent = config.ShowPercent;
        if (ImGui.Checkbox("Show Percent", ref showPercent))
        {
            config.ShowPercent = showPercent;
            Plugin.Logger.Debug($"ShowPercent set to: {(showPercent ? "True" : "False")}");
            config.Save();
        }

        ImGui.Spacing();

        var colorPercent = config.ColorPercent;
        if (ImGui.Checkbox("Color Percent", ref colorPercent))
        {
            config.ColorPercent = colorPercent;
            Plugin.Logger.Debug($"ColorPercent set to: {(colorPercent ? "True" : "False")}");
            config.Save();
        }

        ImGui.Spacing();

        var showPercentOnBar = config.ShowPercentOnBar;
        if (ImGui.Checkbox("Show Percent On Bar", ref showPercentOnBar))
        {
            config.ShowPercentOnBar = showPercentOnBar;
            Plugin.Logger.Debug($"ShowPercentOnBar set to: {(showPercentOnBar ? "True" : "False")}");
            config.Save();
        }
    }

    private static void CenteredText(string text)
    {
        var textWidth = ImGui.CalcTextSize(text).X;
        var windowWidth = ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X;

        var textPosX = (windowWidth - textWidth) / 2.0f;

        if (textPosX > 0.0f)
        {
            ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMin().X + textPosX);
        }
        else
        {
            ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMin().X);
        }

        ImGui.Text(text);
    }
}
