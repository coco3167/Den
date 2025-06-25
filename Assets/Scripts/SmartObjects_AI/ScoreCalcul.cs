using System;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace SmartObjects_AI
{
    
    [Serializable]
    public abstract class BaseScoreCalcul
    {
        protected MouseManager p_mouseManager;
        protected WorldParameters p_worldParameters;
        protected float p_usingCapacity, p_distanceCoefficient;

        public void Init()
        {
            p_mouseManager = GameManager.Instance.GetMouseManager();
            p_worldParameters = GameManager.Instance.worldParameters;
        }

        // public virtual void JumpScareUpdate(float jumpscareValue)
        // {
        //     //
        // }

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
            
            m_hunger = smartAgent.GetDynamicParameter(AgentDynamicParameter.Hunger)/10 * smartObject.GetDynamicParameter(SmartObjectParameter.Usage)/100;
            
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
        [SerializeField] private float proximityDecal;
        private float m_mousePlayerProximity;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position) + proximityDecal;
            m_mousePlayerProximity *= m_mousePlayerProximity;
            
            // if (smartAgent.IsGoing(smartObject))
            // {
            //     return Math.Max(1, smartAgent.GetDynamicParameter(AgentDynamicParameter.Curiosity)) * Math.Max(p_distanceCoefficient, 1/m_mousePlayerProximity);
            // }
            
            return 1 / m_mousePlayerProximity * smartAgent.GetDynamicParameter(AgentDynamicParameter.Curiosity);
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
                return Math.Max(10, smartAgent.GetDynamicParameter(AgentDynamicParameter.Aggression)) * Math.Max(p_distanceCoefficient, 1/m_mousePlayerProximity);
            }
            
            return 10 / m_mousePlayerProximity;
        }
    }

    public class JumpScareScore : BaseScoreCalcul
    {
        [SerializeField] private AgentDynamicParameter parameter;
        
        private float m_mousePlayerProximity, m_emotion, m_usageCoeff;

        // public override void JumpScareUpdate(float jumpscareValue)
        // {
        //     m_usageCoeff = jumpscareValue > 90 ? 1.1f : 0;
        // }

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            m_mousePlayerProximity = p_mouseManager.ObjectDistanceToMouse(smartAgent.transform.position);
            m_mousePlayerProximity *= m_mousePlayerProximity;

            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    m_emotion = smartAgent.GetDynamicParameter(AgentDynamicParameter.Curiosity)/100;
                    break;
                
                case AgentDynamicParameter.Aggression:
                    m_emotion = smartAgent.GetDynamicParameter(AgentDynamicParameter.Aggression) / 100;
                    break;
                
                default:
                    m_emotion = smartAgent.GetDynamicParameter(AgentDynamicParameter.Fear) / 100;
                    break;
            }

            m_usageCoeff = smartObject.GetDynamicParameter(SmartObjectParameter.Usage) > 90 ? 1.1f : 0;
            m_emotion = Math.Max(10, m_emotion);
            
            return m_usageCoeff * m_emotion / m_mousePlayerProximity;
        }
    }

    public class GroomingScore : BaseScoreCalcul
    {
        private float m_dirtiness;
        private float m_worldCuriosity;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (smartAgent.IsOwner(smartObject) || !smartObject.IsUsable)
                return 0;

            base.CalculateScore(smartAgent, smartObject);


            m_dirtiness = smartObject.GetDynamicParameter(SmartObjectParameter.Dirtiness) / 10;
            m_worldCuriosity = GameManager.Instance.worldParameters.AgentGlobalParameters[AgentDynamicParameter.Curiosity];
            m_worldCuriosity = m_worldCuriosity > 25 ? m_worldCuriosity/25 : 0;

            return p_usingCapacity * m_dirtiness * p_distanceCoefficient * m_worldCuriosity;
        }
    }

    public class FightScore : BaseScoreCalcul
    {
        private float m_agentFight;
        private float m_worldAggression;
        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (smartAgent.IsOwner(smartObject))
                return 0;

            base.CalculateScore(smartAgent, smartObject);

            m_agentFight = smartAgent.GetDynamicParameter(AgentDynamicParameter.Fight) / 10;
            m_worldAggression = GameManager.Instance.worldParameters.AgentGlobalParameters[AgentDynamicParameter.Aggression];
            m_worldAggression = m_worldAggression > 25 ? m_worldAggression/50 : 0;

            return p_usingCapacity * m_agentFight * p_distanceCoefficient * m_worldAggression;
        }
    }

    public class HideoutScore : BaseScoreCalcul
    {
        

        private float m_agentFear;
        private float m_worldFear;
        

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);

            m_agentFear = Math.Max(smartAgent.GetDynamicParameter(AgentDynamicParameter.UsableFear), smartAgent.GetDynamicParameter(AgentDynamicParameter.Fear)) / 10;
            m_worldFear = GameManager.Instance.worldParameters.AgentGlobalParameters[AgentDynamicParameter.Fear];

            if (smartAgent.IsGoing(smartObject))
            {
                p_distanceCoefficient *= 5;
            }

            return p_usingCapacity * m_agentFear * p_distanceCoefficient * (m_worldFear*0.8f+10) / 10;
        }
    }

    public class AgressionCapScore : BaseScoreCalcul
    {
        private float m_agressionCap;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            if (!smartAgent.IsOwner(smartObject))
                return 0;
            
            m_agressionCap = smartAgent.GetDynamicParameter(AgentDynamicParameter.AggressionCap);
            
            return 10 * m_agressionCap;
        }
    }

    public class EndScore : BaseScoreCalcul
    {
        [SerializeField] private AgentDynamicParameter parameter;
        private float m_score;

        public override float CalculateScore(SmartAgent smartAgent, SmartObject smartObject)
        {
            base.CalculateScore(smartAgent, smartObject);
            switch (parameter)
            {
                case AgentDynamicParameter.Curiosity:
                    m_score = p_worldParameters.GetDynamicParameter(WorldParameters.WorldParameterType.EndSleep);
                    break;
                
                case AgentDynamicParameter.Aggression:
                    m_score = p_worldParameters.GetDynamicParameter(WorldParameters.WorldParameterType.EndAggression);
                    break;
                
                case AgentDynamicParameter.Fear:
                    m_score = p_worldParameters.GetDynamicParameter(WorldParameters.WorldParameterType.EndFleeOuterSpace);
                    break;
            } 
            
            return 10 * m_score * p_distanceCoefficient;
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
