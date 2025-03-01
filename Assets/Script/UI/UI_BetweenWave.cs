using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_BetweenWave : MonoBehaviour
{
    [SerializeField] private Button startNextWave;
    [SerializeField] private Button openTurretSelection;

    [SerializeField] private GameObject SelectionButtons;
    [SerializeField] private GameObject TurretSelection;

    [Header("Refusal")]
    [SerializeField] private GameObject SelectionRefusal;
    [SerializeField] private TextMeshProUGUI SelectionRefusalText;
    [SerializeField] private float ColorFadeTime;
    private Color originalTextColor;

    private void Start()
    {
        originalTextColor = SelectionRefusalText.color;
        BetweenWaveEvent.TurretSelectionFinished(false);
    }

    private void OnEnable()
    {
        startNextWave.onClick.AddListener(StartNextWave);
        openTurretSelection.onClick.AddListener(OpenTurretSelection);
    }

    private void OnDisable()
    {
        startNextWave.onClick.RemoveListener(StartNextWave);
        openTurretSelection.onClick.RemoveListener(OpenTurretSelection);
    }

    private void StartNextWave()
    {
        GameManager.UpdateGameState(GameState.Game);
    }

    public void OpenTurretSelection()
    {
        SelectionButtons.SetActive(false);
        TurretSelection.SetActive(true);
        BetweenWaveEvent.TurretSelectionFinished(false);
    }

    public void SelectedTurret(BaseTurrets selectedTurret)
    {
        if (GameManager.gold - selectedTurret.turretsStats[0].cost < 0)
        {
            SelectionFailed();
            return;
        }

        SelectorEvent.TurretToPlaceChanged(selectedTurret);
        SelectionButtons.SetActive(true);
        TurretSelection.SetActive(false);
        BetweenWaveEvent.TurretSelectionFinished(true);
    }

    private void SelectionFailed()
    {
        SelectionRefusal.SetActive(true);
        StopCoroutine(RefusalFade());
        StartCoroutine(RefusalFade());
    }

    private IEnumerator RefusalFade()
    {
        SelectionRefusalText.color = originalTextColor;

        Color fadeColor = SelectionRefusalText.color;

        for(float currentLerpTime = 0; currentLerpTime / ColorFadeTime < 0.95f; currentLerpTime += Time.deltaTime)
        {
            if (fadeColor.a < 0.05f)
                break;

            fadeColor.a = Mathf.Lerp(fadeColor.a, 0, currentLerpTime / ColorFadeTime);
            SelectionRefusalText.color = fadeColor;
            yield return null;
        }
        SelectionRefusal.SetActive(false);
    }
}

public static class BetweenWaveEvent
{
    public static event Action<bool> OnTurretSelection;
    public static void TurretSelectionFinished(bool isSelectingTurret) => OnTurretSelection?.Invoke(isSelectingTurret);
}
