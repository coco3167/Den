
using System.Collections.Generic;
using UnityEngine;

namespace Sinj
{
    public class StateMachine
    {
        public SinjBehavior CurrentBehavior { get; private set; }

        public StateMachine()
        {
            ResetBehavior();
        }

        public void ResetBehavior()
        {
            CurrentBehavior = null;
        }

        public bool HasBehavior()
        {
            return CurrentBehavior;
        }

        public void ChangeBehavior(SinjBehavior behavior)
        {
            CurrentBehavior = behavior;
        }

        public void ChoosePassivBehavior(List<SinjPassivBehavior> passivBehaviors)
        {
            int index = Random.Range(0, passivBehaviors.Count);
            CurrentBehavior = passivBehaviors[index];
        }
    }
}
