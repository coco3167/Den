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

        [Title("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField, Range(0, 3.5f)] private float walkingSpeed, runningSpeed;
        
        private StateMachine m_stateMachine = new();
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;

        private readonly List<DebugParameter> m_debugParameters = new();
    
        // Fleeing
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
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
            
            m_debugParameters.Add(new DebugParameter("Current State", "Null"));
        }

        private void FixedUpdate()
        {
            animator.SetBool("Walking", m_navMeshAgent.velocity.magnitude >= walkingSpeed);
            animator.SetBool("Running", m_navMeshAgent.velocity.magnitude >= runningSpeed);
            
            transform.localScale = new Vector3(m_navMeshAgent.velocity.x > 0 ? 1f : -1f, 1f, 1f);
        }

        public void HandleBehaviors()
        {
            foreach (SinjActiveBehavior behavior in activeBehaviors)
            {
                if (!behavior.IsApplying(this))
                    continue;
                behavior.ApplyReaction(this);
            }
            
            if (m_stateMachine.HasBehavior() && m_stateMachine.CurrentBehavior.IsFinished(this))
                    m_stateMachine.ResetBehavior();

            if (!m_stateMachine.HasBehavior())
            {
                m_stateMachine.ChoosePassivBehavior(passiveBehaviors);
                m_stateMachine.CurrentBehavior.ApplyReaction(this);
            }

            m_debugParameters[4].Value = m_stateMachine.CurrentBehavior.ToString();
        }

        public void UpdateEmotion(float value, Emotions emotion)
        {
            int index = (int)emotion;
            emotions[emotion] = value;
            m_debugParameters[index].UpdateValue((int)value + "/100");
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
        public void AddEmotion(float amount, Emotions emotion)
        {
            emotions[emotion] += amount*Time.deltaTime;
        }

        public void ChangeBehavior(SinjBehavior behavior)
        {
            m_stateMachine.ChangeBehavior(behavior);
        }
        
        public void FleeReaction(float distanceToFlee)
        {
            Vector3 directionToFlee = (transform.position - m_mouseManager.GetRawWorldMousePosition()).normalized;
            m_fleeingTarget = distanceToFlee * directionToFlee;
            m_navMeshAgent.SetDestination(transform.position + m_fleeingTarget);
        }

        public void Rest(float minTime, float maxTime) {}

        public void Walk(float distance)
        {
            Vector3 destination;
            do
            {
                destination = transform.position + Random.insideUnitSphere * distance;
            } while (!m_navMeshAgent.SetDestination(destination));
        }
        
        public bool IsCloseToDestination()
        {
            return m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance;
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
        Intensity
    }
}