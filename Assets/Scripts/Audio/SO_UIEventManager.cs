using UnityEngine;
using Audio;

[CreateAssetMenu(fileName = "SO_UIEventManager", menuName = "Scriptable Objects/SO_UIEventManager")]
public class SO_UIEventManager : ScriptableObject
{
    public static void PlayFaderTick()
    {
        AudioManager.Instance.FaderTick.Post(Camera.main.gameObject);
    }
    public static void PlayBox()
    {
        AudioManager.Instance.Box.Post(Camera.main.gameObject);
    }
}
