using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Slider health;

    private void OnEnable()
    {
        GameManager.OnGameStateChange += ShowUI;
        GameManager.OnGoldValueChanged += UpdateGoldText;

        GameUIEvent.OnSetHealth += SetHealth;
        GameUIEvent.OnUpdateHealth += UpdateHealth;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= ShowUI;
        GameManager.OnGoldValueChanged -= UpdateGoldText;

        GameUIEvent.OnSetHealth -= SetHealth;
        GameUIEvent.OnUpdateHealth -= UpdateHealth;
    }

    private void SetHealth(int maxHealth)
    {
        health.maxValue = maxHealth;
        health.value = maxHealth;
    }

    private void UpdateHealth(int newHealth)
    {
        health.value = newHealth;
    }

    private void UpdateGoldText(int newGold)
    {
        goldText.text = newGold.ToString();
    }

    private void ShowUI(GameState gameState)
    {
        bool isVisible = gameState switch
        {
            GameState.Game => true,
            _ => false
        };

        health.gameObject.SetActive(isVisible);
    }
}

public static class GameUIEvent
{
    public static event Action<int> OnSetHealth;
    public static void SetHealth(int maxHealth)
    {
        OnSetHealth?.Invoke(maxHealth);
    }

    public static event Action<int> OnUpdateHealth;
    public static void UpdateHealth(int newHealth)
    {
        OnUpdateHealth?.Invoke(newHealth);
    }
}
