using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Awake()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.forward);
    }
}
