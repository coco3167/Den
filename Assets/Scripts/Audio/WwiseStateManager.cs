using UnityEngine;
using AYellowpaper.SerializedCollections;
using AK.Wwise;


public static class WwiseStateManager
{
    static WwiseMoodState currentMoodState = WwiseMoodState.NoneState;
    static WwiseAudioState currentAudioState = WwiseAudioState.None;
    public static void SetWwiseMoodState(
        WwiseMoodState stateMood)
    {
        if (stateMood == currentMoodState)
        {
            Debug.Log("Mood is already set to " + stateMood);
            return;
        }

        if (!AudioManager.Instance.gameStateMoodVisualization.ContainsKey(stateMood))
        {
            Debug.LogError($"Mood state {stateMood} not found in the dictionary.");
            return;
        }

        AudioManager.Instance.gameStateMoodVisualization[stateMood].SetValue();

        Debug.Log("Mood has been set to " + stateMood);
        currentMoodState = stateMood;
    }

    public static void SetWwiseAudioState(
        WwiseAudioState newAudioState)
    {
        if (newAudioState == currentAudioState)
        {
            Debug.Log("Audio state is already set to " + newAudioState);
            return;
        }

        if (!AudioManager.Instance.audioState.ContainsKey(newAudioState))
        {
            Debug.LogError($"Audio state {newAudioState} not found in the dictionary.");
            return;
        }

        AudioManager.Instance.audioState[newAudioState].SetValue();

        Debug.Log("Audio state has been set to " + newAudioState);
        currentAudioState = newAudioState;
    }
}
