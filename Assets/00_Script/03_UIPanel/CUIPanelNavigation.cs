using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum NAVI_STATE
{
    STATE_CATEGORY_FIRST,
    STATE_CATEGORY_SECOND,
    STATE_CATEGORY_THIRD,
    STATE_CATEGORY_ANALYSYS,
    STATE_CATEGORY_RESULT,
    STATE_ERASE_CATEGORY    
}
public enum CATEGORY_BOJ
{
    NAVI_TOGGLE_BUTTON,
    HOME_BUTTON,
    CAZZLE_LOGO
}

public class CUIPanelNavigation : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;

    //  public Toggle[] _CategoryToggleArray;
    public CUIImageToggle[] _CategoryToggleImageArray;
    public GameObject[]     _CategoryObj;

    //public GameObject[] _EnableObject;
    //public GameObject[] _EnableObjectAll;
    //public GameObject[] _AnanlyObject;

 

    void Awake()
    {
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void EventMoveUpdate(float fValue)
    {
        m_CanvasGroup.alpha = fValue;
    }
    public void FadeInComplete()
    {

    }

    public void FadeOutComplete()
    {
        Destroy(gameObject);
    }

    public void ItweenEventStart(string strUpdetName, string strCompleteName, float fValueA, float fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }

    public void DrawNaviInfoCategory(NAVI_STATE state)
    {
        switch (state)
        {
            case NAVI_STATE.STATE_CATEGORY_FIRST:
                _CategoryToggleImageArray[0].IsOn(true);
                _CategoryToggleImageArray[1].IsOn(false);
                _CategoryToggleImageArray[2].IsOn(false);
                break;
            case NAVI_STATE.STATE_CATEGORY_SECOND:
                _CategoryToggleImageArray[0].IsOn(false);
                _CategoryToggleImageArray[1].IsOn(true);
                _CategoryToggleImageArray[2].IsOn(false);
                break;
            case NAVI_STATE.STATE_CATEGORY_THIRD:
                _CategoryToggleImageArray[0].IsOn(false);
                _CategoryToggleImageArray[1].IsOn(false);
                _CategoryToggleImageArray[2].IsOn(true);
                break;
            case NAVI_STATE.STATE_CATEGORY_ANALYSYS:
                _CategoryObj[(int)CATEGORY_BOJ.NAVI_TOGGLE_BUTTON].SetActive(false);
                _CategoryObj[(int)CATEGORY_BOJ.HOME_BUTTON].SetActive(false);
                _CategoryObj[(int)CATEGORY_BOJ.CAZZLE_LOGO].SetActive(true);
                break;
            case NAVI_STATE.STATE_CATEGORY_RESULT:
                _CategoryObj[(int)CATEGORY_BOJ.NAVI_TOGGLE_BUTTON].SetActive(false);
                _CategoryObj[(int)CATEGORY_BOJ.HOME_BUTTON].SetActive(true);
                _CategoryObj[(int)CATEGORY_BOJ.CAZZLE_LOGO].SetActive(true);
                break;
            case NAVI_STATE.STATE_ERASE_CATEGORY:                 
                break;
        }
    }
}