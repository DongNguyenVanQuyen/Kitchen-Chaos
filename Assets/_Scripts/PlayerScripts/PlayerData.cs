using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString64Bytes name;
    public FixedString64Bytes playerId;
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && playerId == other.playerId && colorId == other.colorId && name == other.name;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref playerId);
    }
}
