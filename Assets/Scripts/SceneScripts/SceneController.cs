using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    const string sceneToLoad = "Assets/_Projects"

    void Start()
    {
        var op = SceneManager.LoadScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
