using System;
using Unity.Netcode;

public enum MatchState : byte
{
    Lobby = 0,
    CoinFlip = 1,
    Setup = 2,
    InGame = 3,
    PostGame = 4
}

public enum PlayerSide : byte
{
    None = 0,
    Attacking = 1,
    Defending = 2
}

/// <summary>
/// NetworkList element for each player.
/// IMPORTANT: It's a struct so you must assign back by index after editing.
/// </summary>
public struct PlayerSlot : INetworkSerializable, IEquatable<PlayerSlot>
{
    public ulong ClientId;
    public bool Ready;
    public PlayerSide Side;

    public PlayerSlot(ulong clientId, bool ready, PlayerSide side)
    {
        ClientId = clientId;
        Ready = ready;
        Side = side;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Ready);

        byte side = (byte)Side;
        serializer.SerializeValue(ref side);
        if (serializer.IsReader) Side = (PlayerSide)side;
    }

    public bool Equals(PlayerSlot other) => ClientId == other.ClientId;
}
