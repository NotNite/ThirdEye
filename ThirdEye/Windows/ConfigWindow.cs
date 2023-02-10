using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ThirdEye.Windows;

public class ConfigWindow : Window, IDisposable {
    private FileDialogManager _fileDialogManager = new();

    public ConfigWindow() : base("Third Eye Config") { }
    public void Dispose() { }

    public override void Draw() {
        _fileDialogManager.Draw();
        
        if (ImGuiComponents.IconButton(FontAwesomeIcon.FolderOpen)) {
            _fileDialogManager.OpenFolderDialog("Select folder", (b, s) => {
                if (!b) return;
                Plugin.Configuration.OutputDirectory = s;
                Plugin.Configuration.Save();
            });
        }
        
        ImGui.SameLine();
        
        if (ImGui.InputText("Recording directory", ref Plugin.Configuration.OutputDirectory, 260)) {
            Plugin.Configuration.Save();
        }

        if (ImGui.Checkbox("Automatically start/stop recording", ref Plugin.Configuration.AutoRecordInCombat)) {
            Plugin.Configuration.Save();
        }
        
        var text = Plugin.RecordingManager.IsRecording ? "Stop recording" : "Start recording";
        if (ImGui.Button(text)) {
            if (Plugin.RecordingManager.IsRecording) {
                Plugin.RecordingManager.StopRecording();
            } else {
                Plugin.RecordingManager.StartRecording();
            }
        }
    }
}
