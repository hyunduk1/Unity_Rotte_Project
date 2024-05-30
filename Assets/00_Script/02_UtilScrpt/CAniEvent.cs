using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EANISTATE
{
    ROTATION = 0,
    MOVE = 1,
    SCALE = 2,
    ALPHA = 3,
    NONE = 4
};


[System.Serializable]
public struct ANITYPE
{
    public EANISTATE AniType;
    public float DelayTime;
    public float EventTime;
    public Vector2 StartValue;
    public Vector2 EndValue;
    public Vector3 StartValue3;
    public Vector3 EndValue3;
    public iTween.EaseType EasyType;
  
};

public class CAniEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public List<ANITYPE> _AniOutArray;
    public List<ANITYPE> _AniInArray;
    
    private CanvasGroup _CanvasGroup = null;


    public bool _bAutoOutEvent = true;
    void Awake()
    {
        _CanvasGroup = transform.GetComponent<CanvasGroup>();
    }
    void Start()
    {
            CheckAnimationInStation();
            if(_bAutoOutEvent)
                CheckAnimationOutStation();
    }

    void Update()
    {

    }

    public void CheckAnimationEventStation()
    {
      

        for (int i = 0; i < _AniInArray.Count; i++)
        {
            switch (_AniInArray[i].AniType)
            {
                case EANISTATE.ROTATION:
                    ItweenEventStart("EventRotationUpdate", "EventRotationComplete", _AniInArray[i]);
                    break;

                case EANISTATE.MOVE:
                    ItweenEventStart("EventMoveUpdate", "EventMoveComplete", _AniInArray[i]);
                    break;
                case EANISTATE.ALPHA:
                    ItweenEventStart("AlphaUpdate", "AlphaComplete", _AniInArray[i]);
                    break;
                case EANISTATE.SCALE:
                    ItweenEventStart("ScaleUpdate", "ScaleComplete", _AniInArray[i]);
                    break;
                case EANISTATE.NONE:
                    break;
            }
        }
    }


    public void CheckAnimationInStation()
    { 
    
        for (int i = 0; i < _AniInArray.Count; i++)
        {
            switch (_AniInArray[i].AniType)
            {
                case EANISTATE.ROTATION:
                    ItweenEventStart("EventRotationUpdate", "EventRotationComplete", _AniInArray[i]);
                    break;
                case EANISTATE.MOVE:
                    ItweenEventStart("EventMoveUpdate", "EventMoveComplete", _AniInArray[i]);
                    break;
                case EANISTATE.ALPHA:
                    ItweenEventStart("AlphaUpdate", "AlphaComplete", _AniInArray[i]);
                    break;
                case EANISTATE.SCALE:
                    ItweenEventStart("ScaleUpdate", "ScaleComplete", _AniInArray[i]);
                    break;
                case EANISTATE.NONE:
                    break;
            }
        }
    }
    public void CheckAnimationOutStation()
    {
      
        for (int i = 0; i < _AniOutArray.Count; i++)
        {
            switch (_AniOutArray[i].AniType)
            {
                case EANISTATE.ROTATION:
                    ItweenEventStart("EventRotationUpdate", "EventRotationComplete", _AniOutArray[i]);
                    break;
                case EANISTATE.MOVE:
                    ItweenEventStart("EventMoveUpdate", "EventMoveComplete", _AniOutArray[i]);
                    break;
                case EANISTATE.ALPHA:
                    ItweenEventStart("AlphaUpdate", "AlphaComplete", _AniOutArray[i]);
                    break;
                case EANISTATE.SCALE:
                    ItweenEventStart("ScaleUpdate", "ScaleComplete", _AniOutArray[i]);
                    break;
                case EANISTATE.NONE:
                    break;
            }
        }
    }
    //-----------------------------------------------------
    // [00] 회전처리
    //-----------------------------------------------------
    public void EventRotationUpdate(Vector3 vecValue)
    {
        transform.localRotation = Quaternion.Euler(vecValue);
    }
    public void EventRotationComplete()
    {

    }
    //-----------------------------------------------------
    // [00] 이동처리
    //-----------------------------------------------------
    public void EventMoveUpdate(Vector2 vecValue)
    {
        RectTransform rectTransformText = transform.GetComponent<RectTransform>();
        Vector2 tempVector2 = new Vector2(vecValue.x, vecValue.y);
        rectTransformText.anchoredPosition = tempVector2;
    }
    public void EventMoveComplete()
    {
        
      //  RectTransform rectTransformText = transform.GetComponent<RectTransform>();

        // Vector2 tempVector2 = new Vector2(0.0f, -5);

        //   rectTransformText.anchoredPosition = tempVector2;

    }


    //-----------------------------------------------------
    // [01] 알파처리
    //-----------------------------------------------------
    public void AlphaUpdate(float fAlpha)
    {
        if (_CanvasGroup != null){
            _CanvasGroup.alpha = fAlpha;
        }
    }
    public void AlphaComplete()
    {

    }

    //-----------------------------------------------------
    // [02] 스케일 처리
    //-----------------------------------------------------
    public void ScaleUpdate(Vector2 vecValue)
    {
        RectTransform rectTransformText = transform.GetComponent<RectTransform>();
        rectTransformText.sizeDelta = vecValue;
    }

    public void ScaleComplete()
    {

    }
    //--------------------------------------------------------------------------------------------------------------------
    //현덕 
    /*public void ItweenScaleEventStart(ANITYPE anyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash(
            "from", anyType.StartValue.x, "to", anyType.EndValue.x), "time", anyType.EventTime, "delay", anyType.DelayTime,
            "easytype", anyType.EasyType.ToString(),"onUpdate","" ,"onComplete", "");
    }*/


    //--------------------------------------------------------------------------------------------------------------------
    public void ItweenEventStart(string strUpdetName, string strCompleteName, Vector3 fValueA, Vector3 fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }

    public void ItweenEventStart(string strUpdetName, string strCompleteName, Vector2 fValueA, Vector2 fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }

    public void ItweenEventStart(string strUpdetName, string strCompleteName, float fValueA, float fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }

    public void ItweenEventStart(string strUpdetName, string strCompleteName, ANITYPE anyType)
    {
        if (anyType.AniType == EANISTATE.ALPHA)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
            "from", anyType.StartValue.x, "to", anyType.EndValue.x, "time", anyType.EventTime, "delay", anyType.DelayTime,
            "easetype", anyType.EasyType.ToString(), "onUpdate", strUpdetName, "oncomplete", strCompleteName));
        }
        else if(anyType.AniType == EANISTATE.ROTATION)
        {
            iTween.ValueTo(gameObject, iTween.Hash(
          "from", anyType.StartValue3, "to", anyType.EndValue3, "time", anyType.EventTime, "delay", anyType.DelayTime,
          "easetype", anyType.EasyType.ToString(), "onUpdate", strUpdetName, "oncomplete", strCompleteName));

        }
        else
        {
            iTween.ValueTo(gameObject, iTween.Hash(
            "from", anyType.StartValue, "to", anyType.EndValue, "time", anyType.EventTime, "delay", anyType.DelayTime,
            "easetype", anyType.EasyType.ToString(), "onUpdate", strUpdetName, "oncomplete", strCompleteName));
        }
    }
}
