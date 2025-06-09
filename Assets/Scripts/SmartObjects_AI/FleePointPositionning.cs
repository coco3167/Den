using System;
using Sinj;
using UnityEngine;

namespace SmartObjects_AI
{
    public class FleePointPositionning : MonoBehaviour
    {
        [SerializeField] private SinjManager sinjManager;
        [SerializeField] private float cohesionMult, fleeMult, distance;
        
        private MouseManager m_mouseManager;


        private void Awake()
        {
            m_mouseManager = GameManager.Instance.GetMouseManager();
        }

        private void FixedUpdate()
        {
            Vector3 mousePos = m_mouseManager.GetRawWorldMousePosition();
            Vector3 sumPositionAway = new Vector3();
            Vector3 sumPosition = new Vector3();
            float sumDistance = 0;
            sinjManager.mouseAgents.ForEach(x =>
            {
                float distance = m_mouseManager.ObjectDistanceToMouse(x.transform.position);
                sumDistance += distance;
                sumPosition += x.transform.position * distance;
                sumPositionAway += (x.transform.position - mousePos) / distance;
            });
            transform.position = (cohesionMult * sumPosition / sinjManager.mouseAgents.Count / sumDistance
                + fleeMult * (sumPositionAway / sinjManager.mouseAgents.Count + mousePos))
                / (cohesionMult + fleeMult) * distance;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(transform.position, Vector3.one);
        }
    }
}
