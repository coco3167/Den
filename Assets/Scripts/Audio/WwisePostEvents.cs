using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WwisePostEvents : MonoBehaviour
{
    [Header("Random Mood Barks")]
    [SerializeField] private AK.Wwise.Event randomMoodEvent;

    [Header("Random Neutral Bark Sequence")]
    [SerializeField] private AK.Wwise.Event neutralEvent;
    [SerializeField] private float intervalBetweenSequences = 10f; // Time between each sequence  
    [SerializeField] private float minDelayBetweenEvents = 0.1f; // Minimum delay between events in a sequence  
    [SerializeField] private float maxDelayBetweenEvents = 0.5f; // Maximum delay between events in a sequence  

    [Header("Mood Steps Barks")]
    [SerializeField] private AK.Wwise.Event angerStepsEvent;
    [SerializeField] private AK.Wwise.Event curiousStepsEvent;
    [SerializeField] private AK.Wwise.Event fearStepsEvent;


    private bool isBarking = false; // Indicates if an event is currently being posted  

    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        PostRandomMoodEvent();
        StartCoroutine(TriggerOKBarkSequence());
    }

    void Update()
    {
        int value = -1;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            value = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            value = 25;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            value = 50;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            value = 75;

        if (value == -1)
            return;

        AudioManager.Instance.SetWwiseEmotionRTPC(Sinj.Emotions.Agression, gameObject, value);
        PostMoodStepEvent();
    }

    // Posts the mood event
    private void PostRandomMoodEvent()
    {
        if (!isBarking)
        {
            isBarking = true;
            randomMoodEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
        }
    }

    private IEnumerator TriggerOKBarkSequence()
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
            angerStepsEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
            curiousStepsEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
            fearStepsEvent.Post(this.gameObject, (uint)AkCallbackType.AK_EndOfEvent, OnEventEnd);
        }
    }
}
