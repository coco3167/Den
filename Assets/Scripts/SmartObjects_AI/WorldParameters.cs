using System;
using AYellowpaper.SerializedCollections;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable]
    public class WorldParameters
    { 
        private SerializedDictionary<WorldParameterType, float> m_parameters;
        private MouseManager m_mouseManager;

        public WorldParameters(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
        }

        public Vector3 GetMousePositon()
        { 
            return m_mouseManager.GetRawWorldMousePosition();
        }

        public float GetMouseVelocity()
        { 
            return m_mouseManager.MouseVelocity();
        }

        public float GetDynamicParameter(WorldParameterType parameter)
        {
            return m_parameters[parameter];
        }
        
        public void SetDynamicParameter(WorldParameterType parameter, float value)
        {
            m_parameters[parameter] = Math.Clamp(value, 0, 100);
        }

        public void AddDynamicParameter(WorldParameterType parameter, float value)
        {
            SetDynamicParameter(parameter, m_parameters[parameter] + value);
        }
        
        public enum WorldParameterType
        {
            None,
            Hunger
        }
    }
}