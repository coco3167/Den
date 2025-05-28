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

        [SerializeField] private Animator animator;
        private MovementAgent m_movementAgent;

        private bool m_isFinished = true;
        
        private void Awake()
        {
            m_movementAgent = GetComponent<MovementAgent>();
        }

        private void Update()
        {
            animator.SetFloat(Speed, m_movementAgent.GetSpeed());
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController)
        {
            m_isFinished = false;
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
