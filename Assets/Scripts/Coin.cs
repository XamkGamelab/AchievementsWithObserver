using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour {
    // The event / action "list" that has all "observers" registered
    public static event Action OnCoinCollected; //*

    private void OnDisable() {
        OnCoinCollected?.Invoke();
    }
}
