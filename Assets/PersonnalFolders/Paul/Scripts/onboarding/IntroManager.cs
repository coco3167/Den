using Sinj;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class IntroManager : MonoBehaviour, IReloadable
{
    [Header("General")]
    [Range(1, 5)]
    public int step;
    private string[] stepNames = { "black screen", "fade", "onboarding", "title pause", "game" };

    /*
    step 1 = black screen
    step 2 = fade
    step 3 = onboarding
    step 4 = title pause
    step 5 = game (blur fading)
    */
    public string currentStep;
    

    [Header("Scripts")]
    public BranchesManager branchesManager;
    public MouseManager mouseManager;
    public SinjManager sinjManager;

    [Header("Black Background")]

    public RawImage blackBackground;
    public float fadeSpeed;
    private float transitionFactor;
    public float mouseDistance;

    [Header("Camera Movement")]
    public bool cameraUp;
    public Camera mainCamera;
    public Transform[] cameraSpots;
    public float cameraMoveSpeed;

    [Header("Transition to Game")]

    public float titleDelay = 3;
    public GameObject uiCursor;

    [Header("Depth of Field (WIP)")]
    public Volume dofVolume;
    public float dofSpeed;

    [Header("Audio")]
    public float mouseVelocity;
    public float branchesLeft;
    public bool coverAnimation;
    public bool titleReveal;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color color = blackBackground.color;
        color.a = 1;
        blackBackground.color = color;

        // mainCamera.transform.position = cameraSpots[0].position;

        
    }

    // Update is called once per frame
    void Update()
    {
        currentStep = stepNames[step - 1];

        mouseVelocity = mouseManager.MouseVelocity();
        mouseDistance += mouseVelocity;


        Cursor.lockState = step == 5 ? CursorLockMode.Locked : CursorLockMode.None;
        uiCursor.SetActive(step == 5);

        branchesManager.transform.position = mainCamera.transform.position;
        branchesManager.transform.rotation = mainCamera.transform.rotation;

        switch (step)
        {
            case 1:
                if (mouseDistance > 500)
                {
                    step = 2;
                }
                break;
            
            case 2:
                transitionFactor = Mathf.Lerp(transitionFactor, 1, Time.deltaTime * fadeSpeed);
                if (transitionFactor > 0.8f)
                {
                    transitionFactor = 1;
                    step = 3;
                }

                Color color = blackBackground.color;
                color.a = 1 - transitionFactor;

                blackBackground.color = color;

                foreach (Branch branchScript in branchesManager.branches)
                {
                    if (branchScript.gone)
                    {
                        branchScript.Reset();
                    }

                    else
                    {
                        branchScript.CoverAnimation(transitionFactor);
                    }
                }
                break;
            
            case 3:
                coverAnimation = false;
                blackBackground.gameObject.SetActive(false);
                transitionFactor = 0f;

                branchesLeft = branchesManager.branches.FindAll(x => !x.gone).Count;

                if (branchesLeft == 0f)
                {
                    step = 4;
                }
                break;
            
            case 4:
                titleReveal = true;
                titleDelay -= Time.deltaTime;
                if (titleDelay < 0)
                {
                    step = 5;
                }
                break;
            
            case 5:
                titleReveal = false;
                if (!cameraUp)
                {
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraSpots[1].position, cameraMoveSpeed * Time.deltaTime);
                    mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraSpots[1].rotation, cameraMoveSpeed * Time.deltaTime);
                }
                sinjManager.InfluencedByMouse(true);
                mouseManager.IsUsed = true;
                dofVolume.weight = Mathf.Lerp(dofVolume.weight, 0, Time.deltaTime * dofSpeed);

                // Déclencher OnGameReady via anim ?
                break;
        }

        if (step != 5)
        {
            if (cameraUp)
            {
                mainCamera.transform.position = cameraSpots[0].position;
                mainCamera.transform.rotation = cameraSpots[0].rotation;
            }

            dofVolume.weight = Mathf.Lerp(dofVolume.weight, 1, Time.deltaTime * dofSpeed);
        }

        
    }

    public void LoopReset()
    {
        step = 2;
        coverAnimation = true;
        sinjManager.InfluencedByMouse(false);
        mouseManager.IsUsed = false;
    }
    
    public void Reload()
    {
        LoopReset();
    }
}
