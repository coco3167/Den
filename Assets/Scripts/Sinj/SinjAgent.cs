using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DebugHUD;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Sinj
{
    public class SinjAgent : MonoBehaviour, IDebugDisplayAble
    {
        [Header("Behaviors")]
        [SerializeField] private List<SinjActiveBehavior> activeBehaviors;
        [SerializeField] private List<SinjPassivBehavior> passiveBehaviors;
        
        [Header("Emotions")]
        [SerializeField, ReadOnly] private SerializedDictionary<Emotions, float> emotions;
        
        // TODO déterminer si j'ai besoin de séparer active et passive states
        [Header("States")]
        [SerializeField, ReadOnly] private ActiveState activeState = ActiveState.None;
        [SerializeField, ReadOnly] private PassiveState passiveState = PassiveState.None;
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;

        private readonly List<DebugParameter> m_debugParameters = new();
    
        // Fleeing
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnDrawGizmos()
        {
            if(activeState == ActiveState.Fleeing)
                Gizmos.DrawRay(transform.position, m_fleeingTarget);
        }

        private void FixedUpdate()
        {
            if (activeState != ActiveState.None)
            {
                if (activeState == ActiveState.Fleeing)
                    if(m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                        activeState = ActiveState.None;
            }
            else
                if(passiveState == PassiveState.Resting)
                    Debug.Log("Resting");
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
            
            m_debugParameters.Add(new DebugParameter("Active State", activeState.ToString()));
            m_debugParameters.Add(new DebugParameter("Passive State", passiveState.ToString()));
        }

        public void HandleBehaviors()
        {
            foreach (SinjActiveBehavior behavior in activeBehaviors)
            {
                if (!behavior.IsApplying(this))
                    continue;
                behavior.ApplyReaction(this);
            }

            if (activeState == ActiveState.None)
            {
                int index = Random.Range(0, passiveBehaviors.Count);
                passiveBehaviors[index].ApplyReaction(this);
            }
            
            // TODO dégeulasse a refacto
            m_debugParameters[4].Value = activeState.ToString();
            m_debugParameters[5].Value = passiveState.ToString();
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
            activeState = ActiveState.Fleeing;
            m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
        }

        public void AddEmotion(float amount, Emotions emotion)
        {
            emotions[emotion] += amount*Time.deltaTime;
        }

        public void Rest()
        {
            passiveState = PassiveState.Resting;
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

        #region States
        private enum ActiveState
        {
            None,
            Fleeing,
        }

        private enum PassiveState
        {
            None,
            Resting,
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