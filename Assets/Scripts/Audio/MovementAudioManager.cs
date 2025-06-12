using Audio;
using UnityEngine;

public class MovementAudioManager : MonoBehaviour
{
    public void PlayFootstepWalk()
    {
        AudioManager.Instance.FootstepWalk.Post(this.gameObject);
    }

    public void PlayFootstepRun()
    {
        AudioManager.Instance.FootstepRun.Post(this.gameObject);
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
}
