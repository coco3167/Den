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
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float distanceMoved;
    public float distanceToLeave;

    [Header("Wiggle (fast interaction)")]
    public float wiggleMagnitude;

    [SerializeField] private AnimationCurve wiggleCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private float solidity;
    public float maxSolidity;

    [Header("Reset")]

    private Vector3 startPos;
    private Quaternion startRot;
    private Vector3 coverPos;

    private Rigidbody m_rigidbody;








    void Start()
    {
        solidity = maxSolidity;
        startPos = transform.position;
        startRot = transform.rotation;
        coverPos = transform.position - transform.forward*3;

        m_rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //Manage Wiggle
        intensity = Mathf.Lerp(intensity, intensityTarget, Time.deltaTime);

        float wiggleIntensity = wiggleCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        float sine = Mathf.Sin(Time.time * 30) / wiggleMagnitude * wiggleIntensity * Time.deltaTime;
        sprite.localPosition = new Vector3(sine, sprite.localPosition.y, sprite.localPosition.z);

        //Break if wiggled enough
        solidity -= wiggleIntensity*Time.deltaTime;
        if (solidity < 0)
        {
            m_rigidbody.isKinematic = false;
            gone = true;
        }

        //Manage Movement
        float movementIntensity = movementCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        transform.position -= transform.forward * (movementIntensity * movementMagnitude * Time.deltaTime);

        //Leave if moved enough
        distanceMoved += movementIntensity * movementMagnitude * Time.deltaTime;
        if (distanceMoved > distanceToLeave)
        {
            transform.position -= transform.forward * (movementMagnitude * Time.deltaTime * 5);
            gone = true;
        }

        //Resets Intensity Target, so it gets back to zero if not hovered
        intensityTarget = 0;

    }

    public void Hover(float mouseIntensity)
    {
        intensityTarget = mouseIntensity;

    }

    public void Reset()
    {
        m_rigidbody.isKinematic = true;
        solidity = maxSolidity;
        distanceMoved = 0f;
        gone = false;

        transform.position = coverPos;
        transform.rotation = startRot;
        
    }

    public void CoverAnimation(float animFactor)
    {
        transform.position = Vector3.Lerp(transform.position, startPos,animFactor);
    }
}
