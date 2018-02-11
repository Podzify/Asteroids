﻿using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This MonoBehaviour instantiates copies of the gameobject its attached to.
/// When first instantiated (outside of this script), the SourcePrefab
/// property must be set by the instantiating script. This is because it is
/// impossible to have a prefab hold a reference to itself (it holds a 
/// reference to the instance instead, which makes cloning impossible).
/// (cf. https://forum.unity.com/threads/reference-to-prefab-changing-to-clone-self-reference.57312/)
/// 
/// Example (code in instantiating script):
///     CloneWhenKilled asteroid = Instantiate(asteroidPrefab, Vector2.zero, Quaternion.identity);
///     asteroid.SourcePrefab = asteroidPrefab;
/// 
/// Inspector options:
/// - Number of clones to create
/// - Maximum number of generations
/// - Optional scaling factor (make the clones larger or smaller)
/// 
/// This component implements the IKillable interface so that Kill()
/// can be called from anywhere.
/// </summary>

public class CloneWhenKilled : MonoBehaviour, IKillable {

    //
    // Inspector fields
    //
    public int numberOfClones;
    public int generationsMax;
    public float scalingFactor;


    //
    // Private fields 
    //
    private float newScale;
    private int _generation;

    //
    // Properties
    //
    public int Generation {
        get { return _generation; }
        private set
        {
            _generation = value;
            gameObject.name = "Asteroid (Generation " + Generation + ")";
        }
    }

    public CloneWhenKilled SourcePrefab { get; set; }

    //
    // Initialisation
    // Set Generation to zero (used in the property for naming and parenting).
    //
    private void Awake()
    {
        Generation = 0;
    }

    // FIXME this is weird
    private void Start()
    {
        if (Generation == 0)
        {
            transform.position = new Vector2(Random.Range(-15f, 15f), Random.Range(3f, 6f));
        }
    }

    //
    // Implementation of IKillable.
    // Instantiate as many clones as needed.
    //
    public void Kill()
    {
        if (Generation < generationsMax)
        {
            for (int i = 0; i < numberOfClones; i++)
            {
                CloneWhenKilled clone = Instantiate(SourcePrefab, Vector2.zero, Quaternion.identity);
                clone.SourcePrefab = SourcePrefab;
                clone.gameObject.transform.position = transform.position;
                clone.gameObject.transform.SetParent(transform.parent);
                clone.Generation = Generation + 1;
                newScale = Mathf.Pow(scalingFactor, Generation + 1);
                clone.transform.localScale = new Vector3(newScale, newScale, 1);
            }
        }
    }
}
