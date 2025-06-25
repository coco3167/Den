using Audio;
using UnityEngine;

public class MovementAudioManager : MonoBehaviour
{
    private static int _globalIndex;
    
    private GameObject _wwiseEmitter;
    private int m_index;

    private void Awake()
    {
        // Cache the GameObject that actually has the AkGameObj
        var ak = GetComponentInChildren<AkGameObj>();
        if (ak == null)
            Debug.LogError($"[{name}] No AkGameObj found in children!");
        else
            _wwiseEmitter = ak.gameObject;

        m_index = _globalIndex;
        _globalIndex++;
    }
    public void PlayFootstepWalk()
    {
        // Post on the emitter (or fallback to root if something�s wrong)
        var target = _wwiseEmitter ?? this.gameObject;
        AudioManager.Instance.FootstepWalk.Post(target);
        CloseCaptionningManager.Instance.ShowCloseCaption("Footstep Walk", m_index);
    }

    public void PlayFootstepRun()
    {
        var target = _wwiseEmitter ?? this.gameObject;
        AudioManager.Instance.FootstepRun.Post(target);
        CloseCaptionningManager.Instance.ShowCloseCaption("Footstep Run", m_index);
    }

    public void PlaySit()
    {
        AudioManager.Instance.Sit.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Sitting sound", m_index);
    }

    public void PlayStand()
    {
        AudioManager.Instance.Stand.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Standing sound", m_index);
    }

    public void PlayEat()
    {
        AudioManager.Instance.Eat.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Eating sound", m_index);
    }

    public void PlayTube()
    {
        AudioManager.Instance.Tube.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Tube sound", m_index);
    }

    public void PlayScratch()
    {
        AudioManager.Instance.Scratch.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Scratch", m_index);
    }

    public void PlayMoodBark()
    {
        AudioManager.Instance.MoodBark.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Bark", m_index);
    }

    public void PlayIdle()
    {
        AudioManager.Instance.Idle.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Idle sound", m_index);
    }

    public void PlayReacAnger()
    {
        AudioManager.Instance.ReacAnger.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Agonisty reaction sound", m_index);
    }
    public void PlayReacFear()
    {
        AudioManager.Instance.ReacFear.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Fear reaction sound", m_index);
    }
    public void PlayReacCurious()
    {
        AudioManager.Instance.ReacCurious.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Curiosity reaction sound", m_index);
    }
    public void PlaySleepFlower()
    {
        AudioManager.Instance.SleepFlower.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Sleeping Sound", m_index);
    }
    public void PlayStingerScream()
    {
        AudioManager.Instance.StingerScream.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Scream", m_index);
    }

    public void PlayGroomed()
    {
        AudioManager.Instance.Groomed.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Groomed sound", m_index);
    }
    public void PlayGroomer()
    {
        AudioManager.Instance.Groomer.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Groomer sound", m_index);
    }
    public void PlayScream()
    {
        AudioManager.Instance.Scream.Post(this.gameObject);
        CloseCaptionningManager.Instance.ShowCloseCaption("Angry scream", m_index);
    }
}
