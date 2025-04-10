using UnityEngine;
using AYellowpaper.SerializedCollections;
using AK.Wwise;


public static class WwiseStateManager
{
    public static void SetWwiseMoodState(
        WwiseMoodState stateMood,
        SerializedDictionary<WwiseMoodState, State> gameStateMoodVisualization,
        ref WwiseMoodState currentMoodState)
    {
        if (stateMood == currentMoodState)
        {
            Debug.Log("Mood is already set to " + stateMood);
            return;
        }

        if (!gameStateMoodVisualization.ContainsKey(stateMood))
        {
            Debug.LogError($"Mood state {stateMood} not found in the dictionary.");
            return;
        }

        gameStateMoodVisualization[stateMood].SetValue();

        Debug.Log("Mood has been set to " + stateMood);
        currentMoodState = stateMood;
    }

    public static void SetWwiseAudioState(
        WwiseAudioState newAudioState,
        SerializedDictionary<WwiseAudioState, State> audioState,
        ref WwiseAudioState currentAudioState)
    {
        if (newAudioState == currentAudioState)
        {
            Debug.Log("Audio state is already set to " + newAudioState);
            return;
        }

        if (!audioState.ContainsKey(newAudioState))
        {
            Debug.LogError($"Audio state {newAudioState} not found in the dictionary.");
            return;
        }

        audioState[newAudioState].SetValue();

        Debug.Log("Audio state has been set to " + newAudioState);
        currentAudioState = newAudioState;
    }
}
