using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private bool _ready;

    private void Start()
    {
        readyButton.onClick.AddListener(ToggleReady);
        InvokeRepeating(nameof(Refresh), 0.1f, 0.2f); // quick & dirty refresh
    }

    private void ToggleReady()
    {
        if (MatchController.Instance == null) return;

        _ready = !_ready;
        MatchController.Instance.RequestReady(_ready);
    }

    private void Refresh()
    {
        var mc = MatchController.Instance;

        var nm = NetworkManager.Singleton;
        bool connected = nm != null && (nm.IsClient || nm.IsServer);

        if (mc == null || !connected)
        {
            statusText.text = "Not connected.";
            readyButton.interactable = false;
            return;
        }

        readyButton.interactable = mc.State.Value == MatchState.Lobby;

        var sb = new StringBuilder();
        sb.AppendLine($"State: {mc.State.Value}");
        sb.AppendLine($"Players: {mc.Players.Count}/2");

        for (int i = 0; i < mc.Players.Count; i++)
        {
            var p = mc.Players[i];
            sb.AppendLine($"- {p.ClientId} | Ready={p.Ready} | Side={p.Side}");
        }

        if (mc.State.Value == MatchState.CoinFlip || mc.State.Value == MatchState.Setup)
            sb.AppendLine($"Coin Winner: {mc.CoinFlipWinnerClientId.Value}");

        statusText.text = sb.ToString();
    }

}
