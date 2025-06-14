using System;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    public class AnimationAgent : MonoBehaviour
    {
        // ID for animator
        private static readonly int StartUse = Animator.StringToHash("StartUse");
        private static readonly int FinishUse = Animator.StringToHash("FinishUse");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Curiosity = Animator.StringToHash("Curiosity");
        private static readonly int Aggression = Animator.StringToHash("Aggression");
        private static readonly int Fear = Animator.StringToHash("Fear");
        private static readonly int FinishFast = Animator.StringToHash("FinishFast");
        private static readonly int SkipStart = Animator.StringToHash("SkipStart");
        private static readonly int SkipEnd = Animator.StringToHash("SkipEnd");

        [SerializeField] private MovementAgent movementAgent;
        [SerializeField] private float rotationLerpSpeed = 10;

        private Animator m_animator;

        private bool m_shouldStopAnimationAgent;

        [NonSerialized] public Transform LookingObject;
        private Transform m_currentLookingObject;
        private Vector3 m_locationToLookAt;
        private Quaternion m_goalRotation;
        private Transform m_transformMovementAgent;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_transformMovementAgent = movementAgent.transform;
        }

        private void Update()
        {
            m_animator.SetFloat(Speed, movementAgent.GetSpeed());
            
            if (!m_currentLookingObject)
                return;
            
            m_locationToLookAt = m_currentLookingObject.position - m_transformMovementAgent.position;
            m_locationToLookAt.y = 0;
            m_goalRotation = Quaternion.LookRotation(m_locationToLookAt, transform.up);
            m_transformMovementAgent.rotation = Quaternion.Lerp(m_transformMovementAgent.rotation, m_goalRotation, rotationLerpSpeed*Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            if (m_currentLookingObject)
            {
                Gizmos.DrawLine(m_transformMovementAgent.position, m_currentLookingObject.position);
            }
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController)
        {
            m_animator.runtimeAnimatorController = animatorController;
            m_animator.SetTrigger(StartUse);
        }

        public void FinishUseAnimation(bool shouldInterrupt)
        {
            m_animator.SetTrigger(FinishUse);

            if (shouldInterrupt)
            {
                SetEndFast(true);
                SetSkipEnd(true);
            }
        }

        public void StartMovementAgent()
        {
            movementAgent.StartAgent();
            
            StopLookingObject();
        }
        
        public void StopMovementAgent()
        {
            if(m_shouldStopAnimationAgent)
                movementAgent.StopAgent();
            
            m_currentLookingObject = LookingObject;
        }

        public void StopLookingObject()
        {
            m_currentLookingObject = null;
        }

        public void SetStopMovementAgent(bool value)
        {
            m_shouldStopAnimationAgent = value;
        }

        public void SetEndFast(bool value)
        {
            m_animator.SetBool(FinishFast, value);
        }
        
        public void SetSkipStart(bool value)
        {
            m_animator.SetBool(SkipStart, value);
        }
        
        public void SetSkipEnd(bool value)
        {
            m_animator.SetBool(SkipEnd, value);
        }

        public bool IsAnimationReady()
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).IsTag("Usable");
        }
        
    }
}
