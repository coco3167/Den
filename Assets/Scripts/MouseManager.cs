using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private Rigidbody mouseRigidBody;
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Material UIcursorMat;
    [SerializeField] private Material tutoArrowsMat;
    [SerializeField] private GameObject mouseAura;

    private Camera m_camera;
    private Vector2 m_otherMoveValue;
    private bool m_isOtherMoving;

    private Vector3 m_movementNewPos;
    private RaycastHit m_hit;

    private Vector3 m_minPos, m_maxPos;

    [NonSerialized] public bool IsUsed = true;

    private void Awake()
    {
        m_camera = GameManager.Instance.GetCamera();

        // Ray minRay = m_camera.ViewportPointToRay(new Vector3(0, .8f, m_camera.nearClipPlane));
        // Ray maxRay = m_camera.ViewportPointToRay(new Vector3(1, 0, m_camera.nearClipPlane));
        //
        // Physics.Raycast(minRay, out RaycastHit minRayHit);
        // Physics.Raycast(maxRay, out RaycastHit maxRayHit);
        //
        // m_minPos = minRayHit.point;
        // m_maxPos = maxRayHit.point;
        //
        // Debug.Log(m_minPos);
        // Debug.Log(m_maxPos);
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
        Mouse.current.WarpCursorPosition(m_camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f)));
        mouseRigidBody.MovePosition(new Vector3(1,1,-3));
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

        // m_movementNewPos = new Vector3(
        //     Math.Clamp(m_movementNewPos.x, m_minPos.x, m_maxPos.x),
        //     Math.Clamp(m_movementNewPos.y, m_minPos.y, m_maxPos.y),
        //     Math.Clamp(m_movementNewPos.z, m_minPos.z, m_maxPos.z));

        mouseRigidBody.MovePosition(m_movementNewPos);
    }

    public float ObjectDistanceToMouse(Vector3 otherPos)
    {
        if (!IsUsed)
            return 10000;
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

    public GameObject GetMouseAura()
    {
        return mouseAura;
    }

    public void UpdateCursorMaterial()
    {

        float newRange = Mathf.Lerp(UIcursorMat.GetFloat("_range"), MouseVelocity() / 10, Time.deltaTime * 5);
        UIcursorMat.SetFloat("_range", Mathf.Clamp(newRange, 0, 1));

        
        if (MouseVelocity() > 0)
        {

            float newAlpha = Mathf.Lerp(tutoArrowsMat.GetFloat("_Alpha"), 0, Time.deltaTime * 2);
            tutoArrowsMat.SetFloat("_Alpha", newAlpha);

        }
        else
        {
            float newAlpha = Mathf.Lerp(tutoArrowsMat.GetFloat("_Alpha"), 1, Time.deltaTime * 0.5f);
            tutoArrowsMat.SetFloat("_Alpha", newAlpha);
        }
    }
}
