using System.Collections;
using Options;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class CloseCaptionningManager : MonoBehaviour
{
    public static CloseCaptionningManager Instance { get; private set; }

    [SerializeField] private GameObject[] closeCaptionObjects;
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUIs;

    private Coroutine[] m_lastCoroutine = {null, null, null};

    private void Awake()
    {
        closeCaptionObjects.ForEach(x => x.SetActive(false));
        Instance = this;
    }

    public void ShowCloseCaption(string content, int momoIndex, float time = 2)
    {
        if(!GameParameters.HasCloseCaptions)
            return;
        
        closeCaptionObjects[momoIndex].SetActive(true);
        textMeshProUGUIs[momoIndex].text = content;
        if(m_lastCoroutine[momoIndex] != null)
            StopCoroutine(m_lastCoroutine[momoIndex]);
        m_lastCoroutine[momoIndex] = StartCoroutine(HideCloseCaption(momoIndex, time));
    }

    private IEnumerator HideCloseCaption(int momoIndex, float time)
    {
        yield return new WaitForSeconds(time);
        closeCaptionObjects[momoIndex].SetActive(false);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
