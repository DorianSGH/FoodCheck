using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using ImGuiNET;

namespace FoodCheck.Windows
{
    public class ConfigWindow
    {
        private Configuration config;
        private bool isVisible = false;

        public ConfigWindow(Configuration config)
        {
            this.config = config;
        }

        public void Toggle()
        {
            isVisible = !isVisible;
        }

        public void Draw()
        {
            if (!isVisible) return;

            ImGui.Begin("Food Buff Reminder Config", ref isVisible, ImGuiWindowFlags.AlwaysAutoResize);

            ImGui.Text("Configure the food buff reminder settings below.");

            bool notificationsEnabled = config.EnableNotifications;
            if (ImGui.Checkbox("Enable Notifications", ref notificationsEnabled))
            {
                config.EnableNotifications = notificationsEnabled;
                config.Save(Service.PluginInterface);
            }

            int warningTime = config.WarningTime / 60;
            if (ImGui.SliderInt("Warning Time (minutes)", ref warningTime, 1, 30))
            {
                config.WarningTime = warningTime * 60;
                config.Save(Service.PluginInterface);
            }

            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                config.Save(Service.PluginInterface);
            }

            bool enableSound = config.EnableSound;
            if (ImGui.Checkbox("Enable Sound Effect", ref enableSound))
            {
                config.EnableSound = enableSound;
                config.Save(Service.PluginInterface);
            }

            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();
            ImGui.Spacing();

            string[] frequencyOptions = { "Seconds", "Minutes" };
            int selectedFormatIndex = config.FrequencyFormat == "Minutes" ? 1 : 0;

            if (ImGui.Combo("Message Frequency Format", ref selectedFormatIndex, frequencyOptions, frequencyOptions.Length))
            {
                config.FrequencyFormat = frequencyOptions[selectedFormatIndex];
                config.Save(Service.PluginInterface);
            }

            if (config.FrequencyFormat == "Minutes")
            {
                int messageFrequencyMinutes = config.MessageFrequency / 60;
                if (ImGui.SliderInt("Message Frequency (Minutes)", ref messageFrequencyMinutes, 1, 29))
                {
                    config.MessageFrequency = messageFrequencyMinutes * 60;
                    config.Save(Service.PluginInterface);
                }
            }
            else
            {
                int messageFrequencySeconds = config.MessageFrequency;
                if (ImGui.SliderInt("Message Frequency (Seconds)", ref messageFrequencySeconds, 1, 300))
                {
                    config.MessageFrequency = messageFrequencySeconds;
                    config.Save(Service.PluginInterface);
                }
            }

            if (ImGui.Button("Close"))
            {
                isVisible = false;
            }

            ImGui.End();
        }

        public void Dispose()
        {
            isVisible = false;
        }
    }
}
