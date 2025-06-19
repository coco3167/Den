using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private Rigidbody mouseRigidBody;
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Material UIcursorMat;

    private Camera m_camera;
    private Vector2 m_otherMoveValue;
    private bool m_isOtherMoving;

    private Vector3 m_movementNewPos;
    private RaycastHit m_hit;

    [NonSerialized] public bool IsUsed = true;

    private void Awake()
    {
        m_camera = GameManager.Instance.GetCamera();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.IsPaused)
            return;

        if (m_isOtherMoving)
            MoveRigidBody(m_otherMoveValue);
            
    }

    private void Update()
    {
        UpdateCursorMaterial();
    }

    public void OnGameReady(object sender, EventArgs eventArgs)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Mouse.current.WarpCursorPosition(m_camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f)));
        //mouseRigidBody.MovePosition(new Vector3(1,1,-3));
    }

    public void OnGameEnded(object sender, EventArgs eventArgs)
    {
        //nothing
    }

    public void OnMouseMoved(Vector2 value)
    {
        value *= Options.GameParameters.MouseSensitivity;
        MoveRigidBody(value);
    }

    public void OnOtherMoveStart(Vector2 value)
    {
        m_otherMoveValue = value * Options.GameParameters.JoystickSensitivity;
        m_isOtherMoving = true;
    }

    public void OnOtherMoveEnd()
    {
        m_isOtherMoving = false;
    }

    private void MoveRigidBody(Vector2 movement)
    {
        movement *= Time.deltaTime;
        if (movement == Vector2.zero)
            return;
        m_movementNewPos = mouseRigidBody.position + m_camera.transform.TransformDirection(new Vector3(movement.x, 0.0f, movement.y));

        Physics.Raycast(m_movementNewPos + Vector3.up * 10f, Vector3.down, out m_hit, 100, terrainLayerMask);
        if (!m_hit.collider)
            return;
        m_movementNewPos.y = m_hit.point.y + mouseRigidBody.transform.localScale.y / 2;

        mouseRigidBody.MovePosition(m_movementNewPos);
    }

    public float ObjectDistanceToMouse(Vector3 otherPos)
    {
        if (!IsUsed)
            return float.MaxValue;
        return (otherPos - mouseRigidBody.position).sqrMagnitude;
    }

    public Vector3 GetRawWorldMousePosition()
    {
        return mouseRigidBody.position;
    }

    public Transform GetMouseTransform()
    {
        return mouseRigidBody.transform;
    }

    public float MouseVelocity()
    {
        return mouseRigidBody.linearVelocity.magnitude;
    }

    public void UpdateCursorMaterial()
    {
        float newRange = Mathf.Lerp(UIcursorMat.GetFloat("_range"), MouseVelocity() / 10, Time.deltaTime * 5);
        UIcursorMat.SetFloat("_range", Mathf.Clamp(newRange,0,1));
    }
}
