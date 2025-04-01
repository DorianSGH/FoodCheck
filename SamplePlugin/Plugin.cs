using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FoodCheck.Windows;
using System.Linq;
using System;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
namespace FoodCheck
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Food Buff Reminder";
        private const string CommandName = "/FoodCheck";
        DateTime lastCheckTime = DateTime.UtcNow;
        DateTime lastMessageSentTime = DateTime.Today;


        public Configuration Config { get; init; }
        public ConfigWindow ConfigWindow { get; init; }

        public Plugin(IDalamudPluginInterface pluginInterface, ICommandManager commandManager, IChatGui chatGui, IClientState clientState, IFramework framework)
        {
            Service.Initialize(pluginInterface, commandManager, chatGui, clientState, framework);
            Config = pluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Save(pluginInterface);

            ConfigWindow = new ConfigWindow(Config);

            Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the Food Buff Reminder settings."
            });

            Service.PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;
            Service.PluginInterface.UiBuilder.Draw += ConfigWindow.Draw;

            Service.Framework.Update += CheckFoodBuff;
        }


        private void OnCommand(string command, string args)
        {
            ConfigWindow.Toggle();
        }

        private void OpenConfigUi()
        {
            ConfigWindow.Toggle();
        }


        public void Dispose()
        {
            ConfigWindow.Dispose();
            Service.CommandManager.RemoveHandler(CommandName);
            Service.PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUi;
        }


        private void CheckFoodBuff(IFramework framework)
        {
            if (Config.FrequencyFormat != "Seconds" && Config.MessageFrequency <= 5)
            {
                if ((DateTime.UtcNow - lastCheckTime).TotalSeconds < 5) return;
                lastCheckTime = DateTime.UtcNow;
            }
            else
            {
                if ((DateTime.UtcNow - lastCheckTime).TotalSeconds < 1) return;
                lastCheckTime = DateTime.UtcNow;
            }

            if (Service.ClientState.LocalPlayer == null) return;

            var buff = Service.ClientState.LocalPlayer.StatusList
                .FirstOrDefault(status => status.StatusId == 48);

            if (buff == null)
            {
                return;
            }

            var timeRemaining = buff.RemainingTime;

            if (timeRemaining <= Config.WarningTime && Config.EnableNotifications == true)
            {
                if ((DateTime.UtcNow - lastMessageSentTime).TotalSeconds >= Config.MessageFrequency)
                {
                    SendNotification(timeRemaining);
                    lastMessageSentTime = DateTime.UtcNow;
                }
            }
        }

        private void SendNotification(float timeRemaining)
        {
            var message = $"Your food buff is running low! Time left: {(int)(timeRemaining / 60)} minutes.";

            Service.ChatGui.Print(new SeString(new TextPayload(message)));

            if (Config.EnableSound)
            {
                PlaySound();
            }
        }

        private void PlaySound()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                string resourceName = $"{assembly.GetName().Name}.sounds.alert.wav";


                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Service.ChatGui.PrintError($"[Food Buff Reminder] Sound resource not found: {resourceName}");

                        var resources = assembly.GetManifestResourceNames();
                        Service.ChatGui.Print($"[Debug] Available resources: {string.Join(", ", resources)}");
                        return;
                    }

                    var memoryStream = new System.IO.MemoryStream();
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try
                        {
                            using (var audioFile = new NAudio.Wave.WaveFileReader(memoryStream))
                            using (var outputDevice = new NAudio.Wave.WaveOutEvent())
                            {
                                outputDevice.Init(audioFile);
                                outputDevice.Play();

                                while (outputDevice.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                                {
                                    System.Threading.Thread.Sleep(100);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Service.ChatGui.PrintError($"[Food Buff Reminder] Error playing sound: {ex.Message}");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Service.ChatGui.PrintError($"[Food Buff Reminder] Error playing sound: {ex.Message}");
            }
        }
    }
}
