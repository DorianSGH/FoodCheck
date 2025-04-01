using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace FoodCheck
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 1;

        public bool EnableNotifications { get; set; } = true;
        public int WarningTime { get; set; } = 300;
        public int MessageFrequency { get; set; } = 60;
        public bool EnableSound { get; set; } = true;
        public string? FrequencyFormat { get; set; }

        public void Save(IDalamudPluginInterface pluginInterface)
        {
            pluginInterface.SavePluginConfig(this);
        }


    }
}
