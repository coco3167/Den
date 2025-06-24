using Audio;
using UnityEngine;

public class MovementAudioManager : MonoBehaviour
{
    private GameObject _wwiseEmitter;

    private void Awake()
    {
        // Cache the GameObject that actually has the AkGameObj
        var ak = GetComponentInChildren<AkGameObj>();
        if (ak == null)
            Debug.LogError($"[{name}] No AkGameObj found in children!");
        else
            _wwiseEmitter = ak.gameObject;
    }
    public void PlayFootstepWalk()
    {
        // Post on the emitter (or fallback to root if something’s wrong)
        var target = _wwiseEmitter ?? this.gameObject;
        AudioManager.Instance.FootstepWalk.Post(target);
    }

    public void PlayFootstepRun()
    {
        var target = _wwiseEmitter ?? this.gameObject;
        AudioManager.Instance.FootstepRun.Post(target);
    }

    public void PlaySit()
    {
        AudioManager.Instance.Sit.Post(this.gameObject);
    }

    public void PlayStand()
    {
        AudioManager.Instance.Stand.Post(this.gameObject);
    }

    public void PlayEat()
    {
        AudioManager.Instance.Eat.Post(this.gameObject);
    }

    public void PlayTube()
    {
        AudioManager.Instance.Tube.Post(this.gameObject);
    }

    public void PlayScratch()
    {
        AudioManager.Instance.Scratch.Post(this.gameObject);
    }

    public void PlayMoodBark()
    {
        AudioManager.Instance.MoodBark.Post(this.gameObject);
    }

    public void PlayIdle()
    {
        AudioManager.Instance.Idle.Post(this.gameObject);
    }

    public void PlayReacAnger()
    {
        AudioManager.Instance.ReacAnger.Post(this.gameObject);
    }
    public void PlayReacFear()
    {
        AudioManager.Instance.ReacFear.Post(this.gameObject);
    }
    public void PlayReacCurious()
    {
        AudioManager.Instance.ReacCurious.Post(this.gameObject);
    }
    public void PlaySleepFlower()
    {
        AudioManager.Instance.SleepFlower.Post(this.gameObject);
    }
    public void PlayStingerScream()
    {
        AudioManager.Instance.StingerScream.Post(this.gameObject);
    }

    public void PlayGroomed()
    {
        AudioManager.Instance.Groomed.Post(this.gameObject);
    }
    public void PlayGroomer()
    {
        AudioManager.Instance.Groomer.Post(this.gameObject);
    }
    public void PlayScream()
    {
        AudioManager.Instance.Scream.Post(this.gameObject);
    }
}
