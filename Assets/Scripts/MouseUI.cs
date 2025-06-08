using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour, IPausable
{
    [SerializeField, ChildGameObjectsOnly] private RectTransform rectTransform;
    [SerializeField, ChildGameObjectsOnly] private RawImage rawImage;
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
    }

    public void OnGamePaused(object sender, EventArgs eventArgs)
    {
        rawImage.enabled = !GameManager.Instance.IsPaused;
    }
}
