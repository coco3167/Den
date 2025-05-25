using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PersonnalFolders.Corentin.UI
{
    public class Graph : MonoBehaviour
    {
        //[SerializeField] private Slider slider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private LineRenderer lineRenderer;

        [SerializeField] private int pointNb;
        [SerializeField] private float updateRate;
        [SerializeField] private AnimationCurve curve;

        private Vector2 m_size;
        private Queue<Vector3> m_points = new();
        private readonly Queue<Vector3> m_tmpQueue = new();

        private int m_nbIter;
        private float m_totalValue;

        private void Awake()
        {
            m_size = GetComponent<RectTransform>().rect.size;
            spriteRenderer.size = m_size;
            lineRenderer.positionCount = pointNb;
            for (int loop = 0; loop < pointNb; loop++)
            {
                m_points.Enqueue(Vector3.zero);
            }
            
            InvokeRepeating(nameof(UpdatePoints), 0, updateRate);
        }
        
        //smooth avec une moyenne

        private void Update()
        {
            m_nbIter++;
            m_totalValue += curve.Evaluate(GameManager.Instance.GetMouseManager().MouseVelocity());
        }

        private void UpdatePoints()
        {
            if(m_nbIter > 0)
                m_totalValue /= m_nbIter;
            m_points.Enqueue(new Vector3(0, m_totalValue * m_size.y, 0));
            m_totalValue = 0;
            m_nbIter = 0;
            
            
            if (m_points.Count > pointNb)
                m_points.Dequeue();

            m_tmpQueue.Clear();
            int pointsCount = m_points.Count;
            for (var loop = 0; loop < pointsCount; loop++)
            {
                Vector3 point = m_points.Dequeue();
                point.x = loop * m_size.x / pointNb;
                m_tmpQueue.Enqueue(point);
            }

            m_points = new Queue<Vector3>(m_tmpQueue);
            lineRenderer.SetPositions(m_points.ToArray());
        }
    }
}
