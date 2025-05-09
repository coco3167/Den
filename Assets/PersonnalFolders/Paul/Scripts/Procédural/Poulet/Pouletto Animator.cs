using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PoulettoAnimator : MonoBehaviour
{
    public bool walking;
    public bool running;

    public Animator animator; 
    public LayerMask groundMask;
    public Transform[] feet;
    public TwoBoneIKConstraint[] feetIK;

    public float feetAdjustSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Walking", walking);
        animator.SetBool("Running", running);

        int index = 0;
        foreach (Transform foot in feet)
        {
            
            feetIK[index].weight = Mathf.Lerp(feetIK[index].weight,SetIK(foot),feetAdjustSpeed);
            // feetIK[index].weight = SetIK(foot);
            index ++;
        }

        
    }

    float SetIK(Transform footBone)
    {
        Vector3 origin = footBone.position + Vector3.up * 5f;
        Ray ray = new Ray(origin, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 5f, groundMask))
        {
            float distanceToGround = footBone.position.y - hit.point.y;
            Debug.Log(distanceToGround);
            return Mathf.Clamp(Mathf.Abs(distanceToGround),0,1);
            

            
        }
        else
        {
            // No ground detected (air) — let animation take over
            return 0f;
        }
    }
}
