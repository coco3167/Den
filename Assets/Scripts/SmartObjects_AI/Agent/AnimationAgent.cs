using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(MovementAgent))]
    public class AnimationAgent : MonoBehaviour
    {
        // ID for animator
        private static readonly int StartUse = Animator.StringToHash("StartUse");
        private static readonly int FinishUse = Animator.StringToHash("FinishUse");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Curiosity = Animator.StringToHash("Curiosity");
        private static readonly int Aggression = Animator.StringToHash("Aggression");
        private static readonly int Fear = Animator.StringToHash("Fear");

        [SerializeField] private Animator animator;
        private MovementAgent m_movementAgent;

        private bool m_isFinished = true;
        private bool m_adaptToMood = false;
        
        private void Awake()
        {
            m_movementAgent = GetComponent<MovementAgent>();
        }

        private void Update()
        {
            animator.SetFloat(Speed, m_movementAgent.GetSpeed());
        }

        public void SwitchMood(AgentDynamicParameter parameter)
        {
            if(!m_adaptToMood)
                return;
            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    animator.SetBool(Curiosity, true);
                    break;
                case AgentDynamicParameter.Aggression:
                    animator.SetBool(Aggression, true);
                    break;
                case AgentDynamicParameter.Fear:
                    animator.SetBool(Fear, true);
                    break;
            }
        }

        public void ResetMood()
        {
            animator.SetBool(Curiosity, false);
            animator.SetBool(Aggression, false);
            animator.SetBool(Fear, false);
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController, bool adaptToMood)
        {
            m_isFinished = false;
            
            m_adaptToMood = adaptToMood;
            ResetMood();
            
            animator.runtimeAnimatorController = animatorController;
            animator.SetTrigger(StartUse);
        }

        public void FinishUseAnimation()
        {
            if(m_isFinished)
                return;
            m_isFinished = true;
            animator.SetTrigger(FinishUse);
        }
    }
}
