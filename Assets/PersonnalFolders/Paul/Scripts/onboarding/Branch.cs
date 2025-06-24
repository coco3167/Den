using Audio;
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

    [Header("Break")]
    private Vector3 breakingPos;
    [SerializeField] private AnimationCurve breakCurve = AnimationCurve.Linear(0, -1, 1, 1);
    private float breakAnimState;
    public float breakAnimSpeed;
    public float breakAnimRange;

    [Header("Reset")]

    private Vector3 startPos;
    public Quaternion startRot;
    private Vector3 coverPos;

    private Rigidbody m_rigidbody;

    [Header("Audio States")]
    public bool wiggling;
    public bool moving;
    public bool isPlaying;








    void Start()
    {
        solidity = maxSolidity;
        startPos = transform.position;
        startRot = transform.rotation;
        coverPos = transform.position - transform.forward * 3;

        m_rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //Manage Wiggle
        intensity = Mathf.Lerp(intensity, intensityTarget, Time.deltaTime);
        float normalizedIntensity = Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity;
        CheckAudioState(intensity);

        float wiggleIntensity = wiggleCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        float sine = Mathf.Sin(Time.time * 30) / wiggleMagnitude * wiggleIntensity * Time.deltaTime;
        sprite.localPosition = new Vector3(sine, sprite.localPosition.y, sprite.localPosition.z);

        //Break if wiggled enough
        solidity -= wiggleIntensity * Time.deltaTime;
        solidity = Mathf.Clamp(solidity, 0, maxSolidity);
        if (solidity == 0)
        {
            // m_rigidbody.isKinematic = false;
            BreakAnim();
            gone = true;
            AudioManager.Instance.BranchBreaking.Post(GameManager.Instance.GetCamera().gameObject);
        }

        else
        {
            breakingPos = transform.localPosition;
            breakAnimState = 0;
        }

        //Manage Movement
            float movementIntensity = movementCurve.Evaluate(Mathf.Clamp(intensity, 0, maxIntensity) / maxIntensity);
        transform.position -= transform.forward * (movementIntensity * movementMagnitude * Time.deltaTime);

        //Leave if moved enough
        distanceMoved += movementIntensity * movementMagnitude * Time.deltaTime;
        if (solidity == 0)
            distanceMoved = 0;

        float localPosMagnitude = Mathf.Abs(transform.localPosition.x) + Mathf.Abs(transform.localPosition.y) + Mathf.Abs(transform.localPosition.z);
        if (distanceMoved > distanceToLeave && localPosMagnitude < 5)
        {
            transform.position -= transform.forward * (movementMagnitude * Time.deltaTime * 5);
            gone = true;
            AudioManager.Instance.BranchGone.Post(this.gameObject);
        }

        //Resets Intensity Target, so it gets back to zero if not hovered
        intensityTarget = 0;

        //branch moving and wiggling audio logic
        if (intensity > 0.1f)
        {
            if (!isPlaying)
            {
                AudioManager.Instance.BranchMoving.Post(this.gameObject);
                isPlaying = true;
            }
            AudioManager.Instance.SetRTPCValue(AudioManager.Instance.DEN_GP_TutoStep, this.gameObject, normalizedIntensity);
        }
        else
        {
            AudioManager.Instance.BranchStopMoving.Post(this.gameObject);
            isPlaying = false;
        }
    }

    public void Hover(float mouseIntensity)
    {
        intensityTarget = mouseIntensity;

    }

    public void BreakAnim()
    {
        breakAnimState += breakAnimSpeed * Time.deltaTime;
        transform.localPosition = new Vector3(breakingPos.x, breakingPos.y + breakCurve.Evaluate(breakAnimState) * breakAnimRange, breakingPos.z);
        sprite.transform.Rotate(Vector3.up, Time.deltaTime*30);
    }

    public void Reset()
    {
        // m_rigidbody.isKinematic = true;
        solidity = maxSolidity;
        distanceMoved = 0f;
        gone = false;

        transform.position = coverPos;
        transform.rotation = startRot;
        sprite.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

    }

    public void CoverAnimation(float animFactor)
    {
        transform.position = Vector3.Lerp(transform.position, startPos, animFactor);
    }

    public void CheckAudioState(float intensity)
    {
        //Debug.Log(intensity);
            wiggling = intensity >= .4 ? true : false;
            moving = intensity <= .4 && intensity > 0.01f ? true : false;
    }
}
