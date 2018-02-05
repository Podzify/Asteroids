﻿using UnityEngine;

public class SpriteSwitcher : MonoBehaviour {

    public Sprite[] playerSprite;
    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        MovePlayerControlled.OnPlayerAccelerate += SpriteThrust;
        MovePlayerControlled.OnPlayerStop += SpriteIdle;
    }

    private void OnDisable()
    {
        MovePlayerControlled.OnPlayerAccelerate -= SpriteThrust;
        MovePlayerControlled.OnPlayerStop -= SpriteIdle;
    }

    private void Start ()
    {
        SpriteIdle();
	}

    public void SpriteIdle()
    {
        rend.sprite = playerSprite[0];
    }

    public void SpriteThrust()
    {
        rend.sprite = playerSprite[1];
    }
}
