using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action OnEnemySlain;

    private void OnDestroy()
    {
        OnEnemySlain?.Invoke();
    }
}
