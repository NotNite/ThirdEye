using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Logging;
using ThirdEye.Structs;

namespace ThirdEye;

public class RecordingManager : IDisposable {
    private bool _recording;
    private FileStream? _fileStream;

    private bool _lastCombat;
    private DateTime _lastCombatTime = DateTime.Now;

    private Dictionary<long, byte[]> _lastRecordings = new();

    public bool IsRecording => _recording;

    public void StartRecording() {
        if (_recording) return;
        Plugin.ChatGui.Print("[Third Eye] Started recording.");

        _recording = true;
        _lastCombatTime = DateTime.Now;
        _lastRecordings.Clear();

        var filename = $"ThirdEye_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.dat";
        var path = Path.Combine(Plugin.Configuration.OutputDirectory, filename);
        _fileStream = new FileStream(path, FileMode.Create);

        WriteHeader();
    }

    public void StopRecording() {
        if (!_recording) return;
        Plugin.ChatGui.Print("[Third Eye] Stopped recording.");

        _recording = false;
        _lastCombatTime = DateTime.Now;
        _lastRecordings.Clear();

        _fileStream?.Close();
        _fileStream = null;
    }

    public void Dispose() {
        if (_recording) StopRecording();
    }

    public void FrameworkUpdate(Framework fr) {
        var currentCombat = Plugin.Condition[ConditionFlag.InCombat];

        if (Plugin.Configuration.AutoRecordInCombat) {
            // TODO: currentInstance
            if (!_lastCombat && currentCombat) {
                StartRecording();
            } else if (_lastCombat && !currentCombat) {
                StopRecording();
            }
        }

        _lastCombat = currentCombat;

        if (!_recording) return;

        var now = DateTime.Now;
        var elapsed = now - _lastCombatTime;
        if (elapsed > TimeSpan.FromMilliseconds(Plugin.Configuration.TickInterval)) {
            _lastCombatTime = _lastCombatTime.AddMilliseconds(Plugin.Configuration.TickInterval);
            PluginLog.Verbose("Recording tick");

            // write out self
            var localPlayer = Plugin.ClientState.LocalPlayer;
            if (localPlayer is not null) {
                var statuses = FormStatusesFromStatusList(localPlayer.StatusList);

                var player = new RecordingBlockPlayer {
                    ContentId = (long)Plugin.ClientState.LocalContentId,

                    CurrentHp = localPlayer.CurrentHp,
                    MaxHp = localPlayer.MaxHp,
                    CurrentMp = localPlayer.CurrentMp,
                    MaxMp = localPlayer.MaxMp,

                    X = localPlayer.Position.X,
                    Y = localPlayer.Position.Y,
                    Z = localPlayer.Position.Z,
                    Rotation = localPlayer.Rotation,

                    StatusCount = (byte)statuses.Length,
                    Statuses = statuses
                };

                WritePlayerBlock(player);
            }

            // write out party
            foreach (var partyMember in Plugin.PartyList) {
                var statuses = FormStatusesFromStatusList(partyMember.Statuses);

                var player = new RecordingBlockPlayer {
                    ContentId = partyMember.ContentId,

                    CurrentHp = partyMember.CurrentHP,
                    MaxHp = partyMember.MaxHP,
                    CurrentMp = partyMember.CurrentMP,
                    MaxMp = partyMember.MaxMP,

                    X = partyMember.Position.X,
                    Y = partyMember.Position.Y,
                    Z = partyMember.Position.Z,
                    Rotation = partyMember.GameObject != null ? partyMember.GameObject.Rotation : 0f,

                    StatusCount = (byte)statuses.Length,
                    Statuses = statuses
                };

                WritePlayerBlock(player);
            }
        }
    }

    private void WriteHeader() {
        if (_fileStream == null) return;

        var fh = new RecordingHeader {
            Version = 1,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            TerritoryType = Plugin.ClientState.TerritoryType,
            TickMs = Plugin.Configuration.TickInterval
        };

        var bytes = fh.GetBytes();
        PluginLog.Verbose($"Writing header: {bytes.Length}");
        _fileStream.Write(bytes, 0, bytes.Length);
    }

    private void WritePlayerBlock(RecordingBlockPlayer block) {
        if (_fileStream == null) return;

        var bytes = block.GetBytes();

        if (_lastRecordings.TryGetValue(block.ContentId, out var lastBytes) && lastBytes.SequenceEqual(bytes)) {
            PluginLog.Verbose($"Skipping block for {block.ContentId} because it's the same as the last one");
            return;
        }

        _lastRecordings[block.ContentId] = bytes;

        var header = new RecordingBlockHeader {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Type = RecordingBlockType.Player,
            DataSize = bytes.Length,
            Data = bytes
        };

        var headerBytes = header.GetBytes();
        PluginLog.Verbose($"Writing player block: {headerBytes.Length}");
        _fileStream.Write(headerBytes, 0, headerBytes.Length);
    }

    private RecordingStatus[] FormStatusesFromStatusList(StatusList list) {
        var result = new List<RecordingStatus>();

        foreach (var status in list) {
            result.Add(new RecordingStatus {
                Id = status.StatusId,
                Stack = status.StackCount,
                RemainingTime = status.RemainingTime,
                Param = status.Param
            });
        }

        return result.ToArray();
    }
}
