using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Sinj
{
    public class SinjAgent : MonoBehaviour, IDebugDisplayAble
    {
        [SerializeField] private List<SinjBehavior> behaviors;
        [SerializeField, ReadOnly] private SerializedDictionary<Emotions, float> emotions;
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;

        private readonly List<DebugParameter> m_debugParameters = new();
    
        // Fleeing
        private bool m_fleeing;
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnDrawGizmos()
        {
            if(m_fleeing)
                Gizmos.DrawRay(transform.position, m_fleeingTarget);
        }
        
        public void Init(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
            for (int loop = 0; loop < Enum.GetNames(typeof(Emotions)).Length; loop++)
            {
                Emotions emotion = (Emotions)loop;
                emotions.Add(emotion, 0f);
                m_debugParameters.Add(new DebugParameter(emotion.ToString(), "0"));
            }
        }

        public void HandleBehaviors()
        {
            foreach (SinjBehavior behavior in behaviors)
            {
                bool reactToStimuli = true;
                foreach (SinjBehavior.SinjStimulus stimulus in behavior.SinjStimuli)
                {
                    if (!stimulus.IsApplying(this))
                    {
                        reactToStimuli = false;
                        break;
                    }
                }

                if (!reactToStimuli)
                    continue;

                foreach (SinjBehavior.SinjReaction reaction in behavior.SinjReactions)
                {
                    reaction.ApplyReaction(this);
                }
            }
        }

        public void UpdateEmotion(float value, Emotions emotion)
        {
            int index = (int)emotion;
            emotions[emotion] = value;
            m_debugParameters[index].UpdateValue(((int)value).ToString());
        }

        #region Getter
        public float DistanceToMouse()
        {
            return m_mouseManager.ObjectDistanceToMouse(transform.position);
        }

        public float MouseVelocity()
        {
            return m_mouseManager.MouseVelocity();
        }

        public float GetEmotion(Emotions emotion)
        {
            return emotions[emotion];
        }
        #endregion

        #region Reactions
        public void FleeReaction(float distanceToFlee)
        {
            Vector3 directionToFlee = (transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
            m_fleeingTarget = distanceToFlee * directionToFlee;
            m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
        }

        public void AddEmotion(float amount, Emotions emotion)
        {
            emotions[emotion] += amount*Time.deltaTime;
        }
        #endregion

        #region Debug
        public int GetParameterCount()
        {
            return m_debugParameters.Count;
        }

        public DebugParameter GetParameter(int index)
        {
            return m_debugParameters[index];
        }
        #endregion
    }
    public enum Emotions
    {
        Tension,
        Curiosity,
        Agression,
        Fear,
    }
}