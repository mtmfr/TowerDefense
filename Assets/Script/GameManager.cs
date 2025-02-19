using System;
using UnityEngine;

public static class GameManager
{
    public static event Action<GameState> OnGameStateChange;
    public static void UpdateGameState(GameState nextGameState)
    {
        OnGameStateChange?.Invoke(nextGameState);
    }
}

public enum GameState
{
    Menu,
    Game,
    Pause,
    GameOver
}
