using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "WargateMainMenu";

    private void Awake()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}