using Unity.Netcode;
using UnityEngine;

public class NetUI : MonoBehaviour
{
    public void StartHost()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsListening)
        {
            Debug.Log("Already listening; shutting down first ...");
            nm.Shutdown();
        }

        bool ok = nm.StartHost();
        Debug.Log($"StartHost() => {ok}");
    }

    public void StartClient()
    {
        var nm = NetworkManager.Singleton;
        if (nm == null) return;

        if (nm.IsListening)
        {
            Debug.Log("Already listening: shutting down first...");
            nm.Shutdown();
        }

        bool ok = nm.StartClient();
        Debug.Log($"StartClient() => {ok}");
    }

    public void Shutdown()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.Shutdown();
    }
}