using Audio;
using System;
using UnityEngine;



public class TutoAudioManager : MonoBehaviour
{
    private Boolean isMoving = false;
    private Boolean isShaking = false;
    public void ChangeState(Boolean value)
    {
        if (isMoving.Equals(value))
        {
            //AudioManager.Instance.BranchMoving.Post(gameObject);
        }
        else if (isShaking.Equals(value))
        {
            //AudioManager.Instance.BranchShaking.Post(gameObject);
        }
        else
        {
            //AudioManager.Instance.BranchIdle.Post(gameObject);
        }
    }

    public void DeterminePhaseAndPost()
    {
        // This method is called when the player changes phase in the tutorial
        // It can be used to trigger specific audio events or states
        // For example, you might want to play a sound effect or change background music
    }


}
