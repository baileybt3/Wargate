using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "WargateMainMenu";
    [SerializeField] private SceneController sceneController;

    private void Awake()
    {
        // if NetworkManager already exists, don't want another bootstrap stack
        if (NetworkManager.Singleton != null)
        {
            // Make NetworkManager persists
            DontDestroyOnLoad(NetworkManager.Singleton.gameObject);

            // If SceneController is on this scene, persist it
            if (sceneController != null)
                DontDestroyOnLoad(sceneController.gameObject);

            // This bootstrapper object can be destroyed if it's not the NetworkManager object
            if (gameObject != NetworkManager.Singleton.gameObject)
                Destroy(gameObject);

            return;
        }

        DontDestroyOnLoad(gameObject);

        if (sceneController != null)
            DontDestroyOnLoad(sceneController.gameObject);
    }

    private void Start()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}