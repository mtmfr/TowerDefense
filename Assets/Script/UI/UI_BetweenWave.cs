using UnityEngine;
using UnityEngine.UI;

public class UI_BetweenWave : MonoBehaviour
{
    [SerializeField] private Button startNextWave;

    private void OnEnable()
    {
        startNextWave.onClick.AddListener(StartNextWave);
    }

    private void OnDisable()
    {
        startNextWave.onClick.RemoveListener(StartNextWave);
    }

    private void StartNextWave()
    {
        GameManager.UpdateGameState(GameState.Game);
    }
}
