using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManagement : MonoBehaviour
{
    public static void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public static void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void NormalTime()
    {
        Time.timeScale = 1;
    }

    public void SlowMotionTime()
    {
        Time.timeScale = 0.25f;
    }

    public void CustomTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
