using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIPanel : MonoBehaviour
{
    private CanvasGroup m_CanvasGroup;
   
    public bool _bCheckAutoIdleMode = true;
    private GameObject _EventMotion;
 
    // Start is called before the first frame update
    void Start()
    {
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
      
        if (_bCheckAutoIdleMode == false)
            CUIPanelMng.Instance.SetIdleCheckTimeEnable(false);
        else
            CUIPanelMng.Instance.SetIdleCheckTimeEnable(true);
        
    }
    private void AutoIdleModeInsert()
    {
        CUIPanelMng.Instance.InsertNextEventPanel((int)WINOOW.PANEL_TOP);
        CUIPanelIdleMng.Instance.EnableIDLEWindow();
        CUIPanelMng.Instance.FadeOutNaviGation();
    }

    void Update()
    {

    }

    public void FadeInWindow()
    {
        CMainMng.Instance.EnalbeDonotTouchWindow(true);
        ItweenEventStart("EventMoveUpdate", "FadeInComplete", 0.0f, 1.0f, CConfigMng.Instance._fTrasionsSpeed, 0.0f, iTween.EaseType.easeOutExpo);
    }

    public void FadeOutWindow()
    {
        ItweenEventStart("EventMoveUpdate", "FadeOutComplete", 1.0f, 0.0f, CConfigMng.Instance._fTrasionsSpeed, 0.0f, iTween.EaseType.easeOutExpo);
    }





    public void EventMoveUpdate(float fValue)
    {
        m_CanvasGroup.alpha = fValue;
    }
    public void FadeInComplete()
    {
       // CMainMng.Instance.EnalbeDonotTouchWindow(false);
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

    //-------------------------------------------------------------------------------------------
    // 스타트버튼 클릭
    //-------------------------------------------------------------------------------------------

    public void EventButtonClick(string strWindowNumber)
    {
        CSoundMng.Instance.PlaySound(SOUND_NAME.BUTTON_DEFAULT.ToString());

        CUIPanelMng.Instance.InsertNextEventPanel(strWindowNumber);
        if(strWindowNumber== "01_PanelFirstQuesition")
        {
            if(CUIPanelMng.Instance.GetNaviGationPoint()==null)
                CUIPanelMng.Instance.InsertNaviGation();

            CUIPanelMng.Instance.DrawNaviInfoCategory(NAVI_STATE.STATE_CATEGORY_FIRST);
        }
        else if (strWindowNumber == "00_PanelTop")
        {
            CUIPanelMng.Instance.FadeOutNaviGation();
        }
    } 
}
