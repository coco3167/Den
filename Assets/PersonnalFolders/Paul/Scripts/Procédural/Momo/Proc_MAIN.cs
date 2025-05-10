using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Proc_MAIN : MonoBehaviour
{
    [Range (0,1)]
    public float getUpAmount;
    public MultiPositionConstraint rootMultiPos;
    public TwoBoneIKConstraint neckIK; 
    
    
    [Range (0,1)]
    public float cryAmount;
    public Transform mouthOpener;
    public float mouthMaxOpening;
    private Quaternion startMouthOpenerRotation;

    public Transform torsoInflater;
    public float torsoMaxInflation;
    private Vector3 startTorsoInflaterPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startMouthOpenerRotation = mouthOpener.rotation;
        startTorsoInflaterPosition = torsoInflater.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rootMultiPos.weight = getUpAmount;
        neckIK.weight = getUpAmount;
        
        
        mouthOpener.rotation = startMouthOpenerRotation * Quaternion.Euler (0, mouthMaxOpening*cryAmount, 0);
        
        torsoInflater.localPosition = startTorsoInflaterPosition;
        torsoInflater.Translate(Vector3.down * torsoMaxInflation * cryAmount); 
    }
}
