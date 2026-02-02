using Unity.Netcode;
using UnityEngine;

public class NetDebugCallbacks : MonoBehaviour
{
    private void Start()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        nm.OnServerStarted += () => Debug.Log("SERVER STARTED (Host is running)");
        nm.OnClientConnectedCallback += id => Debug.Log($"CLIENT CONNECTED: {id}");
        nm.OnClientDisconnectCallback += id => Debug.Log($"CLIENT DISCONNECTED: {id}");
    }
}