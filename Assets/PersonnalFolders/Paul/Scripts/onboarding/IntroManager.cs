using System.Linq;
using Audio;
using Sinj;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private SymbolManager symbolManager;
    
    [Header("General")]
    [Range(1, 7)]
    public int step;
    private string[] stepNames = { "black screen", "fade", "onboarding", "title pause", "title disappearing", "symbol appearing", "game" };

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
    public Material titleMat;
    public float titleFadeSpeed;
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
        titleMat.SetFloat("_MAIN", 0.5f);

        // mainCamera.transform.position = cameraSpots[0].position;
        AudioManager.Instance.InitializeForState(GameState.Blackscreen);
        AudioManager.Instance.StopCursorMoveSound(GameManager.Instance.GetMouseManager().GetMouseAura());
    }

    // Update is called once per frame
    void Update()
    {
        currentStep = stepNames[step - 1];

        mouseVelocity = mouseManager.MouseVelocity();
        mouseDistance += mouseVelocity;


        // Cursor.lockState = step == 5 ? CursorLockMode.Locked : CursorLockMode.None;
        // uiCursor.SetActive(step == 5);

        branchesManager.transform.position = mainCamera.transform.position;
        branchesManager.transform.rotation = mainCamera.transform.rotation;

        float titleMAIN;

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

                int total = branchesManager.branches.Count;
                int left = branchesManager.branches.Count(b => !b.gone);

                // this will lerp Neutral RTPC from 75→0
                // and tutorial RTPC from 0→100

                AudioManager.Instance.SetGameStateBranches(left, total);

                if (left == 0)
                    step = 4;
                break;

            case 4:
                titleReveal = true;

                AudioManager.Instance.SetGameStateTitleReveal();
                AudioManager.Instance.RestartAmbience();

                titleDelay -= Time.deltaTime;

                titleMAIN = Mathf.Lerp(titleMat.GetFloat("_MAIN"), 1, Time.deltaTime * titleFadeSpeed);
                titleMat.SetFloat("_MAIN", titleMAIN);

                if (titleDelay < 0)
                {
                    step = 5;
                }
                break;
            
            case 5:
                titleMAIN = Mathf.Lerp(titleMat.GetFloat("_MAIN"), 0, Time.deltaTime * titleFadeSpeed);
                titleMat.SetFloat("_MAIN", titleMAIN);

                if (titleMAIN < .01f)
                {
                    titleMat.SetFloat("_MAIN", titleMAIN);
                    step++;
                }
                break;
            
            case 6:
                symbolManager.Appear();
                if (symbolManager.hasAppeared)
                {
                    step++;
                    GameLoopManager.Instance.OnGameLoopReady();
                    AudioManager.Instance.StartCursorMoveSound(GameManager.Instance.GetMouseManager().GetMouseAura());
                }

                break;

            case 7:
                titleReveal = false;
                // if (!cameraUp)
                // {
                //     mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraSpots[1].position, cameraMoveSpeed * Time.deltaTime);
                //     mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraSpots[1].rotation, cameraMoveSpeed * Time.deltaTime);
                // }
                sinjManager.InfluencedByMouse(true);
                mouseManager.IsUsed = true;
                AudioManager.Instance.SetGameStateGameplay();
                dofVolume.weight = Mathf.Lerp(dofVolume.weight, 0, Time.deltaTime * dofSpeed);

                
                break;
        }

        if (step < 5)
        {
            // if (cameraUp)
            // {
            //     mainCamera.transform.position = cameraSpots[0].position;
            //     mainCamera.transform.rotation = cameraSpots[0].rotation;
            // }

            dofVolume.weight = Mathf.Lerp(dofVolume.weight, 1, Time.deltaTime * dofSpeed);
            Cursor.lockState = CursorLockMode.Confined;
        }


    }

    public void LoopReset()
    {
        step = 2;
        coverAnimation = true;
        sinjManager.InfluencedByMouse(false);
        mouseManager.IsUsed = false;

        GameManager.Instance.ResetEmotionPalliers();

        AudioManager.Instance.ResetEndMusic();
    }

    public void OnDestroy()
    {
        titleMat.SetFloat("_MAIN", .5f);
    }
}
