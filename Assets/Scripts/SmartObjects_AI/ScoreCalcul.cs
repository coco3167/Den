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
            Value = Vector3.Distance(smartObject.usingPoint.position, GameManager.Instance.worldParameters.GetMousePositon()) * smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion].GetFloatValue();
            return Value;
        }
    }

    public class Rest : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Value = (smartAgent.dynamicParameters[AgentDynamicParameter.Tiredness].GetFloatValue() - Vector3.Distance(smartObject.usingPoint.position, smartAgent.transform.position)) * 1 - (smartObject.dynamicParameters[SmartObjectParameter.Usage].GetBoolValue() ? 0 : 1);
            return Value;
        }
    }

    public class Jump : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float suspiscion = smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion].GetFloatValue();

            Value = 10 - Vector3.Distance(agentPos, mousePos) - suspiscion;
            return Value;
        }
    }
}
