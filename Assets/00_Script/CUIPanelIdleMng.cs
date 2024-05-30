using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DemolitionStudios.DemolitionMedia;

public class CUIPanelIdleMng : MonoBehaviour
{
    public Media m_MediaPlayer;
    public string _FolderName;
    public string _FileName;
    private RectTransform m_RectTransform;
    private static CUIPanelIdleMng _instance;
    public static CUIPanelIdleMng Instance { get { return _instance; } }

    private CanvasGroup m_CanvasGroup;

    public Vector3     _OutSidePosition;
    public GameObject[] _ButtonGroyup;
   // public Media        _Media;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
        _FileName = "00_IDLE_0" + CConfigMng.Instance._nContentsType.ToString() + ".mov";
        CheckContentsType();
    }

    void Start()
    {

    }
    private void CheckContentsType()
    {
        m_MediaPlayer.Loops = 1;
        m_MediaPlayer.openOnStart = true;
        //url º¯°æ
        //m_MediaPlayer.urlType = Media.UrlType.RelativeToPeristentPath;
        m_MediaPlayer.mediaUrl = _FolderName + "/" + _FileName;
        m_MediaPlayer.Play();
      
        _ButtonGroyup[CConfigMng.Instance._nContentsType].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableIDLEWindow()
    {

        transform.localPosition = new Vector3(0.0f,0.0f , 1.0f);

        CMainMng.Instance.EnalbeDonotTouchWindow(true);
        ItweenEventStart("EventMoveUpdate", "FadeInComplete", 0.0f, 1.0f, CConfigMng.Instance._fTrasionsSpeed, 0.0f, iTween.EaseType.easeOutExpo);

    }
    public void DisableIDLEWindow()
    {
        CSoundMng.Instance.PlaySound(SOUND_NAME.BUTTON_DEFAULT.ToString());
        CMainMng.Instance.EnalbeDonotTouchWindow(true);
        CUIPanelMng.Instance.InsertNextEventPanel((int)WINOOW.PANEL_TOP);
        ItweenEventStart("EventMoveUpdate", "FadeOutComplete", 1.0f, 0.0f, CConfigMng.Instance._fTrasionsSpeed, 0.0f, iTween.EaseType.easeOutExpo);
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
        transform.localPosition = new Vector3(0.0f, 1920.0f, 1.0f);
    }
    public void ItweenEventStart(string strUpdetName, string strCompleteName, float fValueA, float fValueB, float fSpeed, float fDelay, iTween.EaseType easyType)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", fValueA, "to", fValueB, "time", fSpeed, "delay", fDelay, "easetype", easyType.ToString(),
        "onUpdate", strUpdetName, "oncomplete", strCompleteName));
    }
}

