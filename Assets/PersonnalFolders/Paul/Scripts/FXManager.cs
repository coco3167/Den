using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FXManager : MonoBehaviour
{
    public Volume volume;
    private Vignette vignette;
    public Material fogMat;
    

    [Range(0, 1)]
    public float vignetteIntensity;
    [Range(0, 1)]
    public float fogAlpha;

    [Range(0, 1)]
    public float mainFactor;
    public bool manual;
    public Color FXColor;

    public bool FXCall;
    public float FXTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (volume.profile.TryGet(out vignette))
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
        vignette.intensity.Override(vignetteIntensity*mainFactor);
        vignette.color.Override(FXColor);
        
        
        FXColor.a = fogAlpha*mainFactor;
        fogMat.color = FXColor;


        if (FXCall)
        {
            FXTimer = 3;
            FXCall = false;
        }

        if (FXTimer > 0)
        {
            FXTimer -= Time.deltaTime;
            mainFactor =(Mathf.Sin(Time.time * 4f) * FXTimer/6f)+0.5f;
        }

        else if(!manual)
        {
            if (mainFactor > 0)
            {
                mainFactor -= Time.deltaTime;
            }
        }

        mainFactor = Mathf.Clamp(mainFactor,0,1);

        
    }
}
