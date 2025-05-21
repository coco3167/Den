using UnityEngine;

public class BounceAnim : MonoBehaviour
{
    
    public AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float animLength;
    [Range(0,1f)]
    public float animCurrent;
    [Range(0,1f)]
    public float widthFactor;
    [Range(0,1f)]
    public float heightFactor;
    public Vector3 scaleFactor;
    public Vector3 startScale;
    public Collider trigger;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (animCurrent < animLength)
        {
            animCurrent += Time.deltaTime;
        }

        float animCurrentNormalized = animCurrent/animLength;
        scaleFactor = new Vector3(1 + animCurve.Evaluate(animCurrentNormalized)*widthFactor,1- animCurve.Evaluate(animCurrentNormalized)*heightFactor,1+ animCurve.Evaluate(animCurrentNormalized)*widthFactor);
        transform.localScale = Vector3.Scale(scaleFactor, startScale);


    }

    public void OnTriggerEnter(Collider other)
    {
        if (other == trigger)
        {
            DoSomething();
        }
    }   

    [ContextMenu("Do Something")]
    public void DoSomething()
    {
        animCurrent = 0f;
    }
}
