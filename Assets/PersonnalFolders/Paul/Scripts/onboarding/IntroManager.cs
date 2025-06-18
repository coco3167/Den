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
    private string[] stepNames = { "black screen", "fade", "onboarding", "title", "game" };
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
    public Vector2 dofRange;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Color color = blackBackground.color;
        color.a = 1;
        blackBackground.color = color;

        // mainCamera.transform.position = cameraSpots[0].position;

        SetDepthOfField(dofRange.x);
        
        sinjManager.InfluencedByMouse(false);
        mouseManager.IsUsed = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetDepthOfField(dofRange.y);

        currentStep = stepNames[step - 1];

        mouseDistance += mouseManager.MouseVelocity();


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
                blackBackground.gameObject.SetActive(false);
                transitionFactor = 0f;

                int leftBranches = 0;
                foreach (Branch branchScript in branchesManager.branches)
                {
                    if (!branchScript.gone)
                    {
                        leftBranches++;
                    }
                }

                if (leftBranches == 0f)
                {
                    step = 4;
                }
                break;
            
            case 4:
                titleDelay -= Time.deltaTime;
                if (titleDelay < 0)
                {
                    step = 5;
                }
                break;
            
            case 5:
                if (!cameraUp)
                {
                    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraSpots[1].position, cameraMoveSpeed * Time.deltaTime);
                    mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraSpots[1].rotation, cameraMoveSpeed * Time.deltaTime);
                }
                sinjManager.InfluencedByMouse(true);
                mouseManager.IsUsed = true;
                // Déclencher OnGameReady via anim ?
                break;
        }

        if (step != 5 && cameraUp)
        {
            mainCamera.transform.position = cameraSpots[0].position;
            mainCamera.transform.rotation = cameraSpots[0].rotation;
        }

        /*if (step == 1 && mouseDistance > 500)
        {
            step = 2;
        }

        if (step == 2)
        {
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
        }

        if (step == 3)
        {
            blackBackground.gameObject.SetActive(false);
            transitionFactor = 0f;

            int leftBranches = 0;
            foreach (Branch branchScript in branchesManager.branches)
            {
                if (!branchScript.gone)
                {
                    leftBranches++;
                }
            }

            if (leftBranches == 0f)
            {
                step = 4;
            }
        }

        if (step == 4)
        {
            titleDelay -= Time.deltaTime;
            if (titleDelay < 0)
            {
                step = 5;
            }

            
        }

        if (step == 5 || !cameraUp)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraSpots[1].position, cameraMoveSpeed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraSpots[1].rotation, cameraMoveSpeed * Time.deltaTime);
            
        }

        else
        {
            mainCamera.transform.position = cameraSpots[0].position;
            mainCamera.transform.rotation = cameraSpots[0].rotation;
        }*/
    }

    public void LoopReset()
    {
        step = 2;
        sinjManager.InfluencedByMouse(false);
        mouseManager.IsUsed = false;
    }

    private void SetDepthOfField(float value)
    {
        var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        var volumeProfile = urpAsset?.volumeProfile;
        
        if (volumeProfile == null)
        {
            Debug.LogError("No default Volume Profile found in URP Asset.");
            return;
        }

        if (volumeProfile.TryGet<DepthOfField>(out var dof))
        {
            dof.focusDistance.value = value;

        }
    }

    public void Reload()
    {
        LoopReset();
    }
}
