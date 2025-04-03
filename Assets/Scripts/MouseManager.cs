using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector2 rawPosition;
    [SerializeField, ReadOnly] private Vector3 rawWorldPosition;
    
    private Camera m_cam;
    private Ray m_mouseRay;
    
    private Vector2 m_oldRawMousePosition;
    private float m_velocity;
    private bool m_moved;
    

    private void Awake()
    {
        m_cam = Camera.main;
    }

    private void Update()
    {
        if (m_moved)
            m_moved = false;
        else
            m_velocity = 0;
    }

    private void OnMouseMoved(InputValue value)
    {
        m_oldRawMousePosition = rawPosition;
        rawPosition = value.Get<Vector2>();
        m_mouseRay = m_cam.ScreenPointToRay(rawPosition);
        
        m_velocity = (m_oldRawMousePosition - rawPosition).magnitude;
        
        if (Physics.Raycast(m_mouseRay, out RaycastHit hit))
        {
            rawWorldPosition = hit.point;
        }
        
        m_moved = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rawWorldPosition, 0.2f);
        Gizmos.DrawRay(m_mouseRay);
    }

    public float ObjectDistanceToMouse(Vector3 otherPos)
    {
        return (otherPos - rawWorldPosition).sqrMagnitude;
    }

    public Vector3 GetRawWorldMousePosition()
    {
        return rawWorldPosition;
    }

    public float MouseVelocity()
    {
        return m_velocity;
    }
}
