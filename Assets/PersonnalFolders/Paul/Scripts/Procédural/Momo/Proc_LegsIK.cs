using UnityEngine;

public class Proc_LegsIK : MonoBehaviour
{
    public LayerMask groundMask;
    public Transform[] feet;
    public Transform[] feetTargets;
    public float heightOffset;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 origin = feetTargets[i].position + Vector3.up * 5f;
            Ray ray = new Ray(origin, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 10f, groundMask))
            {
                feetTargets[i].position = hit.point + new Vector3(0,heightOffset,0);
            }
        }
    }
}
