using UnityEngine;

namespace SmartObjects_AI.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AnimationAgent : MonoBehaviour
    {
        private Animator m_animator;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void SwitchAnimator(RuntimeAnimatorController animatorController)
        {
            m_animator.runtimeAnimatorController = animatorController;
        }
    }
}
