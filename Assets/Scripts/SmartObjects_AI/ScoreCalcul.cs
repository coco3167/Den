using System;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    
    [Serializable]
    public abstract class BaseScoreCalcul
    {
        protected float Value;
        protected MouseManager p_mouseManager;
        protected float p_mouseObjectProximity;

        public void Init()
        {
            p_mouseManager = GameManager.Instance.GetMouseManager();
        }

        public virtual float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            p_mouseObjectProximity = p_mouseManager.ObjectDistanceToMouse(smartObject.usingPoint.position);
            return 0;
        }
    }
    
    public class EatScore : BaseScoreCalcul
    {
        private float m_hunger, m_distanceCoefficient;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            
            m_hunger = smartAgent.GetDynamicParameter(AgentDynamicParameter.Hunger);
            m_distanceCoefficient = smartObject.DistanceCoefficient(smartAgent);
            
            return m_hunger * m_distanceCoefficient * p_mouseObjectProximity / 100;
        }
    }
    
    public class SleepScore : BaseScoreCalcul
    {
        private float m_tiredness, m_distanceCoefficient;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            
            m_tiredness = smartAgent.GetDynamicParameter(AgentDynamicParameter.Tiredness);
            m_distanceCoefficient = smartObject.DistanceCoefficient(smartAgent);
            
            return m_tiredness * m_distanceCoefficient * p_mouseObjectProximity / 100;
        }
    }
    
    public class FleePoint : BaseScoreCalcul
    {
        [SerializeField] private float playerCoeff;

        private float m_distanceCoefficient, m_mousePlayerProximity;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            
            m_distanceCoefficient = smartObject.DistanceCoefficient(smartAgent);
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position);
            
            return playerCoeff*m_distanceCoefficient*p_mouseObjectProximity/(m_mousePlayerProximity*m_mousePlayerProximity);
        }
    }
    
    
    
    #region Deprecated
    
    /*public class TestScoreCalcul : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            return 20;
        }
    }

    public class FleeingPointDistance : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            return 1.5f;
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
            return 0;
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
            return 0;
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


            return 0;
        }
    }
    
    public class AgressionActive : BaseScoreCalcul
    {
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            
            return 0;
        }
        
    }*/
    
    #endregion
}
