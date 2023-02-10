using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Party;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using ThirdEye.Windows;

namespace ThirdEye;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
public sealed class Plugin : IDalamudPlugin {
    public string Name => "Third Eye";
    private const string CommandName = "/pte";

    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static CommandManager CommandManager { get; private set; } = null!;
    [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
    [PluginService] public static Framework Framework { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static Condition Condition { get; private set; } = null!;
    [PluginService] public static PartyList PartyList { get; private set; } = null!;

    public static RecordingManager RecordingManager;
    public static Configuration Configuration;

    public readonly WindowSystem WindowSystem = new("ThirdEye");
    private ConfigWindow ConfigWindow { get; }

    public Plugin() {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        ConfigWindow = new ConfigWindow();
        WindowSystem.AddWindow(ConfigWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "Opens the config window."
        });

        RecordingManager = new RecordingManager();

        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi += OpenConfigUi;
        Framework.Update += FrameworkUpdate;
    }

    public void Dispose() {
        WindowSystem.RemoveAllWindows();
        ConfigWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);

        RecordingManager.Dispose();

        PluginInterface.UiBuilder.Draw -= DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi -= OpenConfigUi;
        Framework.Update -= FrameworkUpdate;
    }

    private void OnCommand(string command, string args) {
        OpenConfigUi();
    }

    private void DrawUi() {
        WindowSystem.Draw();
    }

    private void OpenConfigUi() {
        ConfigWindow.IsOpen = true;
    }

    private void FrameworkUpdate(Framework fr) {
        RecordingManager.FrameworkUpdate(fr);
    }
}
