using UnityEngine;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject gameOverUI;

    private void Start()
    {
        GameManager.UpdateGameState(GameState.Menu);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += UpdateUI;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= UpdateUI;
    }

    private void UpdateUI(GameState gameState)
    {
        mainMenu.SetActive(gameState switch
        {
            GameState.Menu => true,
            _ => false
        });

        gameUI.SetActive(gameState switch
        {
            GameState.GeneratingPath => true,
            GameState.Game => true,
            _ => false
        });

        pauseUI.SetActive(gameState switch
        {
            GameState.Pause => true,
            _ => false
        });

        gameOverUI.SetActive(gameState switch
        {
            GameState.GameOver => true,
            _ => false
        });
    }
}