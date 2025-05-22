using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    public abstract class BaseScoreCalcul
    {
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
            float value = Vector3.Distance(smartObject.usingPoint.position, GameManager.Instance.worldParameters.GetMousePositon()) * smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion].GetFloatValue();
            return value;
        }
    }

    public class Rest : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            float value = (smartAgent.dynamicParameters[AgentDynamicParameter.Tiredness].GetFloatValue() - Vector3.Distance(smartObject.usingPoint.position, smartAgent.transform.position)) * 1 - (smartObject.dynamicParameters[SmartObjectParameter.Usage].GetBoolValue() ? 0 : 1);
            return value;
        }
    }

    public class Jump : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float suspiscion = smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion].GetFloatValue();

            float value = 10 - Vector3.Distance(agentPos, mousePos) - suspiscion;
            return value;
        }
    }
}
