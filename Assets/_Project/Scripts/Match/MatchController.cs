using System;
using System.Linq;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class MatchController : NetworkBehaviour
{
    public static MatchController Instance { get; private set; }

    public NetworkVariable<MatchState> State = new(
        MatchState.Lobby,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<ulong> CoinFlipWinnerClientId = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkList<PlayerSlot> Players;

    [SerializeField] private int maxPlayers = 2;

    private void Awake()
    {
        Instance = this;
        Players = new NetworkList<PlayerSlot>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Build slots for already-connected clients (host is included)
            RebuildSlotsFromConnectedClients();

            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;

            // Force initial lobby state
            SetStateServer(MatchState.Lobby);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!IsServer) return;

        // 1v1 guard: ignore additional clients for now (or kick later)
        if (Players.Count >= maxPlayers)
        {
            Debug.LogWarning($"Client {clientId} connected but lobby is full (max {maxPlayers}).");
            // Optional: NetworkManager.DisconnectClient(clientId);
            return;
        }

        EnsureSlotExists(clientId);
        Debug.Log($"Client connected: {clientId}. Slots now: {Players.Count}/{maxPlayers}");

        // If someone joins mid-lobby, keep state in Lobby.
        if (State.Value != MatchState.Lobby)
            SetStateServer(MatchState.Lobby);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (!IsServer) return;

        RemoveSlot(clientId);

        Debug.Log($"Client disconnected: {clientId}. Slots now: {Players.Count}/{maxPlayers}");

        // Reset to Lobby if someone leaves.
        SetStateServer(MatchState.Lobby);

        // Reset coin flip + sides
        CoinFlipWinnerClientId.Value = 0;
        ResetAllSidesAndReady();
    }

    private void RebuildSlotsFromConnectedClients()
    {
        Players.Clear();

        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            if (Players.Count >= maxPlayers) break;
            Players.Add(new PlayerSlot(clientId, ready: false, side: PlayerSide.None));
        }
    }

    private void EnsureSlotExists(ulong clientId)
    {
        for (int i = 0; i < Players.Count; i++)
            if (Players[i].ClientId == clientId)
                return;

        Players.Add(new PlayerSlot(clientId, ready: false, side: PlayerSide.None));
    }

    private void RemoveSlot(ulong clientId)
    {
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            if (Players[i].ClientId == clientId)
            {
                Players.RemoveAt(i);
                return;
            }
        }
    }

    private void ResetAllSidesAndReady()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            var slot = Players[i];
            slot.Ready = false;
            slot.Side = PlayerSide.None;
            Players[i] = slot;
        }
    }

    private void SetStateServer(MatchState newState)
    {
        if (!IsServer) return;
        if (State.Value == newState) return;

        Debug.Log($"[MatchController] State: {State.Value} -> {newState}");
        State.Value = newState;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetReadyRpc(bool ready, RpcParams rpcParams = default)
    {
        HandleReady(rpcParams.Receive.SenderClientId, ready);
    }

    public void RequestReady(bool ready)
    {
        SetReadyRpc(ready);
    }


    private void HandleReady(ulong senderClientId, bool ready)
    {
        if (!IsServer) return;
        if (State.Value != MatchState.Lobby) return; // only accept ready in lobby

        int idx = FindPlayerIndex(senderClientId);
        if (idx < 0) return;

        var slot = Players[idx];
        slot.Ready = ready;
        Players[idx] = slot;

        Debug.Log($"Client {senderClientId} ready={ready}");

        TryAdvanceFromLobby();
    }

    private int FindPlayerIndex(ulong clientId)
    {
        for (int i = 0; i < Players.Count; i++)
            if (Players[i].ClientId == clientId)
                return i;
        return -1;
    }

    private void TryAdvanceFromLobby()
    {
        if (!IsServer) return;

        if (Players.Count < maxPlayers)
            return;

        bool allReady = true;
        for (int i = 0; i < Players.Count; i++)
            allReady &= Players[i].Ready;

        if (!allReady)
            return;

        RunCoinFlipAndAssignSides();
    }

    private void RunCoinFlipAndAssignSides()
    {
        SetStateServer(MatchState.CoinFlip);

        // Fair random bit on server
        int bit = GetSecureRandomBit();
        ulong winner = Players[bit].ClientId;

        CoinFlipWinnerClientId.Value = winner;

        // Assign sides: winner = SideA, other = SideB
        for (int i = 0; i < Players.Count; i++)
        {
            var slot = Players[i];
            slot.Side = (slot.ClientId == winner) ? PlayerSide.Attacking : PlayerSide.Defending;
            Players[i] = slot;
        }

        Debug.Log($"Coin flip winner: {winner}. Assigned sides.");

        SetStateServer(MatchState.Setup);
    }

    private static int GetSecureRandomBit()
    {
        Span<byte> b = stackalloc byte[1];
        RandomNumberGenerator.Fill(b);
        return (b[0] & 1);
    }
}
