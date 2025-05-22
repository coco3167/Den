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
            float value = Vector3.Distance(smartObject.usingPoint.position, GameManager.Instance.worldParameters.GetMousePositon()) * smartAgent.dynamicParameters[AgentDynamicParameter.Hide].GetFloatValue();
            return value;
        }
    }

    public class SpotRepos : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            float value = Vector3.Distance(smartObject.usingPoint.position, smartAgent.transform.position) * smartAgent.dynamicParameters[AgentDynamicParameter.Tiredness] - smartObject.dynamicParameters[SmartObjectParameter.Usage];
            return value;
        }
    }
}
