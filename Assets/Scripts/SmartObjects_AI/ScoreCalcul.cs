using System;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    
    [Serializable]
    public abstract class BaseScoreCalcul
    {
        protected MouseManager p_mouseManager;
        protected float p_usingCapacity, p_distanceCoefficient;

        public void Init()
        {
            p_mouseManager = GameManager.Instance.GetMouseManager();
        }

        public virtual float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            p_usingCapacity = !smartObject.IsUsing(smartAgent) && !smartObject.HasRoomForUse() ? 0 : 1;
            p_distanceCoefficient = smartObject.DistanceCoefficient(smartAgent);
            return 0;
        }
    }
    
    public class EatScore : BaseScoreCalcul
    {
        private float m_hunger;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            
            m_hunger = smartAgent.GetDynamicParameter(AgentDynamicParameter.Hunger)/10;
            
            return p_usingCapacity * m_hunger * p_distanceCoefficient;
        }
    }
    
    public class RestScore : BaseScoreCalcul
    {
        private float m_tiredness;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            
            m_tiredness = smartAgent.GetDynamicParameter(AgentDynamicParameter.Tiredness)/10;
            
            return p_usingCapacity * m_tiredness * p_distanceCoefficient;
        }
    }
    
    public class FleePointCuriosity : BaseScoreCalcul
    {
        private float m_mousePlayerProximity;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position);
            m_mousePlayerProximity *= m_mousePlayerProximity;
            
            if (smartAgent.IsGoing(smartObject))
            {
                return Math.Max(1, smartAgent.GetDynamicParameter(AgentDynamicParameter.Curiosity)) * Math.Max(p_distanceCoefficient, 1/m_mousePlayerProximity);
            }
            
            return 10 / m_mousePlayerProximity;
        }
    }
    
    public class FleePointAggression : BaseScoreCalcul
    {
        private float m_mousePlayerProximity;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position);
            m_mousePlayerProximity *= m_mousePlayerProximity;
            
            if (smartAgent.IsGoing(smartObject))
            {
                return Math.Max(1, smartAgent.GetDynamicParameter(AgentDynamicParameter.Aggression)) * Math.Max(p_distanceCoefficient, 1/m_mousePlayerProximity);
            }
            
            return 10 / m_mousePlayerProximity;
        }
    }

    public class JumpScareScore : BaseScoreCalcul
    {
        private float m_mousePlayerProximity, m_usageCoeff;
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position);
            m_mousePlayerProximity *= m_mousePlayerProximity;

            m_usageCoeff = smartObject.GetDynamicParameter(SmartObjectParameter.Usage) > 90 ? 1.1f : 0;
            
            return 10 * m_usageCoeff / m_mousePlayerProximity;
        }
    }

    public class GroomingScore : BaseScoreCalcul
    {
        private float m_dirtiness;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (smartAgent.IsOwner(smartObject) || !smartObject.IsUsable)
                return 0;
            
            base.CalculateScore(smartAgent, smartObject);
            
            
            m_dirtiness = smartObject.GetDynamicParameter(SmartObjectParameter.Dirtiness)/10;
            
            return p_usingCapacity * m_dirtiness * p_distanceCoefficient;
        }
    }

    public class FightScore : BaseScoreCalcul
    {
        private float m_agentFight;
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (smartAgent.IsOwner(smartObject))
                return 0;
            
            base.CalculateScore(smartAgent, smartObject);

            m_agentFight = smartAgent.GetDynamicParameter(AgentDynamicParameter.Fight)/10;

            return p_usingCapacity * m_agentFight * p_distanceCoefficient;
        }
    }

    public class Hideout : BaseScoreCalcul
    {
        private float m_agentFear;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);

            m_agentFear = Math.Max(smartAgent.GetDynamicParameter(AgentDynamicParameter.UsableFear), smartAgent.GetDynamicParameter(AgentDynamicParameter.Fear))/10;

            return p_usingCapacity * m_agentFear * p_distanceCoefficient;
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
