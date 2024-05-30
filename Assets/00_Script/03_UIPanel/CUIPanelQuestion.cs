using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum BUTTON_STATE {  DEFAULT_ANI,LEFT_CLICK_ANI,RIGHT_CLICK_ANI}

public class CUIPanelQuestion : MonoBehaviour
{
    private Animator m_Animator;

    public GameObject[] _ButtonNode;
    public GameObject _ParentNode;

    public GameObject _InsertVideoNode;

    public float _InsertAniTime = 0.0f;

    public GameObject[] _TextEnable;
 //   public CAniEvent[] _AniEvent;
    public Image[] _ButtonImage;
    public GameObject[] _ScaleNode;
    private BUTTON_STATE m_ButtonState;
    private bool m_ChangeAniState = false;
    
    public float m_ChangeAniSpeed = 1.0f;
    public float _RePosintSpeed = 0.2f;
    public float _NextPageSpeed = 1.5f;
    private bool m_AniStop = false;

    public float _fFadeAlpha = 0.0f;

    public CATEGORY_TYPE m_nCategory;
    public bool m_bTypeButton = false;

    public bool m_bCheckButton = false;

    public Image[] _ParentImageAlpha;
    // Start is called before the first frame update
    void Start()
    {
        int byRandom = Random.Range(0, 2);
        if (byRandom == 0){
            _TextEnable[0].SetActive(true);
            _TextEnable[2].SetActive(true);
        }
        else {
            _TextEnable[1].SetActive(true);
            _TextEnable[3].SetActive(true);

        }

        m_Animator = transform.GetComponent<Animator>();

        Invoke("InsertFirstAniVideo", _InsertAniTime);
       // m_Animator.enabled=false;
       // ItweenEventStart("TranstionLeftUpUpdate", "TransitionComplete", _fFadeAlpha, 255.0f, m_ChangeAniSpeed, 0.2f, iTween.EaseType.linear);
     //   Invoke("StartAnimation",1.0f);
    }

    public void StartAnimation()
    {
        m_Animator.enabled = false;
        ItweenEventStart("TranstionLeftUpUpdate", "TransitionComplete", _fFadeAlpha, 255.0f, m_ChangeAniSpeed, 0.0f, iTween.EaseType.linear);

    }
    // Update is called once per frame
    void Update()
    {

        return;
        /*
        switch(m_ButtonState)
        {
            case BUTTON_STATE.DEFAULT_ANI:
                ChangeButtonAnimation();
                break;
            case BUTTON_STATE.LEFT_CLICK_ANI:
                break;
            case BUTTON_STATE.RIGHT_CLICK_ANI:
                break;
        }*/
    }


   
    public void TranstionLeftUpUpdate(float fValue)
    {
        if (m_AniStop == true)
            return;

        float fPersent = fValue / 255.0f;

        float fChangeValue = fValue - _fFadeAlpha;
        _ButtonImage[0].color = new Color(
              _ButtonImage[0].color.r,
              _ButtonImage[0].color.g,
              _ButtonImage[0].color.b,
              fValue / 255.0f);

        Vector3 tempScale = _ScaleNode[0].transform.localScale;
        float fScale = 1.0f+(0.02f * fPersent);
        _ScaleNode[0].transform.localScale= new Vector3(fScale, fScale, 1.0f);

        if (bFirst == true)
            return;
        fValue = 255.0f-fChangeValue;
        _ButtonImage[1].color = new Color(
            _ButtonImage[1].color.r,
            _ButtonImage[1].color.g,
            _ButtonImage[1].color.b,
        fValue / 255.0f);
      
        tempScale = _ScaleNode[1].transform.localScale;
        fScale = 1.02f - (0.02f * fPersent);
         if (tempScale.x == 1.0f)
            return;
        _ScaleNode[1].transform.localScale = new Vector3(fScale, fScale, 1.0f);
    }
    bool bFirst = true;
    public void TranstionLeftDownUpdate(float fValue)
    {
        if (m_AniStop == true)
            return;

        float fPersent = fValue / 255.0f;

        float fChangeValue = fValue - _fFadeAlpha;
        _ButtonImage[1].color = new Color(
              _ButtonImage[1].color.r,
              _ButtonImage[1].color.g,
              _ButtonImage[1].color.b,
              fValue / 255.0f);


        Vector3 tempScale = _ScaleNode[1].transform.localScale;
        float fScale = 1.0f + (0.02f * fPersent);
        _ScaleNode[1].transform.localScale = new Vector3(fScale, fScale, 1.0f);

        fValue = 255.0f - fChangeValue;
        _ButtonImage[0].color = new Color(
                _ButtonImage[0].color.r,
                _ButtonImage[0].color.g,
                _ButtonImage[0].color.b,
                fValue / 255.0f
         );

        tempScale = _ScaleNode[0].transform.localScale;
        fScale = 1.02f - (0.02f * fPersent);
        _ScaleNode[0].transform.localScale = new Vector3(fScale, fScale, 1.0f);
      
    }
    public void TransitionComplete()
    {
        bFirst = false;
        if (m_AniStop == true)
            return;

        if (m_ChangeAniState == false)
        {
            m_ChangeAniState = true;
            ItweenEventStart("TranstionLeftDownUpdate", "TransitionComplete", _fFadeAlpha, 255.0f, m_ChangeAniSpeed, 0.0f, iTween.EaseType.easeOutExpo);
        }
        else
        {
            m_ChangeAniState = false;
            ItweenEventStart("TranstionLeftUpUpdate", "TransitionComplete", _fFadeAlpha, 255.0f, m_ChangeAniSpeed, 0.0f, iTween.EaseType.easeOutExpo);
        }
    }

    bool m_bEventCurrent = false;
    float fLeftButtonAlpha = 0.0f;
    float fRightButtonAlpha = 0.0f;
    public void LeftButtonClick()
    {
        if (m_bEventCurrent == true)
            return;
        m_bEventCurrent = true;
        CMainMng.Instance.DonotTouchWindow(true);


        CSoundMng.Instance.PlaySound(SOUND_NAME.BUTTON_ANSWER.ToString());
        m_bCheckButton = true;
   
        m_bTypeButton = false;
        m_AniStop = true;
       
        ItweenEventStart("ClickAniLeftAlphaUpdate", "ClickAniLeftScaleComplete",
            0.0f, 1.0f,
            _RePosintSpeed, 0.0f,
            iTween.EaseType.linear);
        
        fLeftButtonAlpha = _ButtonImage[0].color.a;
        fRightButtonAlpha = _ButtonImage[1].color.a;
        fLeftScale = _ScaleNode[0].transform.localScale.x;
        fRightScale = _ScaleNode[1].transform.localScale.x;
        _ButtonNode[0].transform.SetParent(_ParentNode.transform);
    }

    float fLeftScale = 0.0f;
    float fRightScale = 0.0f;
    public void RightButtonClick()
    { 
        if (m_bEventCurrent == true)
            return;
        m_bEventCurrent = true;

        CMainMng.Instance.DonotTouchWindow(true);
        CSoundMng.Instance.PlaySound(SOUND_NAME.BUTTON_ANSWER.ToString());
        m_bCheckButton = true;
      
        m_bTypeButton = true;
           m_AniStop = true;
 
        ItweenEventStart("ClickAniRightAlphaUpdate", "ClickAniRightScaleComplete",
                0.0f, 1.0f,
               _RePosintSpeed, 0.0f,
           iTween.EaseType.linear);
        fRightButtonAlpha = _ButtonImage[1].color.a;
        fLeftButtonAlpha = _ButtonImage[0].color.a;
        fLeftScale = _ScaleNode[0].transform.localScale.x;
        fRightScale = _ScaleNode[1].transform.localScale.x;
        _ButtonNode[1].transform.SetParent(_ParentNode.transform);
    }

    public void ClickAniLeftAlphaUpdate(float fValue)
    {
        m_AniStop = true;
        float fChangeValue = fValue - fLeftButtonAlpha;
        float fValueOk = Mathf.Lerp(fLeftButtonAlpha, 1.0f, fValue);
        _ButtonImage[0].color = new Color(
           _ButtonImage[0].color.r,
           _ButtonImage[0].color.g,
           _ButtonImage[0].color.b,
          fValueOk);

        float fScale = Mathf.Lerp(fLeftScale, 1.02f, fValue);
        _ScaleNode[0].transform.localScale = new Vector3(fScale, fScale, 1.0f);

        fValueOk = Mathf.Lerp(fRightButtonAlpha, 0.0f, fValue);
        _ButtonImage[1].color = new Color(
            _ButtonImage[1].color.r,
            _ButtonImage[1].color.g,
            _ButtonImage[1].color.b,
            fValueOk);

         fScale = Mathf.Lerp(fRightScale, 1.0f, fValue);
        _ScaleNode[1].transform.localScale = new Vector3(fScale, fScale, 1.0f);
    }
    public void ClickAniRightAlphaUpdate(float fValue)
    {
        m_AniStop = true;
        float fChangeValue = fValue - fRightButtonAlpha;
        float fValueOk = Mathf.Lerp(fRightButtonAlpha, 1.0f, fValue);
        _ButtonImage[1].color = new Color(
           _ButtonImage[1].color.r,
           _ButtonImage[1].color.g,
           _ButtonImage[1].color.b,
          fValueOk);

        float fScale = Mathf.Lerp(fRightScale, 1.02f, fValue);
        _ScaleNode[1].transform.localScale = new Vector3(fScale, fScale, 1.0f);

        fValueOk = Mathf.Lerp(fLeftButtonAlpha, 0.0f, fValue);
        _ButtonImage[0].color = new Color(
            _ButtonImage[0].color.r,
            _ButtonImage[0].color.g,
            _ButtonImage[0].color.b,
            fValueOk);

        fScale = Mathf.Lerp(fLeftScale, 1.0f, fValue);
        _ScaleNode[0].transform.localScale = new Vector3(fScale, fScale, 1.0f);
    }

    public void ClickAniRightScaleComplete()
    {
        //   return;
        m_Animator.enabled = true;
        m_Animator.SetInteger("ButtonState", 2);
        _ParentImageAlpha[1].color= new Color(
            _ParentImageAlpha[1].color.r,
            _ParentImageAlpha[1].color.g,
            _ParentImageAlpha[1].color.b,
            0.0f);

        switch (m_nCategory.Category)
        {
            case QUESTION_CATEGORY.FIRST_ANSWER:
                Invoke("InsertSecondWindow", _NextPageSpeed);
                break;
            case QUESTION_CATEGORY.SECOND_ANSER:
                Invoke("InsertThirdWindow", _NextPageSpeed);
                break;
            case QUESTION_CATEGORY.THIRD_ANSER:
                Invoke("InsertAnalyzingWindow", _NextPageSpeed);
                break;
        }
    }
    public void ClickAniLeftScaleComplete()
    {
        m_Animator.enabled = true;
        m_Animator.SetInteger("ButtonState", 1);
        _ParentImageAlpha[0].color = new Color(
         _ParentImageAlpha[0].color.r,
         _ParentImageAlpha[0].color.g,
         _ParentImageAlpha[0].color.b,
         0.0f);
        switch (m_nCategory.Category)
        {
            case QUESTION_CATEGORY.FIRST_ANSWER:
                Invoke("InsertSecondWindow", _NextPageSpeed);
                break;
            case QUESTION_CATEGORY.SECOND_ANSER:
                Invoke("InsertThirdWindow", _NextPageSpeed);
                break;
            case QUESTION_CATEGORY.THIRD_ANSER:
                Invoke("InsertAnalyzingWindow", _NextPageSpeed);
                break;
        }
    }

    public void ItweenEventStart(string strUpdetName, string strCompleteName, float fValueA, float fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }

   
    public void InsertAnalyzingWindow()
    {
        CUIPanelMng.Instance.ResetIdleCheckTime();
        CMainMng.Instance.SetQuestionType((byte)QUESTION_CATEGORY.THIRD_ANSER, m_bTypeButton);

        CUIPanelMng.Instance.DrawNaviInfoCategory(NAVI_STATE.STATE_CATEGORY_ANALYSYS);
        CUIPanelMng.Instance.InsertNextEventPanel("04_PanelAnalyzing");
    }
    public void InsertSecondWindow()
    {
        CMainMng.Instance.DonotTouchWindow(true);
        CUIPanelMng.Instance.ResetIdleCheckTime();
        CMainMng.Instance.SetQuestionType((byte)QUESTION_CATEGORY.FIRST_ANSWER, m_bTypeButton);
         CUIPanelMng.Instance.DrawNaviInfoCategory(NAVI_STATE.STATE_CATEGORY_SECOND);
        CUIPanelMng.Instance.InsertNextEventPanel("02_PanelSecondQuesition");
    }

    public void InsertThirdWindow()
    {
        CMainMng.Instance.DonotTouchWindow(true);
        CUIPanelMng.Instance.ResetIdleCheckTime();
        CMainMng.Instance.SetQuestionType((byte)QUESTION_CATEGORY.SECOND_ANSER, m_bTypeButton);

        CUIPanelMng.Instance.DrawNaviInfoCategory(NAVI_STATE.STATE_CATEGORY_THIRD);
        CUIPanelMng.Instance.InsertNextEventPanel("03_PanelThirdQuesition");
    }






    //왼쪽은 처음에 100 이다 오른쪽은 255
    private void ChangeButtonAnimation()
    {
        if(m_ChangeAniState==true)
        {
            //  0.39215
            float fAlpha = 0.0f;
            _ButtonImage[0].color = new Color(
                _ButtonImage[0].color.r,
                _ButtonImage[0].color.g, 
                _ButtonImage[0].color.b,
                fAlpha/255.0f);

            fAlpha = 0.0f;
            _ButtonImage[0].color = new Color(
                _ButtonImage[1].color.r,
                _ButtonImage[1].color.g,
                _ButtonImage[1].color.b,
                fAlpha / 255.0f);
        }
    }
    private void InsertFirstAniVideo()
    {
        if (m_bCheckButton == true)
            return;
    
        GameObject tempWindow;
        if (Random.Range(0, 2) == 0){
            tempWindow =MonoBehaviour.Instantiate(CUIPanelMng.Instance.GetPrefabsRef("12_LeftFirstAndRight")) as GameObject;
        }
        else
        {
    
            tempWindow =MonoBehaviour.Instantiate(CUIPanelMng.Instance.GetPrefabsRef("13_RightFirstAndLeft")) as GameObject;
        }

        tempWindow.transform.SetParent(_InsertVideoNode.transform);
        RectTransform rectTransform = tempWindow.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0.0f);
        rectTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        tempWindow.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
      //  tempWindow.GetComponent<CUIPanelQuestionVideo>().SetParentNode(gameObject,true);
    }

 
}