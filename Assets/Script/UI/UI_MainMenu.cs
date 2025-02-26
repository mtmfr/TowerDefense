using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Buttons. The action to do is assigned in script")]
    [SerializeField] private Button startGame;
    [SerializeField] private Button optionMenu;
    [SerializeField] private Button exitGame;

    [Header("Option menu")]
    [SerializeField] private GameObject OptionUI;
    private bool isOptionShowed;

    private void OnEnable()
    {
        startGame.onClick.AddListener(OnStartGame);
        optionMenu.onClick.AddListener(OnOptionMenu);
        exitGame.onClick.AddListener(OnExitGame);
    }

    private void OnDisable()
    {
        startGame.onClick.RemoveAllListeners();
        optionMenu.onClick.RemoveAllListeners();
        exitGame.onClick.RemoveAllListeners();
    }

    private void OnStartGame()
    {
        GameManager.UpdateGameState(GameState.GeneratingPath);
    }

    private void OnOptionMenu()
    {
        isOptionShowed = !isOptionShowed;

        OptionUI.SetActive(isOptionShowed);
    }

    private void OnExitGame()
    {
        Application.Quit();
    }
}
