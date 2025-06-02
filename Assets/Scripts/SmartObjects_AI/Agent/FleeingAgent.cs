using System;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    public class FleeingAgent : MonoBehaviour
    {
        [SerializeField] private float distance;
        [SerializeField] private MouseAgent agent;
        
        private MouseManager m_mouseManager;

        private void Awake()
        {
            m_mouseManager = GameManager.Instance.GetMouseManager();
            
        }

        private void FixedUpdate()
        {
            transform.position = agent.transform.position + distance * (agent.transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
            
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
    }
}
