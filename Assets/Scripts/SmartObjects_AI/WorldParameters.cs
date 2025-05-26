using System;
using AYellowpaper.SerializedCollections;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable]
    public class WorldParameters
    { 
        public SerializedDictionary<WorldParameterType, float> parameters;
        private MouseManager m_mouseManager;

        public WorldParameters(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
        }

        public Vector3 GetMousePositon()
        { 
            return m_mouseManager.GetRawWorldMousePosition();
        }
        
        public enum WorldParameterType
        {
            None,
            Hunger
        }
    }
}