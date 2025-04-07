using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector2 rawPosition;
    [SerializeField, ReadOnly] private Vector3 rawWorldPosition;
    
    private Camera m_cam;
    private Ray m_mouseRay;
    
    private Vector2 m_oldRawPosition;
    private float m_velocity;
    

    private void Awake()
    {
        m_cam = Camera.main;
    }

    private void Update()
    {
        m_velocity /=   Mathf.Pow(100, Time.deltaTime);
        Debug.Log(m_velocity);
    }

    private void OnMouseMoved(InputValue value)
    {
        m_oldRawPosition = rawPosition;
        rawPosition = value.Get<Vector2>();
        m_mouseRay = m_cam.ScreenPointToRay(rawPosition);
        
        m_velocity = (m_oldRawPosition - rawPosition).magnitude;
        
        if (Physics.Raycast(m_mouseRay, out RaycastHit hit))
        {
            rawWorldPosition = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rawWorldPosition, 0.2f);
        Gizmos.DrawRay(m_mouseRay);
        Gizmos.DrawRay(m_oldRawPosition, m_oldRawPosition + rawPosition);
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
