using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.Events;
using System;


public class Proc_Look : MonoBehaviour /*, IGameStateListener*/
{



    public Transform mouseTarget;
    public Transform socialTarget;
    public Transform destinationTarget;
    public Transform mouseAura;
    public GameObject[] heads;
    public GameObject selfHead;
    public NavMeshAgent agent;
    public MultiAimConstraint[] aimConstraints;

    public Vector3 scores;
    public float mouseRange = 10f;
    public float socialRange = 5f;

    // Assign the constraint itself in the Inspector

    // public void OnGameReady(object sender, EventArgs eventArgs)
    // {

    // }

    // public void OnGameEnded(object sender, EventArgs eventArgs)
    // {

    // }

    // Update is called once per frame

    void Start()
    {

    }
    void Update()
    {
        if (mouseAura == null)
        {
            mouseAura = GameObject.Find("MouseAura").transform;
            heads = GameObject.FindGameObjectsWithTag("Head");
        }

        mouseTarget.position = mouseAura.position;

        Vector3 nearestHead = new Vector3(999, 999, 999);

        foreach (GameObject head in heads)
        {
            if (head != selfHead)
            {
                if (Vector3.Distance(head.transform.position, selfHead.transform.position) < Vector3.Distance(head.transform.position, nearestHead))
                {
                    nearestHead = head.transform.position;
                }
            }
        }

        socialTarget.position = nearestHead;
        destinationTarget.position = agent.destination;



        //calculate Aim Score
        scores.x = Mathf.Clamp(mouseRange - Vector3.Distance(mouseAura.position, selfHead.transform.position), 0, 999);
        scores.y = Mathf.Clamp(socialRange - Vector3.Distance(nearestHead, selfHead.transform.position), 0, 999);
        scores.z = Mathf.Clamp(Vector3.Distance(agent.transform.position, destinationTarget.position),0,1);

        float sum = scores.x + scores.y + scores.z;
        if (sum > 1)
        {
            scores /= sum;
        }
         
        UpdateConstraints(scores);
    }

    void UpdateConstraints(Vector3 newRatio)
    {
        Debug.Log(newRatio);

        
        foreach (MultiAimConstraint aimConstraint in aimConstraints)
        {
            WeightedTransformArray sources = aimConstraint.data.sourceObjects;

            sources.SetWeight(0, newRatio.x);
            sources.SetWeight(1, newRatio.y);
            sources.SetWeight(2, newRatio.z);
            
            aimConstraint.data.sourceObjects = sources;
        }
    }
}
