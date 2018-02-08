﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class AsteroidController : Entity, IKillable
{
    // -----------------------------------------------------------------------------
    // Inspector fields
    // -----------------------------------------------------------------------------

    // Explosion prefab to be instantiated when destroyed
    public GameObject explosion;

    // Reference for asteroid sprites variations
    public Sprite[] sprite;

    // -----------------------------------------------------------------------------
    // Properties
    // -----------------------------------------------------------------------------

    // Phase Property
    // Asteroids start at Phase 0
    // and go through 1 & 2 until completely destroyed.
    // When set, adjust the scale of the asteroid to half the previous size.
    // Calculate its new point value and set its name.
    public int Phase
    {
        get { return _phase; }
        private set
        {
            _phase = value;
            float newScale = 1.0f / Mathf.Pow(2, _phase);
            transform.localScale = new Vector3(newScale, newScale, 1);
            gameObject.name = "Asteroid (Phase " + _phase + ")";
        }
    }

    //
    // Private fields
    //

    private int _phase = 0;

    //
    // Static property counting how many are in the scene
    //
    public static int Count { get; private set; }

    //
    //  Events
    //

    public static Action<AsteroidController> OnAsteroidDestroyed; 
    public static Action OnAsteroidLastDestroyed;


    // -----------------------------------------------------------------------------
    // Methods
    // -----------------------------------------------------------------------------

    public override void Awake()
    {
        base.Awake();

        // Set a random sprite variation.
        rend.sprite = sprite[Random.Range(0, 3)];

        // Increase static asteroid count with each instantiation.
        Count += 1;

        // Set default phase property. This is changed only by Break().
        Phase = 0;
    }



    void Start()
    {
        // If it's a new asteroid (instantiated by GameManager),
        // set its transform to a random value.
        if (_phase == 0) 
        {
            transform.position = new Vector2(Random.Range(-15, 15), Random.Range(3, 6)); 
        }

        // Set the mass to be proportionate to the asteroid size
        // This makes inter-asteroid collisions more realistic
        rb.mass = 1 / (_phase + 1);

        // Add position and rotation variations
        float x = Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
        float y = Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
        float rot = Random.Range(0f, 1f);

        // Set the transform
        transform.position = new Vector2(x, y);
        transform.Rotate(new Vector3(0, 0, rot)); 

        // Give the asteroid random force and torque
        float dirX = Random.Range(-1f, 1f);
        float dirY = Random.Range(-1f, 1f);
        Vector2 randomVector = new Vector2(dirX, dirY) * (2 + GameManager.CurrentLevel * 0.5f) * rb.mass;
        rb.AddRelativeForce(randomVector, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-1 * rb.mass, 1 * rb.mass), ForceMode2D.Impulse);
    }



    // (Required by IKillable)
    // Asteroid kill sequence.
    public void Kill()
    {
        // Instantiate an explosion
        Instantiate(explosion, transform.position, Quaternion.identity);

        // Asteroid should break if it's not the smallest size
        if (_phase < 2) { Break(_phase + 1); }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //
        // Decrease the total asteroid count property and fire an event
        //
        Count -= 1;
        if (OnAsteroidDestroyed != null) { OnAsteroidDestroyed(this); }

        //
        // If number of asteroids is zero fire the proper event
        //
        if (Count == 0)
        {
            if (OnAsteroidLastDestroyed != null) { OnAsteroidLastDestroyed(); }
        }
    }



    // Instantiate two copies of this asteroid.
    // Set them to the next phase to make them smaller.
    public void Break(int newPhase)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject obj = Instantiate(gameObject, Vector2.zero, Quaternion.identity);
            obj.transform.position = transform.position;
            AsteroidController astController = obj.GetComponent<AsteroidController>();
            astController.Phase = newPhase;
        }
    }



    // The PlayerCollisionManager already checks for collisions with asteroids
    // but we want the asteroid to be destroyed when the player crashes into it,
    // so we deal with it here.
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            Kill();
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            ScoreController sc = (ScoreControllerAsteroid)GetComponent<ScoreController>();
            sc.ScorePoints();
            Kill();
        }
    }
}
