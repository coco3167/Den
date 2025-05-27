using UnityEngine;

public class LeafManager : MonoBehaviour
{
    public ParticleSystem poufSystem;
    public ParticleSystem dragSystem;
    public bool pouf = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dragSystem.Stop();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("VFXCollider"))
        {
            if (pouf)
            {
                poufSystem.Play();
            }
            dragSystem.Play();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("VFXCollider"))
        {
            dragSystem.Stop();
        }
    }
}
