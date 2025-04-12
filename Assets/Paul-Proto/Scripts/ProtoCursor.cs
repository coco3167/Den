using UnityEngine;

public class ProtoCursor : MonoBehaviour
{
    public Camera mainCamera; // Assign your main camera in the Inspector
    public float fixedY = 1f; // Set this to whatever height you want
    public float speed = 5f;  // Movement speed
    

    public SinjIndiv[] sinjColl;

    void Update()
    {
        MoveToCursor();
        InfluenceSinj();
    }

    void MoveToCursor()
    {
        // Get mouse position in screen space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast to the ground (assumes the ground is on LayerMask "Ground")
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 targetPos = hit.point;
            targetPos.y = fixedY; // Lock Y-axis

            speed = Vector3.Distance(targetPos,transform.position);
            
            // Move smoothly to target
            transform.position = targetPos;
        }
    }

    void InfluenceSinj()
    {
        
        
        foreach (SinjIndiv sinj in sinjColl)
        {
            float distance = Vector3.Distance(sinj.gameObject.transform.position,transform.position);
            Debug.Log(distance);
            
            if (distance < sinj.range)
            {
                sinj.cursorFactor += Time.deltaTime;
                
            }
            else
            {
                sinj.cursorFactor -= Time.deltaTime;
                
            }
            sinj.cursorFactor = Mathf.Clamp(sinj.cursorFactor,0,1);
        }
    }
}