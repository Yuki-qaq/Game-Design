using TMPro;
using UnityEngine;

public class IntValueDisplay : MonoBehaviour
{
    public IntVariableScriptable scoreVariable;
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreVariable.OnValueChange += UpdateScoreText;
        UpdateScoreText(scoreVariable.Value);
    }

    private void UpdateScoreText(int newValue)
    {
        scoreText.text = newValue.ToString();
    }
}
