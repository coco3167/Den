using Sirenix.OdinInspector;
using UnityEngine;

public class BranchesManager : MonoBehaviour
{
    public IntroManager introManager;
    private string targetTag = "Branch";
    private float rayLength = 100f;
    private Color rayColor = Color.red;
    [ReadOnly] public Branch[] branches;

    void Start()
    {
        branches = GetComponentsInChildren<Branch>();
    }

    void Update()
    {
        Ray ray = introManager.mainCamera.ScreenPointToRay(Input.mousePosition);
        // Debug.DrawRay(ray.origin, ray.direction * rayLength, rayColor);

        if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag(targetTag) && introManager.step == 3)
            {

                Branch branchScript = hit.collider.GetComponentInParent<Branch>();

                branchScript.Hover(introManager.mouseManager.MouseVelocity());
            }
        }
    }
}
