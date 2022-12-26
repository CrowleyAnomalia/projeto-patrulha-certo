using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Assets/GameModes/CollectCoins", fileName = "CollectCoinsGameModeSO")]
public class CollectCoinsGameMode : GameModeSO
{
    public int coinsToWin;
    public float timeToWin;
    
    /// <summary>
    /// Atualiza o estado do tipo de jogo Collect Coins
    /// </summary>
    /// <param name="intValue">quantidade de moedas coletadas</param>
    /// <param name="floatValue">tempo que se passou desde o inicio da partida</param>
    public override void UpdateGameState([Optional] int intValue, [Optional] float floatValue, [Optional] bool boolValue)
    {
        if (intValue >= coinsToWin) GameState = GameState.Victory;

        if (floatValue >= timeToWin) GameState = GameState.GameOver;
    }
}
