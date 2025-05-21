using UnityEngine;

public class TEMP_FollowMouse : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position+offset,Time.deltaTime*speed);
    }
}
