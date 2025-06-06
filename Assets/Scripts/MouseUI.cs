using Sirenix.OdinInspector;
using UnityEngine;

public class MouseUI : MonoBehaviour
{
    [SerializeField, ChildGameObjectsOnly] private RectTransform rectTransform;
    [SerializeField] private MouseManager mouseManager;


    private Camera m_camera;
    private Vector2 m_viewportSize;

    private void Awake()
    {
        m_camera = GameManager.Instance.GetCamera();
    }


    private void Update()
    {
        m_viewportSize = new Vector2(m_camera.scaledPixelWidth, m_camera.scaledPixelHeight);
        rectTransform.position = m_camera.WorldToViewportPoint(mouseManager.GetRawWorldMousePosition()) * m_viewportSize;
        
        //Debug.Log(m_camera.WorldToViewportPoint(mouseManager.GetRawWorldMousePosition()));
    }
}
