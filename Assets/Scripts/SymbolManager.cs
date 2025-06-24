using System.Collections;
using AYellowpaper.SerializedCollections;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.UI;

public class SymbolManager : MonoBehaviour
{
    [SerializeField] private RectTransform baseCircle;
    
    [SerializeField] private Sprite[] percentagesTextures;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersImages;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersEffect;

    private Vector2 m_anchorMin, m_anchorMax, m_offsetMin, m_offsetMax;

    private void Awake()
    {
        GameManager.Instance.pallierReached.AddListener(OnPallierReached);

        parametersImages.Values.ForEach(x => x.enabled = false);
        parametersEffect.Values.ForEach(x => x.enabled = false);

        m_anchorMin = baseCircle.anchorMin;
        m_anchorMax = baseCircle.anchorMax;

        m_offsetMin = baseCircle.offsetMin;
        m_offsetMax = baseCircle.offsetMax;
        
        
    }

    private void OnPallierReached(AgentDynamicParameter parameter, int value)
    {
        Image image = parametersImages[parameter];
        image.sprite = percentagesTextures[value / GameManager.IntervalPallier];
        image.enabled = true;

        Image effect = parametersEffect[parameter];
        effect.enabled = true;
        
        baseCircle.anchorMin = Vector2.zero;
        baseCircle.anchorMax = Vector2.one;
        
        baseCircle.offsetMin = Vector2.zero;
        baseCircle.offsetMax = Vector2.zero;

        StartCoroutine(HideEffect(effect));
    }

    private IEnumerator HideEffect(Image effect)
    {
        yield return new WaitForSeconds(5);
        effect.enabled = false;
        
        baseCircle.anchorMin = m_anchorMin;
        baseCircle.anchorMax = m_anchorMax;
        
        baseCircle.offsetMin = m_offsetMin;
        baseCircle.offsetMax = m_offsetMax;
    }
}
