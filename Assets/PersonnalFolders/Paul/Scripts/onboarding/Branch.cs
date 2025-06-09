using UnityEngine;

public class Branch : MonoBehaviour
{
    [Header("General")]
    public Transform sprite;
    public bool gone;
    private float intensityTarget;
    private float intensity;
    public float maxIntensity;

    [Header("Movement (slow interaction)")]
    
    public float movementMagnitude;
    [SerializeField] private  AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float distanceMoved;
    public float distanceToLeave;
    
    [Header("Wiggle (fast interaction)")]
    public float wiggleMagnitude;
    
    [SerializeField] private AnimationCurve wiggleCurve = AnimationCurve.Linear(0, 0, 1, 1);
    
    private float solidity;
    public float maxSolidity;
    
    
    

    

    
    void Start()
    {
        solidity = maxSolidity;
    }

    
    void Update()
    {
        //Manage Wiggle
        intensity = Mathf.Lerp(intensity, intensityTarget, Time.deltaTime);
        
        float wiggleIntensity = wiggleCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        float sine = Mathf.Sin(Time.time *30) * wiggleMagnitude * wiggleIntensity;
        sprite.localPosition = new Vector3(sine ,0,0);

        //Break if wiggled enough
        solidity -= wiggleIntensity * Time.deltaTime;
        if (solidity < 0)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            gone = true;
        }

        //Manage Movement
        float movementIntensity = movementCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        transform.position += transform.forward * movementIntensity * movementMagnitude * Time.deltaTime;

        //Leave if moved enough
        distanceMoved += movementIntensity * movementMagnitude * Time.deltaTime;
        if (distanceMoved > distanceToLeave)
        {
            transform.position += transform.forward * movementMagnitude * Time.deltaTime * 5;
            gone = true;
        }

        //Resets Intensity Target, so it gets back to zero if not hovered
        intensityTarget = 0;
        
    }

    public void Hover(float mouseIntensity)
    {
        intensityTarget = mouseIntensity;

    }
}
