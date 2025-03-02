using System;
using UnityEngine;

public static class GameManager
{
    public static int gold { get; private set; }

    public static event Action<GameState> OnGameStateChange;
    public static void UpdateGameState(GameState nextGameState)
    {
        switch (nextGameState)
        {
            case GameState.Menu:
                gold = 10;
                GC.Collect();
                break;
            case GameState.Shop:
                GC.Collect();
                break;
        }
        OnGameStateChange?.Invoke(nextGameState);
    }

    public static event Action<int> OnGoldValueChanged;

    /// <summary>
    /// Add the amount of gold Gained to gold
    /// </summary>
    public static void AddGold(int goldGained)
    {
        gold += goldGained;
        OnGoldValueChanged?.Invoke(gold);
    }

    /// <summary>
    /// Remove the amount of gold used from gold
    /// </summary>
    public static void UseGold(int goldUsed)
    {
        gold -= goldUsed;
        OnGoldValueChanged?.Invoke(gold);
    }
}

public enum GameState
{
    Menu,
    GeneratingPath,
    Game,
    Shop,
    Pause,
    GameOver,
}
