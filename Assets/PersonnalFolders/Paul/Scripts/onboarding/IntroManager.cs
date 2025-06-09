using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class IntroManager : MonoBehaviour
{
    [Header("General")]
    [Range(1, 5)]
    public int step;
    private string[] stepNames = { "black screen", "fade", "onboarding",  "title", "game" };
    public string currentStep;

    [Header("Scripts")]
    public BranchesManager branchesManager;
    public MouseManager mouseManager;

    [Header("Black Background")]
    
    public RawImage blackBackground;
    public float fadeSpeed;
    public float distanceMoved;

    [Header("Camera Movement")]
    public Camera mainCamera;
    public Transform[] cameraSpots;
    public float cameraMoveSpeed;

    [Header("Transition to Game")]
    
    public float titleDelay = 3;
    public GameObject uiCursor;

    [Header("Depth of Field (WIP)")]
    public VolumeProfile volumeProfile;
    public Vector2 dofRange;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        Color color = blackBackground.color;
        color.a = 1;
        blackBackground.color = color;

        mainCamera.transform.position = cameraSpots[0].position;
        
        // if (volumeProfile.TryGet(out DepthOfField dof))
        // {
        //     dof.focusDistance.value = dofRange.x; // Set your desired focus distance
        // }
    }

    // Update is called once per frame
    void Update()
    {
        currentStep = stepNames[step - 1];

        distanceMoved += mouseManager.MouseVelocity();
        

        Cursor.lockState = step == 5 ? CursorLockMode.Locked : CursorLockMode.None;
        uiCursor.SetActive(step == 5);

        branchesManager.transform.position = mainCamera.transform.position;
        branchesManager.transform.rotation = mainCamera.transform.rotation;

        if (step == 1 && distanceMoved > 500)
        {
            step = 2;
        }

        if (step == 2)
        {
            Color color = blackBackground.color;
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * fadeSpeed);
            if (color.a < 0.2f)
            {
                color.a = 0;
                step = 3;
            }
            blackBackground.color = color;
        }

        if (step == 3)
        {
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

        if (step == 5)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraSpots[1].position, cameraMoveSpeed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, cameraSpots[1].rotation, cameraMoveSpeed * Time.deltaTime);
            // if (volumeProfile.TryGet(out DepthOfField dof))
            // {

            //     // dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, dofRange.y, Time.deltaTime * cameraMoveSpeed); // Set your desired focus distance
            //     dof.focusDistance.value = dofRange.y;
            // }
        }

        else
        {
            mainCamera.transform.position = cameraSpots[0].position;
            mainCamera.transform.rotation = cameraSpots[0].rotation;
        }
    }
}
