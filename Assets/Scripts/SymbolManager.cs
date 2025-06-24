using System;
using System.Collections;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.UI;

public class SymbolManager : MonoBehaviour, IGameStateListener
{
    [SerializeField] private RectTransform baseCircle, bigModel;

    [SerializeField] private float transitionTime, showTime;
    
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Sprite[]> percentagesTextures;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersImages;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersEffect;

    private Vector2 m_anchorMin, m_anchorMax, m_anchoredPosition, m_sizeDelta;

    private Sequence m_showSymbols, m_effect;

    private void Awake()
    {
        GameManager.Instance.pallierReached.AddListener(OnPallierReached);

        parametersImages.Values.ForEach(x => x.enabled = false);
        parametersEffect.Values.ForEach(x =>
        {
            x.color = Color.clear;
            x.enabled = false;
        });
    }

    private void OnPallierReached(AgentDynamicParameter parameter, int value)
    {
        Image image = parametersImages[parameter];
        image.sprite = percentagesTextures[parameter][value / GameManager.IntervalPallier];
        image.enabled = true;

        Image effect = parametersEffect[parameter];
        
        m_effect = DOTween.Sequence();
        m_effect.Append(effect.DOColor(Color.white, transitionTime))
            .AppendInterval(showTime)
            .Append(effect.DOColor(Color.clear, transitionTime));

        m_effect.OnPlay(() =>
        {
            effect.color = Color.clear;
            effect.enabled = true;
        });
        m_effect.OnComplete(() =>
        {
            effect.color = Color.clear;
            effect.enabled = false;
        });

        // baseCircle.anchorMin = bigModel.anchorMin;
        // baseCircle.anchorMax = bigModel.anchorMax;
        // baseCircle.anchoredPosition = bigModel.anchoredPosition;
        // baseCircle.sizeDelta = bigModel.sizeDelta;
        
        // baseCircle.anchorMin = Vector2.zero;
        // baseCircle.anchorMax = Vector2.one;
        //
        // baseCircle.offsetMin = Vector2.zero;
        // baseCircle.offsetMax = Vector2.zero;
        
        m_effect.Restart();
        m_showSymbols.Restart();

        //StartCoroutine(HideEffect(effect));
    }

    private IEnumerator HideEffect(Image effect)
    {
        yield return new WaitForSeconds(5);
        effect.enabled = false;
        
        baseCircle.anchorMin = m_anchorMin;
        baseCircle.anchorMax = m_anchorMax;
        
        baseCircle.anchoredPosition = m_anchoredPosition;
        baseCircle.sizeDelta = m_sizeDelta;
    }

    public void OnGameReady(object sender, EventArgs eventArgs)
    {
        m_anchorMin = baseCircle.anchorMin;
        m_anchorMax = baseCircle.anchorMax;

        m_anchoredPosition = baseCircle.anchoredPosition;
        m_sizeDelta = baseCircle.sizeDelta;
        
        m_showSymbols = DOTween.Sequence();
        m_showSymbols.SetAutoKill(false);

        m_showSymbols
            .Append(baseCircle.DOAnchorMin(bigModel.anchorMin, transitionTime))
            .Join(baseCircle.DOAnchorMax(bigModel.anchorMax, transitionTime))
            .Join(baseCircle.DOAnchorPos(bigModel.anchoredPosition, transitionTime));
            //.Join(baseCircle.DOSizeDelta(bigModel.sizeDelta, transitionTime));

        m_showSymbols.AppendInterval(showTime);
        
        m_showSymbols.Append(baseCircle.DOAnchorPos(m_anchoredPosition, transitionTime))
            .Join(baseCircle.DOAnchorMax(m_anchorMax, transitionTime))
            .Join(baseCircle.DOAnchorMin(m_anchorMin, transitionTime))
            .Join(baseCircle.DOSizeDelta(m_sizeDelta, transitionTime));
    }

    public void OnGameEnded(object sender, EventArgs eventArgs)
    {
        //
    }
}
