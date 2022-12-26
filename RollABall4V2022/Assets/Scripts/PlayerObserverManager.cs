using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esse seria o nosso Youtube com coisas do Jogador
public static class PlayerObserverManager
{
    // Esse aqui vai ser o nosso canal para atualizações da quantidade de coins do jogador
    public static Action<int> OnPlayerCoinsChanged;
    
    // A segunda parte é como o player notifica seus inscritos que as moedas mudaram
    public static void PlayerCoinsChanged(int value)
    {
        OnPlayerCoinsChanged?.Invoke(value);
    }
}
