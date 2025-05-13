using Random = UnityEngine.Random;

namespace Sinj
{
    [CreateAssetMenu(fileName = "SinjBehavior", menuName = "SinjBehavior/PassiveBehavior", order = 1)]
    public class SinjPassivBehavior : SinjBehavior
    {
        #region Reaction
        [Serializable]
        public class RestReaction : SinjReaction
        {
            [SerializeField, Range(.5f, 5)] private float minTime = .5f, maxTime = 5f;

            private Timer m_timer;
            private bool m_isFinished;
            public override void ApplyReaction(SinjAgent agent)
            {
                m_isFinished = false;

                m_timer = new Timer(Random.Range(minTime, maxTime) * 1000);
                m_timer.AutoReset = false;
                m_timer.Elapsed += (_, _) =>
                {
                    m_isFinished = true;
                    m_timer.Stop();
                };
                m_timer.Start();
                agent.Rest(minTime, maxTime);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                return m_isFinished;
            }
        }

        [Serializable]
        public class WalkReaction : SinjReaction
        {
            [SerializeField] private float distance;
            public override void ApplyReaction(SinjAgent agent)
            {
                agent.Walk(distance);
            }

            public override bool IsFinished(SinjAgent agent)
            {
                if (agent.IsCloseToDestination())
                {
                    WwisePostEvents.Instance.PostRandomMoodEvent(agent.gameObject);
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}