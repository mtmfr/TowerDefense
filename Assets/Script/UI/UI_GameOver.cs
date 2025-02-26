using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(Restart);

        GameOverEvent.OnEndGameScore += UpdateScore;
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveListener(Restart);

        GameOverEvent.OnEndGameScore -= UpdateScore;
    }

    private void UpdateScore(int newScore)
    {
        score.text = newScore.ToString();
    }

    private void Restart()
    {
        GameManager.UpdateGameState(GameState.Menu);
    }
}

public static class GameOverEvent
{
    public static event Action<int> OnEndGameScore;
    public static void UpdateEndGameScore(int endScore) => OnEndGameScore?.Invoke(endScore);
}