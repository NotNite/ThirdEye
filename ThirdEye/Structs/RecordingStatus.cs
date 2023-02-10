using System.IO;
using Dalamud.Logging;

namespace ThirdEye.Structs;

struct RecordingStatus {
    public uint Id;
    public byte Stack;
    public float RemainingTime;
    public ushort Param;

    public byte[] GetBytes() {
        var bw = new BinaryWriter(new MemoryStream());
        bw.Write(Id);
        bw.Write(Stack);
        bw.Write(RemainingTime);
        bw.Write(Param);
        
        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        PluginLog.Verbose($"RecordingStatus len: {bw.BaseStream.Length}");
        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}
