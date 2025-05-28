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
        //[SerializeField] private List<SinjPassivBehavior> passiveBehaviors;
        
        [Header("Emotions")]
        [SerializeField, ReadOnly] private SerializedDictionary<Emotions, float> emotions;
        [SerializeField] private SerializedDictionary<Emotions, float> emotionsDisplayCap;

        [Title("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField, Range(0, 3.5f)] private float runningSpeed;
        [SerializeField] private NavMeshAgentParameters calmAgent, runningAgent;
        
        private StateMachine m_stateMachine = new();
    
        private NavMeshAgent m_navMeshAgent;
        private MouseManager m_mouseManager;
        
        private readonly List<DebugParameter> m_debugParameters = new();
        private static int _avoidanceValue;

        private float m_scale;
    
        // Fleeing
        private Vector3 m_fleeingTarget;

        private void Awake()
        {
            m_scale = transform.localScale.x;
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            gameObject.AddComponent<AkGameObj>();
            SetCalmAgent();
        }

        public void Init(MouseManager mouseManager)
        {
            m_mouseManager = mouseManager;
            for (int loop = 0; loop < Enum.GetNames(typeof(Emotions)).Length; loop++)
            {
                Emotions emotion = (Emotions)loop;
                emotions.Add(emotion, 0f);
                if(emotion == Emotions.Intensity)
                    continue;
                m_debugParameters.Add(new DebugParameter(emotion.ToString(), "0"));
            }

            m_navMeshAgent.avoidancePriority = _avoidanceValue++;
            
            m_debugParameters.Add(new DebugParameter("Current State", "Null"));
        }

        private void FixedUpdate()
        {
            animator.SetBool("Walking", m_navMeshAgent.velocity.magnitude > 0);
            animator.SetBool("Running", m_navMeshAgent.velocity.magnitude > runningSpeed);
            
            animator.SetBool("Curiosity", emotions[Emotions.Curiosity] >= emotionsDisplayCap[Emotions.Curiosity]);
            animator.SetBool("Agression", emotions[Emotions.Agression] >= emotionsDisplayCap[Emotions.Agression]);
            animator.SetBool("Fear", emotions[Emotions.Fear] >= emotionsDisplayCap[Emotions.Fear]);
            
            transform.localScale = new Vector3(m_navMeshAgent.velocity.x > 0 ? -1f : 1f, 1f, 1f) * m_scale;
        }

        public void HandleBehaviors()
        {
            m_stateMachine.ResetBehavior(this);
            if (!m_stateMachine.HasBehavior(true))
            {
                foreach (SinjActiveBehavior behavior in activeBehaviors)
                {
                    // if (!behavior.IsApplying(this))
                    //     continue;
                    //behavior.ApplyReaction(this);
                }
                
                if (!m_stateMachine.HasBehavior())
                {
                    SetCalmAgent();
                    //m_stateMachine.ChoosePassivBehavior(passiveBehaviors);
                    //m_stateMachine.CurrentBehavior.ApplyReaction(this);
                }
            }
            

            m_debugParameters[4].UpdateValue(m_stateMachine.ToString());
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

        public Emotions GetMainEmotion()
        {
            if (emotions[Emotions.Curiosity] < emotions[Emotions.Agression])
            {
                return emotions[Emotions.Agression] < emotions[Emotions.Fear] ? Emotions.Fear : Emotions.Agression;
            }

            return emotions[Emotions.Curiosity] < emotions[Emotions.Fear] ? Emotions.Fear : Emotions.Curiosity;
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
            SetRunningAgent();
        }

        public void Rest(float minTime, float maxTime) {}

        public void Walk(float distance)
        {
            Vector3 destination;
            do
            {
                Vector2 randomPoint = Random.insideUnitCircle;
                destination = Vector3.zero + new Vector3(randomPoint.x, 0, randomPoint.y) * distance;
            } while (!m_navMeshAgent.SetDestination(destination));
        }
        
        public bool IsCloseToDestination()
        {
            bool finished = m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance;
            
            if (finished)
            {
                if (emotions[Emotions.Curiosity] >= emotionsDisplayCap[Emotions.Curiosity])
                    animator.SetTrigger("EndCuriosityFlee");

                if (emotions[Emotions.Agression] >= emotionsDisplayCap[Emotions.Agression])
                    animator.SetTrigger("EndAgressionFlee");
                
                return true;
            }
            return false;
        }

        public void PlaySound(AK.Wwise.Event eventToPlay)
        {
            eventToPlay.Post(gameObject);
        }
        #endregion

        #region AgentParameters
        private void SetCalmAgent()
        {
            m_navMeshAgent.speed = calmAgent.maxSpeed;
            m_navMeshAgent.acceleration = calmAgent.acceleration;
        }

        private void SetRunningAgent()
        {
            m_navMeshAgent.speed = runningAgent.maxSpeed;
            m_navMeshAgent.acceleration = runningAgent.acceleration;
            animator.SetTrigger("Flee");
        }
        
        [Serializable]
        private struct NavMeshAgentParameters
        {
            [field: SerializeField] public float maxSpeed { get; private set; }
            [field: SerializeField] public float acceleration { get; private set; }
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