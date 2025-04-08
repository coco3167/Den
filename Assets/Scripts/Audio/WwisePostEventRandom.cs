using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WwisePostEventRandom : MonoBehaviour
{
    [Header("Random Mood Barks")]
    [SerializeField] private AK.Wwise.Event moodEvent;
    [SerializeField] private float MinInterval = 1f; // Minimum time interval between events  
    [SerializeField] private float MaxInterval = 5f; // Maximum time interval between events  

    [Header("Random Neutral Barks")]
    [SerializeField] private AK.Wwise.Event neutralEvent;
    [SerializeField] private float intervalBetweenSequences = 10f; // Time between each sequence  
    [SerializeField] private float minDelayBetweenEvents = 0.1f; // Minimum delay between events in a sequence  
    [SerializeField] private float maxDelayBetweenEvents = 0.5f; // Maximum delay between events in a sequence  

    [Header("Mood Steps Barks")]
    [SerializeField] private AK.Wwise.Event moodStepsEvent;

    [Header("Wwise Game Parameters")]
    [SerializeField] private AK.Wwise.RTPC Curiosity_Value;
    [SerializeField] private AK.Wwise.RTPC Fear_Value;
    [SerializeField] private AK.Wwise.RTPC Anger_Value;
    [SerializeField] private AK.Wwise.RTPC Intensity_Value;
    [SerializeField] private AK.Wwise.RTPC Tension_Value;

    [Range(0, 100)]
    public float currentCuriosityValue;
    [Range(0, 100)]
    public float currentFearValue;
    [Range(0, 100)]
    public float currentAngerValue;
    [Range(0, 100)]
    public float currentIntensityValue;
    [Range(0, 100)]
    public float currentTensionValue;

    private bool isBarking = false; // Indicates if an event is currently being posted  

    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        StartCoroutine(PostEventRandomly());
        StartCoroutine(TriggerEventSequence());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //SetWwiseMoodState(WwiseMoodState.CuriosityState);
            AudioManager.Instance.SetWwiseEmotionRTPC(Sinj.Emotions.Agression, this.gameObject, 0, ref Anger_Value, ref currentAngerValue);
            PostMoodStepEvent();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //SetWwiseMoodState(WwiseMoodState.FearState);
            AudioManager.Instance.SetWwiseEmotionRTPC(Sinj.Emotions.Agression, this.gameObject, 25, ref Anger_Value, ref currentAngerValue);
            PostMoodStepEvent();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //SetWwiseMoodState(WwiseMoodState.AngerState);
            AudioManager.Instance.SetWwiseEmotionRTPC(Sinj.Emotions.Agression, this.gameObject, 50, ref Anger_Value, ref currentAngerValue);
            PostMoodStepEvent();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //SetWwiseMoodState(WwiseMoodState.NeutralState);
            AudioManager.Instance.SetWwiseEmotionRTPC(Sinj.Emotions.Agression, this.gameObject, 75, ref Anger_Value, ref currentAngerValue);
            PostMoodStepEvent();
        }
    }
    // Coroutine to post the event at random intervals  
    private IEnumerator PostEventRandomly()
    {
        while (true)
        {
            float waitTime = Random.Range(MinInterval, MaxInterval);
            yield return new WaitForSeconds(waitTime);

            if (!isBarking && moodEvent != null)
            {
                isBarking = true;
                moodEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
            }
        }
    }

    private IEnumerator TriggerEventSequence()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalBetweenSequences);

            if (!isBarking)
            {
                isBarking = true;
                for (int i = 0; i < 5; i++) // Trigger event 5 times in a row  
                {
                    neutralEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
                    float delay = Random.Range(minDelayBetweenEvents, maxDelayBetweenEvents);
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }

    // Callback to reset isBarking when the event ends  
    private void OnEventEnd(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
    {
        if (in_type == AkCallbackType.AK_EndOfEvent)
        {
            isBarking = false;
        }
    }
    public void PostMoodStepEvent()
    {
        if (!isBarking)
        {
            isBarking = true;
            moodStepsEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
        }
    }
}
