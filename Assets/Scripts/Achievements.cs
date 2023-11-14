using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour {

    private const int nAchievements = 3;
    public enum Achievement_ID {
        Coin_Collector,
        Terminator,
        SomeOtherFancyAchievement
    } //*
    
    // Are the achievements unlocked
    private bool[] bUnlockedAchievements = new bool[nAchievements]; //*
    

    private int nCoins = 0; //*
    private int nSlain = 0; //Kuinka monta enemii o ded
    private void Start() {
        // Add our method to listen to Coin-class event:
        Coin.OnCoinCollected += CoinWasCollected;
        Enemy.OnEnemySlain += EnemiesWereTerminated;
    }

    void CoinWasCollected() {
        nCoins++;
        if (nCoins == 5) {
            int index = (int)Achievement_ID.Coin_Collector;
            if (!bUnlockedAchievements[index]) {
                bUnlockedAchievements[index] = true;
                Debug.Log("You've unlocked: COIN COLLECTOR!!!");
            }
        }
    } //*
    void EnemiesWereTerminated()
    {
        nSlain++;
        if (nSlain > 10) //Kun enemmän kuin 10 vihollista ded, olet terminaattori
        {
            int index = (int)Achievement_ID.Terminator;
            if (!bUnlockedAchievements[index])
            {
                bUnlockedAchievements[index] = true;
                Debug.Log("You've unlocked: TERMINATOR!!!");
            }
        }
    } //*

}
