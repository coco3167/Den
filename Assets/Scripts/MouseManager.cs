using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Rigidbody mouseRigidBody;
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private LayerMask terrainLayerMask;

    [SerializeField, ReadOnly] private Vector2 deltaSum;

    private Camera m_camera;
    
    private void Awake()
    {
        GameManager.Instance.GameReady += delegate
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_camera = GameManager.Instance.GetCamera();
            Mouse.current.WarpCursorPosition(m_camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f)));
        };
    }

    private void OnMouseMoved(InputValue value)
    {
        Vector2 tmpMouseDelta = value.Get<Vector2>();
        if (tmpMouseDelta.magnitude > 0f)
        {
            deltaSum += tmpMouseDelta;
            return;
        }
        
        if(!Application.isFocused)
            return;
        
        
        Vector2 mouseDelta = deltaSum * mouseSensitivity * 0.01f;
        Vector3 mousePos = mouseRigidBody.position + m_camera.transform.TransformDirection(new Vector3(mouseDelta.x, 0.0f, mouseDelta.y));
        deltaSum = Vector2.zero;
        
        // Keep the mouse on the ground
        Physics.Raycast(mousePos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 100, terrainLayerMask);
        if(hit.collider == null)
            Physics.Raycast(mousePos + Vector3.down * 10f, Vector3.up, out hit, 100, terrainLayerMask);
        if(hit.collider == null)
            return;
        mousePos.y = hit.point.y;
        
        mouseRigidBody.MovePosition(mousePos);
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
