using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using System;


public class Proc_Look : MonoBehaviour, IGameStateListener
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RigBuilder rigBuilder;

    public MultiAimConstraint aimConstraint; // Assign the constraint itself in the Inspector
    public string sourceObjectName = "MouseAura"; // Name of object to find and add
    public float weight = 1f;
    public bool setupDone;

    public void OnGameReady(object sender, EventArgs eventArgs)
    {
        
    }

    public void OnGameEnded(object sender, EventArgs eventArgs)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!setupDone)
        {
            Transform source = GameObject.Find(sourceObjectName)?.transform;

            if (aimConstraint == null || source == null)
            {
                Debug.LogWarning("MultiAimConstraint or source object not found.");
                return;
            }

            // Copy current sources
            var sources = aimConstraint.data.sourceObjects;

            // Add new source
            sources.Add(new WeightedTransform(source, weight));

            // Assign back to the constraint
            aimConstraint.data.sourceObjects = sources;

            // Refresh the constraint (force update)
            aimConstraint.weight = aimConstraint.weight;



            Debug.Log("built");

            setupDone = true;
        }
        
        rigBuilder.Build();
    }
}
