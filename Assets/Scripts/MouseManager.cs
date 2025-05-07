using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Rigidbody mouseRigidBody;
    [SerializeField] private LayerMask terrainLayerMask;

    private Vector2 m_OtherMoveValue;
    private bool m_isOtherMoving;

    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Mouse.current.WarpCursorPosition(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f))); 
    }

    private void FixedUpdate()
    {
        if(m_isOtherMoving)
            MoveRigidBody(m_OtherMoveValue);
    }

    public void OnMouseMoved(Vector2 value)
    {
        value *= Options.GameParameters.MouseSensitivity;
        MoveRigidBody(value);
    }

    public void OnOtherMoveStart(Vector2 value)
    {
        m_OtherMoveValue = value * Options.GameParameters.JoystickSensitivity;
        m_isOtherMoving = true;
    }

    public void OnOtherMoveEnd()
    {
        m_isOtherMoving = false;
    }

    private void MoveRigidBody(Vector2 movement)
    {
        Vector3 newPos = mouseRigidBody.position + new Vector3(movement.x, 0.0f, movement.y);
        Physics.Raycast(newPos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 100, terrainLayerMask);
        if(hit.collider == null)
            Physics.Raycast(newPos + Vector3.down * 10f, Vector3.up, out hit, 100, terrainLayerMask);
        if(hit.collider == null)
            return;
        newPos.y = hit.point.y;
        
        mouseRigidBody.MovePosition(newPos);
    }

    public float ObjectDistanceToMouse(Vector3 otherPos)
    {
        return (otherPos - mouseRigidBody.position).sqrMagnitude;
    }

    public Vector3 GetRawWorldMousePosition()
    {
        return mouseRigidBody.position;
    }

    public float MouseVelocity()
    {
        return mouseRigidBody.linearVelocity.magnitude;
    }
}
