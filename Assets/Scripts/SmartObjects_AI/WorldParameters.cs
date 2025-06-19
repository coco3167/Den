using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    [Serializable]
    public class WorldParameters : IReloadable
    {
        public Dictionary<AgentDynamicParameter, float> AgentGlobalParameters = new();
        private static readonly AgentDynamicParameter[] EmotionsType =
        {
            AgentDynamicParameter.Curiosity,
            AgentDynamicParameter.Aggression,
            AgentDynamicParameter.Fear
        };
        
        private Dictionary<WorldParameterType, float> m_parameters;
        private MouseManager m_mouseManager;

        public WorldParameters(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
            EmotionsType.ForEach(x => AgentGlobalParameters.Add(x, 0));

            m_parameters = new()
            {
                { WorldParameterType.EndSleep, 0 },
                { WorldParameterType.EndFleeOuterSpace, 0 },
                { WorldParameterType.EndAggression, 0 },
            };
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
            EndSleep,
            EndFleeOuterSpace,
            EndAggression,
        }

        public void Reload()
        {
            EmotionsType.ForEach(x => AgentGlobalParameters[x] = 0);
        }
    }
}