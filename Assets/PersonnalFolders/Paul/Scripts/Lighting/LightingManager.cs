using UnityEngine;
using UnityEngine.Rendering;

public class LightingManager : MonoBehaviour
{
    [Header("Controller")]
    [Range (0,1)]
    public float daySlider;
    
    private float gradientStep;
    public string currentDayStep;

    [Header("Skybox materials")]
    public Material skyMat;
    // public LightingScriptable morning;

    public Material morningMat;
    public Material dayMat;
    public Material eveningMat;

    [Header("Sun Light")]
    public Light sunLight;
    public float startXRotation;
    public float endXRotation;
    private Vector3 originRotation;
    public Color morningSunCol;
    public Color daySunCol;
    public Color eveningSunCol;

    [Header("God Rays")]
    public Color morningRayCol;
    public Color dayRayCol;
    public Color eveningRayCol;
    public ParticleSystem[] godRays;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originRotation = sunLight.transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (daySlider < 0.25f)
        {
            gradientStep = daySlider*4;
            PickMatParam(eveningMat, morningMat);
            currentDayStep = "morning";
            
            SetFXCol(eveningRayCol, morningRayCol);
            sunLight.color = SetIntermediateCol(eveningSunCol, morningSunCol);
        }
        else if (daySlider < 0.75f)
        {
            gradientStep = (daySlider-0.25f)*2;
            PickMatParam(morningMat, dayMat);
            currentDayStep = "day";

            SetFXCol(morningRayCol, dayRayCol);
            sunLight.color = SetIntermediateCol(morningSunCol, daySunCol);
        }
        else
        {
            gradientStep = (daySlider-0.75f)*4;
            PickMatParam(dayMat, eveningMat);
            currentDayStep = "evening";
            
            SetFXCol(dayRayCol, eveningRayCol);
            sunLight.color = SetIntermediateCol(daySunCol, eveningSunCol);
        }

        sunLight.transform.eulerAngles = new Vector3(Mathf.Lerp(startXRotation,endXRotation,daySlider), originRotation.y, originRotation.z);

        
        
    }

    void PickMatParam(Material previousMat, Material nextMat)
    {
        SetMatIntermediate(previousMat,nextMat,"_Col1", true);
        SetMatIntermediate(previousMat,nextMat,"_Col2", true);
        SetMatIntermediate(previousMat,nextMat,"_Col3", true);
        SetMatIntermediate(previousMat,nextMat,"_SubCol", true);
        SetMatIntermediate(previousMat,nextMat,"_Step1", false);
        SetMatIntermediate(previousMat,nextMat,"_Step2", false);
        SetMatIntermediate(previousMat,nextMat,"_Step3", false);
    }
    
    void SetMatIntermediate (Material previousMat, Material nextMat, string variableReference, bool isColor)
    {
        if (isColor)
        {
            skyMat.SetColor(variableReference, SetIntermediateCol(previousMat.GetColor(variableReference),nextMat.GetColor(variableReference)));
        }
        else
        {
            skyMat.SetFloat(variableReference, SetIntermediateFloat(previousMat.GetFloat(variableReference),nextMat.GetFloat(variableReference)));
        }
    }

    void SetFXCol (Color previousCol, Color nextCol)
    {
        foreach (ParticleSystem godRay in godRays)
            {
                var fxCol = godRay.main;
                fxCol.startColor = SetIntermediateCol(previousCol, nextCol);
            }
    }

    Color SetIntermediateCol(Color previousCol, Color nextCol)
    {
        Color currentColor = Color.Lerp(previousCol, nextCol, gradientStep);
        return currentColor;
        
    }

    float SetIntermediateFloat(float previousFloat, float nextFloat)
    {
        float currentFloat = Mathf.Lerp(previousFloat, nextFloat, gradientStep);
        return currentFloat;
        
    }

    
}
