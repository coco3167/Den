using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ClimbProc : MonoBehaviour
{
    public TwoBoneIKConstraint[] iks;
    public Transform[] extremities;
    public Transform[] targets;
    public Transform[] rayCasters;

    
    public Vector3[] positions;
    public Vector3[] nextPositions;

    public float[] stepLengths;
    public LayerMask groundMask;
    public float weight;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int index = 0;
        foreach (TwoBoneIKConstraint ik in iks)
        {
            extremities[index] = ik.data.tip;
            targets[index] = ik.data.target;
            rayCasters[index] = ik.gameObject.transform.GetChild(1);

            index ++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int index = 0;
        foreach (TwoBoneIKConstraint ik in iks)
        {
            StickToTree(extremities[index], targets[index], rayCasters[index], ik, stepLengths[index], index);
            
            index ++;
        }
        
    }

    void StickToTree(Transform extremity, Transform target, Transform rayCaster, TwoBoneIKConstraint ik, float stepLength, int loopIndex)
    {
        Vector3 origin = rayCaster.position;
        Ray ray = new Ray(origin, Vector3.forward * 10);
        Debug.DrawRay(origin, Vector3.forward * 10f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f, groundMask))
        {
            float distance = Vector3.Distance(hit.point, positions[loopIndex]);
            if (distance > stepLength)
            {
                target.position = hit.point;
                nextPositions[loopIndex] = hit.point + (hit.point - nextPositions[loopIndex]) * 0.75f;
                
            }
            else
            {
                target.position = positions[loopIndex];
            }

            positions[loopIndex] = Vector3.Lerp(positions[loopIndex], nextPositions[loopIndex], Time.deltaTime *20);
            weight = Mathf.Lerp(weight, 1, Time.deltaTime *3);
            

            
        }
        else
        {
            Debug.Log("noColl");
            weight = Mathf.Lerp(weight, 0, Time.deltaTime *3);
        }

        ik.weight = weight;
    }
}
