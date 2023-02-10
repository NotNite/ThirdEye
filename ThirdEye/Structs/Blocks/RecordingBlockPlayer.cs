using System.IO;

namespace ThirdEye.Structs;

struct RecordingBlockPlayer {
    public long ContentId;

    public uint CurrentHp;
    public uint MaxHp;
    public uint CurrentMp;
    public uint MaxMp;

    public float X;
    public float Y;
    public float Z;
    public float Rotation;

    public byte StatusCount;
    public RecordingStatus[] Statuses;

    public byte[] GetBytes() {
        var bw = new BinaryWriter(new MemoryStream());
        bw.Write(ContentId);
        
        bw.Write(CurrentHp);
        bw.Write(MaxHp);
        bw.Write(CurrentMp);
        bw.Write(MaxMp);
        
        bw.Write(X);
        bw.Write(Y);
        bw.Write(Z);
        bw.Write(Rotation);
        
        bw.Write(StatusCount);

        foreach (var status in Statuses) {
            bw.Write(status.GetBytes());
        }

        return ((MemoryStream)bw.BaseStream).ToArray();
    }
}
