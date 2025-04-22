using UnityEngine;

namespace Audio
{
    public static class WwiseSwitchManager
    {
        private static WwiseReactionMoodSwitch _currentReactionMoodSwitch = WwiseReactionMoodSwitch.None;

        public static void SetWwiseSwitch(WwiseReactionMoodSwitch switchReactionMood, GameObject targetGameObject)
        {
            if (switchReactionMood == _currentReactionMoodSwitch)
            {
                Debug.Log("Mood is already set to " + switchReactionMood);
                return;
            }

            if (!AudioManager.Instance.switchReactionMood.ContainsKey(switchReactionMood))
            {
                Debug.LogError($"Mood state {switchReactionMood} not found in the dictionary.");
                return;
            }

            AudioManager.Instance.switchReactionMood[switchReactionMood].SetValue(targetGameObject);

            Debug.Log("Mood has been set to " + switchReactionMood);
            _currentReactionMoodSwitch = switchReactionMood;
        }
    }
}
