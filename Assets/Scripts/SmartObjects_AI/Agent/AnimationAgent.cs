using UnityEngine;
using UnityEngine.Serialization;

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

        [SerializeField] private MovementAgent movementAgent;
        private Animator m_animator;

        private bool m_isFinished = true;
        private bool m_adaptToMood = false;
        
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        private void Update()
        {
            m_animator.SetFloat(Speed, movementAgent.GetSpeed());
        }

        public void SwitchMood(AgentDynamicParameter parameter)
        {
            if(!m_adaptToMood)
                return;
            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    m_animator.SetBool(Curiosity, true);
                    break;
                case AgentDynamicParameter.Aggression:
                    m_animator.SetBool(Aggression, true);
                    break;
                case AgentDynamicParameter.Fear:
                    m_animator.SetBool(Fear, true);
                    break;
            }
        }

        public void ResetMood()
        {
            m_animator.SetBool(Curiosity, false);
            m_animator.SetBool(Aggression, false);
            m_animator.SetBool(Fear, false);
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController, bool adaptToMood)
        {
            m_isFinished = false;
            
            m_adaptToMood = adaptToMood;
            ResetMood();
            
            m_animator.runtimeAnimatorController = animatorController;
            m_animator.SetTrigger(StartUse);
        }

        public void FinishUseAnimation()
        {
            if(m_isFinished)
                return;
            m_isFinished = true;
            m_animator.SetTrigger(FinishUse);
        }

        public void StartMovementAgent()
        {
            movementAgent.StartAgent();
        }
        
        public void StopMovementAgent()
        {
            movementAgent.StopAgent();
        }
    }
}
