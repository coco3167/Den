using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AnimationAgent : MonoBehaviour
    {
        private static readonly int StartUse = Animator.StringToHash("StartUse");
        private static readonly int FinishUse = Animator.StringToHash("FinishUse");
        
        private Animator m_animator;

        private bool m_isFinished = true;
        
        private void Awake()
        {
            m_animator = GetComponent<Animator>();
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
