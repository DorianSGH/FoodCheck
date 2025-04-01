using Dalamud.Plugin.Services;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace FoodCheck
{
    public static class Service
    {
        public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
        public static ICommandManager CommandManager { get; private set; } = null!;
        public static IChatGui ChatGui { get; private set; } = null!;
        public static IClientState ClientState { get; private set; } = null!;
        public static IFramework Framework { get; private set; } = null!;

        public static void Initialize(IDalamudPluginInterface pluginInterface, ICommandManager commandManager, IChatGui chatGui, IClientState clientState, IFramework framework)
        {
            PluginInterface = pluginInterface;
            CommandManager = commandManager;
            ChatGui = chatGui;
            ClientState = clientState;
            Framework = framework;
        }
    }
}
