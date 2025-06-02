using UnityEngine;

public class ThresholdFX : MonoBehaviour
{
    public Material tresholdMat;
    public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve secondAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float animProgress = 0f;
    public float animSpeed = 1f;
    public bool play;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float curveValue = animCurve.Evaluate(animProgress);
        tresholdMat.SetFloat("_MAIN", curveValue);
        curveValue = secondAnimCurve.Evaluate(animProgress);
        tresholdMat.SetFloat("_SECOND", curveValue);

        if (animProgress < 1f)
        {
            animProgress += Time.deltaTime * animSpeed;
        }
        else
        {
            animProgress = 1f;
        }

        if (play)
        {
            animProgress = 0f;
            play = false;
        }
    }
}
