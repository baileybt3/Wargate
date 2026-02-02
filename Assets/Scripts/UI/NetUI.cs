using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetUI : MonoBehaviour
{
    [SerializeField] private GameObject connectPanel;
    [SerializeField] private GameObject lobbyPanel;

    private void Awake()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        nm.OnClientConnectedCallback += OnClientConnected;
        nm.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        nm.OnClientConnectedCallback -= OnClientConnected;
        nm.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    private void SetPanels(bool connected)
    {
        if (connectPanel) connectPanel.SetActive(!connected);
        if (lobbyPanel) lobbyPanel.SetActive(connected);
    }

    private void OnClientConnected(ulong id)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        // Only flip UI when *this* instance is connected
        if (id == nm.LocalClientId)
            SetPanels(true);
    }

    private void OnClientDisconnected(ulong id)
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (id == nm.LocalClientId)
            SetPanels(false);
    }

    public void StartHost() => StartCoroutine(StartHostCo());
    public void StartClient() => StartCoroutine(StartClientCo());

    private IEnumerator StartHostCo()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) yield break;

        if (nm.IsListening || nm.ShutdownInProgress)
        {
            nm.Shutdown();
            yield return null;
            yield return null;
        }

        bool ok = nm.StartHost();
        Debug.Log($"StartHost() => {ok}");

        if (!ok) SetPanels(false);
        // if ok, UI will switch on local connect callback
    }

    private IEnumerator StartClientCo()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) yield break;

        if (nm.IsListening || nm.ShutdownInProgress)
        {
            nm.Shutdown();
            yield return null;
            yield return null;
        }

        bool ok = nm.StartClient();
        Debug.Log($"StartClient() => {ok}");

        if (!ok) SetPanels(false);
    }

    public void Shutdown()
    {
        var nm = NetworkManager.Singleton;
        if (nm != null) nm.Shutdown();
        SetPanels(false);
    }
}
