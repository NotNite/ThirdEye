using System.IO;
using Dalamud.Logging;

namespace ThirdEye.Structs;

public struct RecordingBlockHeader {
    public long Timestamp;
    public RecordingBlockType Type;
    public int DataSize;
    public byte[] Data;

    public byte[] GetBytes() {
        var bw = new BinaryWriter(new MemoryStream());

        bw.Write(Timestamp);
        bw.Write((byte)Type);
        bw.Write(DataSize);
        bw.Write(Data);

        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        PluginLog.Verbose($"RecordingBlock len: {bw.BaseStream.Length}");
        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}

public enum RecordingBlockType : byte {
    Player = 0
}
