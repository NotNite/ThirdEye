using System.IO;
using Dalamud.Logging;

namespace ThirdEye.Structs;

public struct RecordingHeader {
    public byte Version;
    public long Timestamp;
    public ushort TerritoryType;
    public ushort TickMs;
    
    public byte[] GetBytes() {
        var bw = new BinaryWriter(new MemoryStream());
        bw.Write(Version);
        bw.Write(Timestamp);
        bw.Write(TerritoryType);
        bw.Write(TickMs);
        
        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        PluginLog.Verbose($"RecordingHeader len: {bw.BaseStream.Length}");
        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}
