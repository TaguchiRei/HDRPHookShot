using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager
{
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ReturnBase()
    {
        SceneManager.LoadScene("Base");
    }
}
