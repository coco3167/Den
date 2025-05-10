
using System.Collections.Generic;
using UnityEngine;

namespace Sinj
{
    public class StateMachine
    {
        public SinjBehavior CurrentBehavior { get; private set; }

        public void ResetBehavior(SinjAgent agent)
        {
            if(CurrentBehavior  && CurrentBehavior.IsFinished(agent))
                CurrentBehavior = null;
        }

        public bool HasBehavior(bool active = false)
        {
            return active ? CurrentBehavior is SinjActiveBehavior : CurrentBehavior;
        }

        public void ChangeBehavior(SinjBehavior behavior)
        {
            if (CurrentBehavior == behavior)
            {
                Debug.Log(behavior);
                return;
            }

            CurrentBehavior = behavior;
        }

        public void ChoosePassivBehavior(List<SinjPassivBehavior> passivBehaviors)
        {
            int index = Random.Range(0, passivBehaviors.Count);
            CurrentBehavior = passivBehaviors[index];
        }

        public override string ToString()
        {
            if (CurrentBehavior)
                return CurrentBehavior.ToString();
            return "None";
        }
    }
}
