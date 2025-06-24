using System;
using System.Collections;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SmartObjects_AI.Agent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SymbolManager : MonoBehaviour
{
    [Title("Main")]
    [SerializeField] private RectTransform baseCircle, bigModel;

    [SerializeField] private float transitionTime, showTime, timeAfterStinger;
    
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Sprite[]> percentagesTextures;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersImages;
    [SerializeField] private SerializedDictionary<AgentDynamicParameter, Image> parametersEffect;

    private Vector2 m_anchorMin, m_anchorMax, m_anchoredPosition, m_sizeDelta;

    private Sequence m_appear, m_showSymbols, m_effect, m_lastAppear;

    // Appearance
    [Title("Start Apparition")] [SerializeField]
    private float apparitionTime = 5;
    public bool hasAppeared { get; private set; }
    private bool m_startedAppearing;
    private Image m_baseColorImage;

    [Title("Blinking")]
    [SerializeField] private float blinkingTotalTime;
    [SerializeField] private int blinkingCount;

    private void Awake()
    {
        GameManager.Instance.pallierReached.AddListener(OnPallierReached);

        parametersImages.Values.ForEach(x => x.enabled = false);
        parametersEffect.Values.ForEach(x =>
        {
            x.color = Color.clear;
            x.enabled = false;
        });
        
        m_anchorMin = baseCircle.anchorMin;
        m_anchorMax = baseCircle.anchorMax;

        m_anchoredPosition = baseCircle.anchoredPosition;
        m_sizeDelta = baseCircle.sizeDelta;

        m_baseColorImage = baseCircle.GetComponent<Image>();
        m_baseColorImage.color = Color.clear;
    }

    public void Appear()
    {
        if(m_startedAppearing)
            return;

        m_startedAppearing = true;

        baseCircle.anchorMin = bigModel.anchorMin;
        baseCircle.anchorMax = bigModel.anchorMax;
        baseCircle.anchoredPosition = bigModel.anchoredPosition;
        baseCircle.sizeDelta = bigModel.sizeDelta;
        
        m_appear = DOTween.Sequence();
        
        m_appear.Append(m_baseColorImage.DOColor(Color.black, apparitionTime/2));
        m_appear.Append(baseCircle.DOAnchorPos(m_anchoredPosition, apparitionTime/2))
            .Join(baseCircle.DOAnchorMax(m_anchorMax, apparitionTime/2))
            .Join(baseCircle.DOAnchorMin(m_anchorMin, apparitionTime/2))
            .Join(baseCircle.DOSizeDelta(m_sizeDelta, apparitionTime/2));
        m_appear.OnComplete(() => hasAppeared = true);
        m_appear.Play();
    }

    public void LastAppearance(AgentDynamicParameter parameter)
    {
        Image effect = parametersEffect[parameter];
        
        m_effect = DOTween.Sequence();
        m_effect.Append(effect.DOColor(Color.white, transitionTime))
            .AppendInterval(showTime);

        m_effect.OnPlay(() =>
        {
            effect.color = Color.clear;
            effect.enabled = true;
        });
        m_effect.OnComplete(() =>
        {
            effect.color = Color.clear;
            effect.enabled = false;
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);
            Debug.Log("tried loading scene");
        });

        m_lastAppear = DOTween.Sequence();
        
        m_lastAppear
            .Append(baseCircle.DOAnchorMin(bigModel.anchorMin, transitionTime))
            .Join(baseCircle.DOAnchorMax(bigModel.anchorMax, transitionTime))
            .Join(baseCircle.DOAnchorPos(bigModel.anchoredPosition, transitionTime));

        m_lastAppear.OnComplete(() => m_effect.Play());

        m_lastAppear.Play();
    }

    private void OnPallierReached(AgentDynamicParameter parameter, int value)
    {
        Image image = parametersImages[parameter];

        m_showSymbols = DOTween.Sequence();
        m_showSymbols
            .AppendInterval(timeAfterStinger)
            .AppendCallback(() =>
            {
                image.sprite = percentagesTextures[parameter][value / GameManager.IntervalPallier];
                image.enabled = true;
            })
            .Append(image.DOColor(Color.clear, blinkingTotalTime / (2 * blinkingCount))
                .SetLoops(2 * blinkingCount, LoopType.Yoyo))
            .OnComplete(() => image.color = Color.black)
            .Play();

        // baseCircle.anchorMin = bigModel.anchorMin;
        // baseCircle.anchorMax = bigModel.anchorMax;
        // baseCircle.anchoredPosition = bigModel.anchoredPosition;
        // baseCircle.sizeDelta = bigModel.sizeDelta;

        // baseCircle.anchorMin = Vector2.zero;
        // baseCircle.anchorMax = Vector2.one;
        //
        // baseCircle.offsetMin = Vector2.zero;
        // baseCircle.offsetMax = Vector2.zero;

        // m_effect.Restart();

    }
    
        /*m_showSymbols
            .Append(baseCircle.DOAnchorMin(bigModel.anchorMin, transitionTime))
            .Join(baseCircle.DOAnchorMax(bigModel.anchorMax, transitionTime))
            .Join(baseCircle.DOAnchorPos(bigModel.anchoredPosition, transitionTime));
            //.Join(baseCircle.DOSizeDelta(bigModel.sizeDelta, transitionTime));

        m_showSymbols.AppendInterval(showTime);
        
        m_showSymbols.Append(baseCircle.DOAnchorPos(m_anchoredPosition, transitionTime))
            .Join(baseCircle.DOAnchorMax(m_anchorMax, transitionTime))
            .Join(baseCircle.DOAnchorMin(m_anchorMin, transitionTime))
            .Join(baseCircle.DOSizeDelta(m_sizeDelta, transitionTime));*/
    
}
