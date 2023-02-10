using System.IO;

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
        
        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}
