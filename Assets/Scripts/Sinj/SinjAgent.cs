using System;
using System.Collections.Generic;
using System.Timers;
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
        // TODO faire une classe StateMachine
        [Header("States")]
        [SerializeField, ReadOnly] private ActiveState activeState = ActiveState.None;
        [SerializeField, ReadOnly] private PassiveState passiveState = PassiveState.None;

        private Timer m_restTimer;
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;

        private readonly List<DebugParameter> m_debugParameters = new();
    
        // Fleeing
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_restTimer = new Timer(1000);
            m_restTimer.Enabled = false;
            m_restTimer.AutoReset = false;
            m_restTimer.Elapsed += (_, _) =>
            {
                Debug.Log("reset");
                ResetState();
            };
            m_navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void OnDrawGizmos()
        {
            if(activeState == ActiveState.Fleeing)
                Gizmos.DrawRay(transform.position, m_fleeingTarget);
        }

        private void FixedUpdate()
        {
            if (activeState == ActiveState.Fleeing)
                if(m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                    ResetState();
            
            if(passiveState == PassiveState.Walking)
                if(m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                    ResetState();
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

            if (activeState == ActiveState.None && passiveState == PassiveState.None)
            {
                int index = Random.Range(0, passiveBehaviors.Count);
                passiveBehaviors[index].ApplyReaction(this);
            }
            
            // TODO dégeulasse a refacto
            m_debugParameters[4].Value = activeState.ToString();
            m_debugParameters[5].Value = passiveState.ToString();
        }

        private void ResetState()
        {
            activeState = ActiveState.None;
            passiveState = PassiveState.None;
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

        public void Rest(float minTime, float maxTime)
        {
            m_restTimer.Interval = Random.Range(minTime, maxTime) * 1000;
            m_restTimer.Start();
            passiveState = PassiveState.Resting;
        }

        public void Walk(float distance)
        {
            Vector3 destination;
            do
            {
                destination = transform.position + Random.insideUnitSphere * distance;
            } while (!m_navMeshAgent.SetDestination(destination));
            passiveState = PassiveState.Walking;
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
            Walking,
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