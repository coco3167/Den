using UnityEngine;

public class TEMP_MouseSprite : MonoBehaviour
{
    public float proximity;
    public Transform camera;
    public Transform mouseAura;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = camera.position;
        transform.LookAt(mouseAura);
        transform.position += transform.forward * proximity; 
    }
}
