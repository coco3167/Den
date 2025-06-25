using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MouseUI : MonoBehaviour, IPausable
{
    [SerializeField, ChildGameObjectsOnly] private RectTransform rectTransform;
    [SerializeField, ChildGameObjectsOnly] private RawImage rawImage;
    [SerializeField] private MouseManager mouseManager;
    [SerializeField] private bool m_groundedMode;
    [SerializeField] private IntroManager introManager;
    [SerializeField] private GameObject tutoArrows;


    private Camera m_camera;
    private Vector2 m_viewportSize;

    private void Awake()
    {
        m_camera = GameManager.Instance.GetCamera();
    }


    private void Update()
    {
        m_viewportSize = new Vector2(m_camera.scaledPixelWidth, m_camera.scaledPixelHeight);
        if (m_groundedMode)
        {
            rectTransform.position = m_camera.WorldToViewportPoint(mouseManager.GetRawWorldMousePosition()) * m_viewportSize;
        }
        else
        {
            rawImage.rectTransform.position = Input.mousePosition;
        }

        m_groundedMode = !GameManager.Instance.IsPaused && introManager.step >= 7;
        tutoArrows.SetActive(introManager.step != 6);
    }

    public void OnGamePaused(object sender, EventArgs eventArgs)
    {
        // rawImage.enabled = !GameManager.Instance.IsPaused;
        
    }
}
