using UnityEngine;

public class Proc_Ears : MonoBehaviour
{
    public Transform earTarget;
    public Transform earTarget_L;
    public Transform earTarget_R;
    public Transform earHint;
    public Transform earHint_L;
    public Transform earHint_R;

    public float targetsGap;
    public float hintsGap;
    public bool up = true;
    public Vector3 upTargetPos;
    public Vector3 upHintPos;
    public Vector3 downTargetPos;
    public Vector3 downHintPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (up)
        {
            earTarget.localPosition = upTargetPos;
            earHint.localPosition = upHintPos;
            targetsGap = 1.897f;
            hintsGap = 1.34f;
        }

        else
        {
            earTarget.localPosition = downTargetPos;
            earHint.localPosition = downHintPos;
            targetsGap = 0.74f;
            hintsGap = 1.74f;
        }
        
        
        EarLerp(earTarget_L,earTarget.localPosition, targetsGap, true);
        EarLerp(earTarget_R,earTarget.localPosition, targetsGap, false);
        EarLerp(earHint_L, earHint.localPosition, hintsGap, true);
        EarLerp(earHint_R, earHint.localPosition, hintsGap, false);
    }

    void EarLerp(Transform movingTransform, Vector3 posToGo, float gap, bool left)
    {
        float multiplier = -1;
        if (left)
        {
            multiplier = 1;
        }

       
        movingTransform.localPosition = Vector3.Lerp(movingTransform.localPosition,new Vector3(gap*multiplier,posToGo.y,posToGo.z),Time.deltaTime);
    }
}
