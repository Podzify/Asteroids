﻿using UnityEngine;

/// <summary>
/// This MonoBehaviour applies a user-definable variation
/// to the gameobject's initial position and rotation,
/// and optionally applies a random force and torque
/// to the rigidbody (if one is attached).
/// </summary>

public class RandomizeInitialMotion : MonoBehaviour {

    //
    // Inspector fields
    //
    [Header("Position variations when instantiated")]
    public bool randomizeTransform = true;
    public float delta = 0.5f;

    [Header("Forces applied when spawning")]
    public bool addRandomForce = true;
    public float forceDelta;
    public bool addRandomTorque = true;
    public float torqueDelta;

    [Header("Move faster at each level")]
    public float levelMultiplier = 0.5f;

    //
    // Private fields
    //
    private Rigidbody2D rb;
    private Vector3 currentPos;

    //
    // Initialisation
    //
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //
    // Set transform, apply forces.
    //
	private void Start () {

        if (randomizeTransform)
        {
            // Add position and rotation variations
            currentPos = transform.position;
            float posOffset = Random.Range(-delta, delta);
            float rot = Random.Range(0f, 1f);

            // Set the transform position to the random delta value
            transform.position = new Vector2(currentPos.x + posOffset, currentPos.y + posOffset);

            // Rotate the transform by the random rot value around the z axis
            transform.Rotate(new Vector3(0, 0, rot));
        }

        // Apply random force and torque
        if (addRandomForce && rb != null)
        {
            float dirX = Random.Range(-torqueDelta, torqueDelta);
            float dirY = Random.Range(-torqueDelta, torqueDelta);
            Vector2 randomVector = new Vector2(dirX, dirY) *
                (2 + GameManager.CurrentLevel * levelMultiplier) * rb.mass;
            rb.AddRelativeForce(randomVector, ForceMode2D.Impulse);
        }

        if (addRandomTorque && rb != null)
        {
            rb.AddTorque(Random.Range(-torqueDelta * rb.mass, torqueDelta * rb.mass), ForceMode2D.Impulse);
        }
	}
}