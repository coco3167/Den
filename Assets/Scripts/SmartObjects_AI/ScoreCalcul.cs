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
            Value = Vector3.Distance(smartObject.usingPoint.position, GameManager.Instance.worldParameters.GetMousePositon()) * smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion];
            return Value;
        }
    }

    public class Rest : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Value = (smartAgent.dynamicParameters[AgentDynamicParameter.Tiredness] / Vector3.Distance(smartObject.usingPoint.position, smartAgent.transform.position));
            // if (smartAgent.IsUsing(smartObject))
            // {
            //     Debug.Log("Rest" + Value);
            //     return Value;
            // }

            // Debug.Log(Vector3.Distance(smartObject.usingPoint.position, smartAgent.transform.position));
            // Debug.Log(smartAgent);
            
            // TODO changer ça Value *= ((smartObject.dynamicParameters[SmartObjectParameter.Usage].GetBoolValue() ? 0 : 1));
            // !!!!
            
            // Debug.Log(smartObject.dynamicParameters[SmartObjectParameter.Usage].GetBoolValue());
            
            return Value;
        }
    }

    public class Jump : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float suspiscion = smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion];

            Value = 10 / Vector3.Distance(agentPos, mousePos) - suspiscion;
            return Value;
        }
    }

    public class Flee : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            Vector3 agentPos = smartAgent.transform.position;
            Vector3 mousePos = GameManager.Instance.worldParameters.GetMousePositon();
            float suspiscion = smartAgent.dynamicParameters[AgentDynamicParameter.Suspicion];
            Debug.Log(Vector3.Distance(agentPos, mousePos));
            Value = Mathf.Clamp(2 - Vector3.Distance(agentPos, mousePos),0,5)*100;
            Debug.Log("Flee" + Value);
            return Value;
        }
    }
}
