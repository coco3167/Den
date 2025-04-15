using AYellowpaper.SerializedCollections;
using UnityEngine;
using AK.Wwise;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class WwiseSwitchManager
{
    static WwiseReactionMoodSwitch currentReactionMoodSwitch = WwiseReactionMoodSwitch.None;

    public static void SetWwiseSwitch(WwiseReactionMoodSwitch switchReactionMood, GameObject targetGameObject)
    {
        if (switchReactionMood == currentReactionMoodSwitch)
        {
            UnityEngine.Debug.Log("Mood is already set to " + switchReactionMood);
            return;
        }

        if (!AudioManager.Instance.switchReactionMood.ContainsKey(switchReactionMood))
        {
            UnityEngine.Debug.LogError($"Mood state {switchReactionMood} not found in the dictionary.");
            return;
        }

        AudioManager.Instance.switchReactionMood[switchReactionMood].SetValue(targetGameObject);

        UnityEngine.Debug.Log("Mood has been set to " + switchReactionMood);
        currentReactionMoodSwitch = switchReactionMood;
    }
}
