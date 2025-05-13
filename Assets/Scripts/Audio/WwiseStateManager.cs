using UnityEngine;

namespace Audio
{
    public static class WwiseStateManager
    {
        private static WwiseMoodState _currentMoodState = WwiseMoodState.NoneState;
        private static WwiseAudioState _currentAudioState = WwiseAudioState.None;
        public static void SetWwiseMoodState(
            WwiseMoodState stateMood)
        {
            if (stateMood == _currentMoodState)
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
            _currentMoodState = stateMood;
        }

        public static void SetWwiseAudioState(
            WwiseAudioState newAudioState)
        {
            if (newAudioState == _currentAudioState)
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
            _currentAudioState = newAudioState;
        }

        public static WwiseMoodState GetCurrentMoodState()
        {
            return _currentMoodState;
        }
    }
}
