using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DemolitionStudios.DemolitionMedia;

public class CMovieSyncPlayer : MonoBehaviour
{
    private static CMovieSyncPlayer _instance;
    public static CMovieSyncPlayer Instance { get { return _instance; } }

    public Media m_MediaPlayer;

    private CanvasGroup m_CanvasGroup;

    public string _FolderName;
    public string _FileName;

    private CNetWorkMng _NetWorkUDP;
    public Vector3 _OutSidePosition;
    public GameObject[] _ButtonGroyup;
    private void Awake()
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
        _FileName = "02_ForestAlpha" + ".mov";
        //_FileName = "00_IDLE_0" + CConfigMng.Instance._nContentsType.ToString() + ".mov";
        CheckContentsType();

        m_MediaPlayer.Loops = 1;
        m_MediaPlayer.openOnStart = true;
        m_MediaPlayer.playOnOpen = true;
        m_MediaPlayer.preloadToMemory = true;
        m_MediaPlayer.mediaUrl = _FolderName + "/" + _FileName;
        if (m_MediaPlayer != null) {
            m_MediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        }
    }
    
    void Start()
    {
        _NetWorkUDP = transform.GetComponent<CNetWorkMng>();
    }

   
    private void CheckContentsType()
    {
        

        _ButtonGroyup[CConfigMng.Instance._nContentsType].SetActive(true);
    }
    public void OnMediaPlayerEvent(Media source, MediaEvent.Type type, MediaError error)
    {

      
        switch (type) {
            case MediaEvent.Type.Closed: break;
            case MediaEvent.Type.OpeningStarted: break;
            case MediaEvent.Type.PreloadingToMemoryStarted: break;
            case MediaEvent.Type.PreloadingToMemoryFinished:
                PlayerPlay();
                break;
            case MediaEvent.Type.Opened: break;
            case MediaEvent.Type.OpenFailed: break;
            case MediaEvent.Type.VideoRenderTextureCreated: break;
            case MediaEvent.Type.PlaybackStarted: break;
            case MediaEvent.Type.PlaybackStopped: break;
            case MediaEvent.Type.PlaybackEndReached: break;
            case MediaEvent.Type.PlaybackSuspended: break;
            case MediaEvent.Type.PlaybackResumed: break;
            case MediaEvent.Type.PlaybackNewLoop:
                if (CConfigMng.Instance._bIsMediaServer){
                    _NetWorkUDP.Send(PROTOCOL.RCV_START_MOVIE);
                }
                break;
            case MediaEvent.Type.PlaybackErrorOccured: break;
        }
    }


    public void PlayerPlay()
    {
        m_MediaPlayer.Play();
    }

    public void PlayerPause() 
    {
        m_MediaPlayer.Pause();
    }
    public void PlayerTogglePause(){
        m_MediaPlayer.TogglePause();
    }

    public void PlayerForwardOneStep()
    {
        m_MediaPlayer.StepForward();

    }
    public void PlayerBackWardOneStep()
    {
        m_MediaPlayer.StepForward();
    }

    public void PlayerSeekToTime(float fSeconds)
    {
        m_MediaPlayer.SeekToTime(fSeconds);
    }

    public void PlayerSeekToFrame(int nFrame)
    {
        m_MediaPlayer.SeekToFrame(nFrame);
    }


    public void EnableIDLEWindow()
    {

        transform.localPosition = new Vector3(0.0f, 0.0f, 1.0f);

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