using AYellowpaper.SerializedCollections;
using DG.Tweening;
using SmartObjects_AI.Agent;
using UnityEngine;

namespace PersonnalFolders.Paul.Scripts.VFX
{
    public class ThresholdFX : MonoBehaviour
    {
        private static readonly int Main = Shader.PropertyToID("_MAIN");
        private static readonly int Second = Shader.PropertyToID("_SECOND");
        private static readonly int Color1 = Shader.PropertyToID("_color");

        [SerializeField] private Material thresholdMat;
        [SerializeField] private  AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve secondAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float animTime = 1f;

        [SerializeField] private SerializedDictionary<AgentDynamicParameter, Color> emotionColors;
    
        private Tween m_firstTween, m_secondTween;

        private void Awake()
        {
            m_firstTween.SetEase(animCurve);
            m_secondTween.SetEase(secondAnimCurve);
            m_firstTween.SetAutoKill(false);
            m_secondTween.SetAutoKill(false);

            m_firstTween = thresholdMat.DOFloat(1f, Main, animTime);
            m_secondTween = thresholdMat.DOFloat(1f, Second, animTime);
            
            m_firstTween.isBackwards = true;
            m_secondTween.isBackwards = true;

            m_firstTween.onComplete += () =>
            {
                m_firstTween.Rewind();
            };
            m_secondTween.onComplete += () =>
            {
                m_secondTween.Rewind();
            };
            //m_firstTween.onComplete += () => thresholdMat.SetFloat(Main, 0);
            //m_secondTween.onComplete += () => thresholdMat.SetFloat(Second, 0);
            
            m_firstTween.Pause();
            m_secondTween.Pause();
        
            GameManager.Instance.pallierReached.AddListener(TryPlayFX);
        }

        private void TryPlayFX(AgentDynamicParameter parameter, int value)
        {
            thresholdMat.SetColor(Color1, emotionColors[parameter]);
            PlayFX();
        }

        private void PlayFX()
        {
            Debug.Log("vfx");
            
            m_firstTween.Restart();
            m_secondTween.Restart();
        }

        private void OnDestroy()
        {
            thresholdMat.SetFloat(Main, 0);
            thresholdMat.SetFloat(Second, 0);
        }
    }
}
