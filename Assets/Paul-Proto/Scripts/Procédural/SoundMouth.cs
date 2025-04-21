using UnityEngine;

public class SoundMouth : MonoBehaviour
{
    public Transform mouthTarget;
    public float openAmount;
    public float sensitivity;

    public Vector3 startPos;

    // public AudioSource audioSource;
    public AudioSpectrum audioSpectrumScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = mouthTarget.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        mouthTarget.localPosition = new Vector3(startPos.x,startPos.y,openAmount*sensitivity);
        openAmount = audioSpectrumScript.meanLevels[0];
    }
}
