using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(Animator), typeof(MovementAgent))]
    public class AnimationAgent : MonoBehaviour
    {
        // ID for animator
        private static readonly int StartUse = Animator.StringToHash("StartUse");
        private static readonly int FinishUse = Animator.StringToHash("FinishUse");
        private static readonly int Speed = Animator.StringToHash("Speed");

        private Animator m_animator;
        private MovementAgent m_movementAgent;

        private bool m_isFinished = true;
        
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            m_movementAgent = GetComponent<MovementAgent>();
        }

        private void Update()
        {
            m_animator.SetFloat(Speed, m_movementAgent.GetSpeed());
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController)
        {
            m_isFinished = false;
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
    }
}
