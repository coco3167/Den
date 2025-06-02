using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Proc_Look : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RigBuilder rigBuilder;

    public MultiAimConstraint aimConstraint; // Assign the constraint itself in the Inspector
    public string sourceObjectName = "MouseAura"; // Name of object to find and add
    public float weight = 1f;

    void Start()
    {
        // Try to find the source object by name
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

        rigBuilder.Build();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
