using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WwisePostEvents : MonoBehaviour
{
    public static WwisePostEvents instance;


    [Header("Random Mood Barks")]
    [SerializeField] private AK.Wwise.Event randomMoodEvent;
    private bool canTriggerEvent = true;
    [SerializeField] private float eventCooldown = 3f;

    [Header("Random Neutral Bark Sequence")]
    [SerializeField] private AK.Wwise.Event okEvent;
    [SerializeField] private float intervalBetweenSequences = 10f; // Time between each sequence  
    [SerializeField] private float minDelayBetweenEvents = 0.1f; // Minimum delay between events in a sequence  
    [SerializeField] private float maxDelayBetweenEvents = 0.5f; // Maximum delay between events in a sequence  

    [Header("Mood Steps Barks")]
    [SerializeField] private AK.Wwise.Event angerStepsEvent;
    [SerializeField] private AK.Wwise.Event curiousStepsEvent;
    [SerializeField] private AK.Wwise.Event fearStepsEvent;

    [Header("Reaction Barks")]
    [SerializeField] private AK.Wwise.Event reactionMoodEvent;


    void Initialize()
    {
        //Singleton logic
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("Destroying duplicate AudioManager");
            Destroy(this);
        }
    }
    private void Awake()
    {
        Initialize();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        StartCoroutine(TriggerOKBarkSequence());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PostRandomMoodEvent();
        }
    }

    public void PostRandomMoodEvent()
    {
        if (!canTriggerEvent) return; // Prevent triggering if cooldown is active

        randomMoodEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);

        // Start cooldown
        StartCoroutine(RandomMoodCooldownCoroutine());
    }

    private IEnumerator RandomMoodCooldownCoroutine()
    {
        canTriggerEvent = false; // Disable event triggering
        yield return new WaitForSeconds(eventCooldown); // Wait for cooldown duration
        canTriggerEvent = true; // Re-enable event triggering
    }

    private IEnumerator TriggerOKBarkSequence()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalBetweenSequences);

            // Wait until the randomMoodEvent is not playing
            while (!canTriggerEvent)
            {
                yield return null; // Wait for the next frame
            }

            for (int i = 0; i < 5; i++) // Trigger event 5 times in a row  
            {
                okEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
                float delay = Random.Range(minDelayBetweenEvents, maxDelayBetweenEvents);
                yield return new WaitForSeconds(delay);
            }
        }
    }

    // Callback to reset isBarking when the event ends  
    private void OnEventEnd(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        // No longer resetting isBarking as it has been removed
    }

    public void PostMoodStepEvent(AK.Wwise.Event moodEvent)
    {
        moodEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
    }

    public void PostReactionMoodEvent(WwiseReactionMoodSwitch moodSwitch)
    {
        StopAllEvents();
        WwiseSwitchManager.SetWwiseSwitch(moodSwitch, this.gameObject);
        reactionMoodEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
    }

    public void StopAllEvents()
    {
        // Stop all events on this GameObject
        randomMoodEvent.Stop(this.gameObject);
        okEvent.Stop(this.gameObject);
        angerStepsEvent.Stop(this.gameObject);
        curiousStepsEvent.Stop(this.gameObject);
        fearStepsEvent.Stop(this.gameObject);
        reactionMoodEvent.Stop(this.gameObject);
    }
}
