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
    [SerializeField] private AK.Wwise.Event neutralEvent; // Name of the Wwise event to trigger  
    [SerializeField] private float intervalBetweenSequences = 10f; // Time between each sequence  
    [SerializeField] private float minDelayBetweenEvents = 0.1f; // Minimum delay between events in a sequence  
    [SerializeField] private float maxDelayBetweenEvents = 0.5f; // Maximum delay between events in a sequence  

    private bool isBarking = false; // Indicates if an event is currently being posted  

    // Start is called once before the first execution of Update after the MonoBehaviour is created  
    void Start()
    {
        StartCoroutine(PostEventRandomly());
        StartCoroutine(TriggerEventSequence());
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
        isBarking = false;
    }
}
