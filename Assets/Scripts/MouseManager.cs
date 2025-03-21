using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector2 rawMousePosition;
    [SerializeField, ReadOnly] private Vector3 rawWorldMousePosition;
    
    private Camera m_cam;
    private Ray m_mouseRay;
    

    private void Awake()
    {
        m_cam = Camera.main;
    }

    private void OnMouseMoved(InputValue value)
    {
        rawMousePosition = value.Get<Vector2>();
        m_mouseRay = m_cam.ScreenPointToRay(rawMousePosition);
        if (Physics.Raycast(m_mouseRay, out RaycastHit hit))
        {
            rawWorldMousePosition = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rawWorldMousePosition, 0.2f);
        Gizmos.DrawRay(m_mouseRay);
    }

    public float ObjectDistanceToMouse(Vector3 otherPos)
    {
        return (otherPos - rawWorldMousePosition).sqrMagnitude;
    }
}
