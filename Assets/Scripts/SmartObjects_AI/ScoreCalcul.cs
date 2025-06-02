using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    public abstract class BaseScoreCalcul
    {
        protected float Value;
        public abstract float CalculateScore(SmartAgent smartAgent, SmartObject smartObject);
    }

    public class TestScoreCalcul : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            return 50;
        }
    }

    public class Hideout : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Value = Vector3.Distance(smartObject.usingPoint.position, GameManager.Instance.worldParameters.GetMousePositon()) * smartAgent.GetDynamicParameter(AgentDynamicParameter.Suspicion);
            return Value;
        }
    }

    public class Rest : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            float tiredness = smartAgent.GetDynamicParameter(AgentDynamicParameter.Tiredness);
            Value = tiredness / 2;
            return Value;
        }
    }

    public class Food : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            float hunger = smartAgent.GetDynamicParameter(AgentDynamicParameter.Hunger);
            Value = hunger / 1.5f;
            return Value;
        }
    }

    //MOUSE REACTIVE

    public class Jump : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float mouseSpeed = GameManager.Instance.worldParameters.GetMouseVelocity();
            float suspiscion = smartAgent.GetDynamicParameter(AgentDynamicParameter.Suspicion);


            Value = Mathf.Clamp(3 - Vector3.Distance(agentPos, mousePos), 0, 3) * mouseSpeed * 35 - suspiscion;
            Value = Mathf.Clamp(Value, 0, 100);
            return Value;
        }
    }

    public class Flee : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float fleeRange = 2f;
            float distance = Vector3.Distance(agentPos, mousePos);
            Value = (fleeRange - Mathf.Clamp(distance, 0, fleeRange)) * (100/fleeRange);
           
            
            
            return Value;
        }
    }

    public class CuriosityActive : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            


            float curiosityLevel = smartAgent.GetDynamicParameter(AgentDynamicParameter.Curiosity);


            Value = curiosityLevel * Vector3.Distance(agentPos, mousePos);
            Value = Mathf.Clamp(Value, 0, 100);


            return Value;
        }
    }
    
    public class AgressionActive : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            
            return Value;
        }
        
    }
}
