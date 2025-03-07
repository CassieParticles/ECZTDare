using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeTracker : MonoBehaviour
{
    private static SceneChangeTracker instance;

    private string lastSceneName;

    public static SceneChangeTracker GetTracker()
    {
        return instance;
    }
    public void ChangeScene(string newScene)
    {
        lastSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(newScene);
    }

    public void GoBack()
    {
        if (lastSceneName == null){ return; }
        SceneManager.LoadScene(lastSceneName);
        lastSceneName = null;
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        lastSceneName = null;
        DontDestroyOnLoad(gameObject);
    }
}
