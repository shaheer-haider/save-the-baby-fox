﻿using UnityEngine;
using System;
using System.Collections;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int Coins { get; private set; }

    public static event Action<int> CoinsUpdated = delegate {};

    [SerializeField]
    int INITIAL_COINS = 0;
    const string COINS = "COINS";   // key name to store high score in PlayerPrefs

    void Awake()
    {
        INITIAL_COINS = PlayerPrefs.GetInt(COINS, 0);
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        // Initialize coins
        Coins = PlayerPrefs.GetInt(COINS, INITIAL_COINS);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;


        // Store new coin value
        PlayerPrefs.SetInt(COINS, Coins);

        // Fire event
        CoinsUpdated(Coins);
    }

    public void RemoveCoins(int amount)
    {
        Coins -= amount;

        // Store new coin value
        PlayerPrefs.SetInt(COINS, Coins);

        // Fire event
        CoinsUpdated(Coins);
    }
}
