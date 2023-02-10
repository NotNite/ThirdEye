using System;
using System.IO;
using Dalamud.Logging;

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
        
        bw.BaseStream.Seek(0, SeekOrigin.Begin);
        PluginLog.Verbose($"RecordingBlockPlayer len: {bw.BaseStream.Length}");
        return ((MemoryStream)bw.BaseStream).ToArray();
    }

    public bool MatchesOther(RecordingBlockPlayer other) {
        var fpTolerance = 0.5;
        return ContentId == other.ContentId &&
               CurrentHp == other.CurrentHp &&
               MaxHp == other.MaxHp &&
               CurrentMp == other.CurrentMp &&
               MaxMp == other.MaxMp &&
               Math.Abs(X - other.X) < fpTolerance &&
               Math.Abs(Y - other.Y) < fpTolerance &&
               Math.Abs(Z - other.Z) < fpTolerance &&
               Math.Abs(Rotation - other.Rotation) < fpTolerance &&
               StatusCount == other.StatusCount &&
               Statuses == other.Statuses;
    }
}
