using UnityEngine;

public class FXCollider : MonoBehaviour
{
    public SphereCollider m_collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, m_collider.radius);
    }
}
