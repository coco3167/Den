using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SmartObjects_AI.Agent
{
    public class AnimationAgent : MonoBehaviour
    {
        // ID for animator
        private static readonly int StartUse = Animator.StringToHash("StartUse");
        private static readonly int FinishUse = Animator.StringToHash("FinishUse");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int FinishFast = Animator.StringToHash("FinishFast");
        private static readonly int SkipStart = Animator.StringToHash("SkipStart");
        private static readonly int SkipEnd = Animator.StringToHash("SkipEnd");

        [SerializeField] private SmartAgent smartAgent;
        [SerializeField] private MovementAgent movementAgent;
        [SerializeField] private float rotationLerpSpeed = 10;

        private Animator m_animator;

        private bool m_shouldStopAnimationAgent;
        private bool m_startedAnimation;

        [SerializeField, ReadOnly] private Transform m_currentLookingObject;
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

            if (IsAnimationReady())
            {
                StartMovementAgent();
                StopLookingObject();
            }

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

        public void SwitchAnimator(SmartObjectData data, Transform lookingObject)
        {
            m_animator.runtimeAnimatorController = data.animatorController;
            m_startedAnimation = true;

            if (data.shouldLookAtObject)
            {
                m_currentLookingObject = lookingObject;
            }

            
            m_animator.SetBool(SkipStart, data.shouldSkipStart);
            m_animator.SetBool(SkipEnd, data.shouldSkipEnd);
            m_animator.SetBool(FinishFast, data.shouldEndFast);
            
            m_animator.SetTrigger(StartUse);
            
            if(data.shouldStopAgent)
                movementAgent.StopAgent();
        }

        public void FinishUseAnimation(bool shouldEnd, bool shouldInterrupt)
        {
            m_startedAnimation = false;
            m_animator.SetBool(FinishUse, shouldEnd);

            if (shouldEnd && shouldInterrupt)
            {
                m_animator.SetBool(FinishFast, true);
                m_animator.SetBool(SkipEnd, true);
            }
        }

        public void OnPallierFinished(AgentDynamicParameter parameter)
        {
            GameManager.Instance.InfluencedByMouse(true);
            smartAgent.SetDynamicParameter(parameter, 0);
        }

        public void StartMovementAgent()
        {
            movementAgent.StartAgent();
        }
        
        public void StopMovementAgent()
        {
            movementAgent.StopAgent();
        }

        public void StopLookingObject()
        {
            m_currentLookingObject = null;
        }

        public bool IsAnimationReady()
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).IsTag("Usable") && !m_startedAnimation;
        }

    }
}
