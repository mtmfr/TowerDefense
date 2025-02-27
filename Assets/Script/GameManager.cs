using System;
using UnityEngine;

public static class GameManager
{
    public static int gold { get; private set; }

    public static event Action<GameState> OnGameStateChange;
    public static void UpdateGameState(GameState nextGameState)
    {
        OnGameStateChange?.Invoke(nextGameState);
    }

    public static event Action<int> OnGoldValueChanged;

    public static void AddGold(int goldGained)
    {
        gold += goldGained;
        OnGoldValueChanged?.Invoke(gold);
    }

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
