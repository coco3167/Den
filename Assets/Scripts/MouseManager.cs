using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Rigidbody mouseRigidBody;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private LayerMask terrainLayerMask;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Mouse.current.WarpCursorPosition(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f))); 
    }


    private void OnMouseMoved(InputValue value)
    {
        if(!Application.isFocused)
            return;
        
        Vector2 mouseDelta = value.Get<Vector2>() * mouseSensitivity * 0.01f;
        Vector3 mousePos = mouseRigidBody.position + new Vector3(mouseDelta.x, 0.0f, mouseDelta.y);
        
        // Keep the mouse on the ground
        Physics.Raycast(mousePos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 100, terrainLayerMask);
        if(hit.collider == null)
            Physics.Raycast(mousePos + Vector3.down * 10f, Vector3.up, out hit, 100, terrainLayerMask);
        if(hit.collider == null)
            return;
        mousePos.y = hit.point.y;
        
        mouseRigidBody.MovePosition(mousePos);
        
        Debug.Log("mouse moved", this);
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
