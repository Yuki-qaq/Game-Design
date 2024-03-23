using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameFlow", menuName = "ScriptableObjects/Game Flow", order = 1)]
public class GameFlowScriptable : ScriptableObject
{
    public float slowMotionScale = 0.25f;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void LoadPreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
        Time.timeScale = slowMotionScale;
    }

    public void SlowMotionToggle()
    {
        if (Time.timeScale < 1)
        {
            NormalTime();
        }
        else
        {
            SlowMotionTime();
        }
    }

    public void CustomTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
