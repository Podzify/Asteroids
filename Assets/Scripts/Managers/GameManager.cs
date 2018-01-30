﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // -------------------------------------------------------------------------
    // Messaging delegates
    // -------------------------------------------------------------------------

    public static Action<int> OnScoreChanged;
    public static Action<string, float> OnAnnounceMessage;
    public static Action OnLevelStarted;

    // -------------------------------------------------------------------------
    // Inspector variables
    // -------------------------------------------------------------------------

    public int startingAsteroids;
    public static int level;
    public GameObject PrefabAsteroid;
    public GameObject PrefabUFO;
    public GameObject PrefabPowerup;
    public PlayerController player;
    public float UFOSpawnFrequency;
    public float UFOSpawnProbability;

    // -------------------------------------------------------------------------
    // Private variables and properties
    // -------------------------------------------------------------------------

    private int playerScore;
    public int PlayerScore { get { return playerScore; } }

    // -------------------------------------------------------------------------
    // Setup methods
    // -------------------------------------------------------------------------

    void Awake()
    {
        Assert.IsNotNull(PrefabAsteroid);
        Assert.IsNotNull(PrefabUFO);
        Assert.IsNotNull(player);

        // Reset the (static) count variable
        AsteroidController.countAsteroids = 0;

        level = 0;
    }

    void OnEnable()
    {
        AsteroidController.OnLastAsteroidDestroyed += NextLevel;
        UFOController.OnScorePoints += HandleScorePoints;
        UFOController.OnUFODespawned += StartUFOSpawner;
        AsteroidController.OnScorePoints += HandleScorePoints;
    }

    void OnDisable()
    {
        AsteroidController.OnLastAsteroidDestroyed -= NextLevel;
        UFOController.OnScorePoints -= HandleScorePoints;
        UFOController.OnUFODespawned -= StartUFOSpawner;
        AsteroidController.OnScorePoints -= HandleScorePoints;
    }

    void Start()
    {
        // Spawn player
        player = Instantiate(player, Vector2.zero, Quaternion.identity);
        player.Lives = 3;

        // Start the first level
        NextLevel();
    }



    void NextLevel()
    {
        // Increase level number, display it for three seconds,
        // disable (hide) the player while doing do
        level += 1;
        Debug.Log("[GameManager/NextLevel] Level " + level + " starting");
        OnAnnounceMessage(string.Format("LEVEL {0}", level), 3.0f);
        player.gameObject.SetActive(false);
        StartCoroutine(SpawnAsteroids());
    }



    IEnumerator SpawnAsteroids()
    {
        yield return new WaitForSeconds(3.0f);
        // Spawn asteroids based on level number
        Assert.IsNotNull(PrefabAsteroid);
        for (int i = 0; i < startingAsteroids + level - 1; i++)
        {
            Instantiate(PrefabAsteroid, Vector2.zero, Quaternion.identity);
        }
        Debug.Log("[GameManager/SpawnAsteroids] Asteroids spawned");
        player.gameObject.SetActive(true); // FIXME find a clean solution for spawning
        if (OnLevelStarted != null) OnLevelStarted();
        StartUFOSpawner();
    }

    private void StartUFOSpawner()
    {
        Debug.Log("[GameManager/StartUFOSpawner]");
        StartCoroutine(SpawnUFO());
    }

    IEnumerator SpawnUFO()
    {
        // Allow at least 2 seconds between death and respawn
        yield return new WaitForSeconds(3.0f);

        // If the random value is higher than the probability,
        // wait some more (spawnFrequency in seconds)
        while (Random.value > UFOSpawnProbability)
        {
            yield return new WaitForSeconds(UFOSpawnFrequency);
        }

        // Spawn UFO and add the player object as its target
        GameObject ufo = Instantiate(PrefabUFO);
        ufo.GetComponent<ICanFireAtTarget>().Target = player.gameObject;
        Debug.Log("[GameManager/SpawnUFO] Spawned");

    }


    void HandleScorePoints(Entity e)
    {
        playerScore += e.pointValue;
        if (OnScoreChanged != null) OnScoreChanged(playerScore);
    }



}

// TODO: powerups
// TODO: clean player spawning routine
// TODO: fix screen aspect and wrap around issues
// TODO: end level only when UFO not here
// TODO: reset player acceleration sprite when respawning
// TODO: more asteroids sprite variations
