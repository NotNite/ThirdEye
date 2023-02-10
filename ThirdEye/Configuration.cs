using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.IO;

namespace ThirdEye {
    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 0;

        public string OutputDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ThirdEye");

        public bool AutoRecordInCombat = true;
        public ushort TickInterval = 500;

        [NonSerialized] private DalamudPluginInterface? _pluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            _pluginInterface = pluginInterface;
        }

        public void Save() {
            _pluginInterface!.SavePluginConfig(this);
        }
    }
}
