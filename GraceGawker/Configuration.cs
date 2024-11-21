using Dalamud.Configuration;
using System;

namespace GraceGawker;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public DataModels.Manual? CurrentManual { get; set; } = null;

    public int RemainingExp { get; set; } = 0;

    public bool PluginEnabled { get; set; } = true;

    public bool IsMainWindowMovable { get; set; } = false;

    public bool ShowPercent { get; set; } = true;

    public bool ColorPercent { get; set; } = true;

    public bool ShowPercentOnBar { get; set; } = false;

    public bool ShowManualName { get; set; } = true;

    public bool HideForWrongJobs { get; set; } = false;

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
