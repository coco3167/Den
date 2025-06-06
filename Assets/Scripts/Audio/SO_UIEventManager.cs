using UnityEngine;
using Audio;

[CreateAssetMenu(fileName = "SO_UIEventManager", menuName = "Scriptable Objects/SO_UIEventManager")]
public class SO_UIEventManager : ScriptableObject
{
    public static void PlayFaderTick()
    {
        AudioManager.Instance.FaderTick.Post(GameManager.Instance.GetCamera().gameObject);
    }
    public static void PlayBox()
    {
        AudioManager.Instance.Box.Post(GameManager.Instance.GetCamera().gameObject);
    }

    public static void PlayMasterTest()
    {
        AudioManager.Instance.MasterTest.Post(GameManager.Instance.GetCamera().gameObject);
    }
    public static void PlayMusicTest()
    {
        AudioManager.Instance.MusicTest.Post(GameManager.Instance.GetCamera().gameObject);
    }
    public static void PlaySFXTest()
    {
        AudioManager.Instance.SFXTest.Post(GameManager.Instance.GetCamera().gameObject);
    }
    public static void PlayAmbienceTest()
    {
        AudioManager.Instance.AmbienceTest.Post(GameManager.Instance.GetCamera().gameObject);
    }
}
