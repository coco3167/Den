using AYellowpaper.SerializedCollections;
using UnityEngine;
using AK.Wwise;

public static class WwiseSwitchManager
{
    public static void SetWwiseSwitch<TSwitch>(TSwitch switchState, GameObject targetObject, SerializedDictionary<TSwitch, Switch> moodSwitches, ref TSwitch currentSwitch, Sinj.Emotions emotion)
        where TSwitch : System.Enum
    {
        if (switchState.Equals(currentSwitch))
        {
            Debug.Log($"{typeof(TSwitch).Name} switch is already set to {switchState}");
            return;
        }

        if (!moodSwitches.ContainsKey(switchState))
        {
            Debug.LogError($"Switch state {switchState} not found in mood switches.");
            return;
        }

        // Set the switch value on the target object
        moodSwitches[switchState].SetValue(targetObject);

        // Calculate the RTPC value
        int amount = System.Convert.ToInt32(switchState) * 25;

        // Set the RTPC value using the AudioManager
        AudioManager.Instance.SetWwiseEmotionRTPC(emotion, targetObject, amount);

        Debug.Log($"{typeof(TSwitch).Name} switch has been set to {switchState}");
        currentSwitch = switchState;
    }
}
