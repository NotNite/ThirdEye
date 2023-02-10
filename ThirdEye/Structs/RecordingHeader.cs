using System.IO;

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

        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}
