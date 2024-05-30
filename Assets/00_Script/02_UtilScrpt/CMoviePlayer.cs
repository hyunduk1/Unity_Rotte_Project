using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DemolitionStudios.DemolitionMedia;

public class CMoviePlayer : MonoBehaviour
{
  
    public CMoviePlayer _SubVideoPlayer;

    public Media   m_MediaPlayer;
    

    public string _FolderName;
    public string _FileName;

    public bool _isLoop = false;
    public bool _isAutoStart = true;
    public bool _IsSubVideo=false;
    public bool _SeltDestroy = false;
    private CanvasGroup m_CanvasGroup;


    private void Awake()
    {    
        if(_isLoop)
            m_MediaPlayer.Loops =-1;
        else
            m_MediaPlayer.Loops = 1;


        m_MediaPlayer.openOnStart = true;   

        m_MediaPlayer.mediaUrl = _FolderName + "/" + _FileName + ".mov";
        if (m_MediaPlayer != null){
            m_MediaPlayer.Events.AddListener(OnMediaPlayerEvent);
        }

        m_CanvasGroup = transform.GetComponent<CanvasGroup>();
    }
    void Start()
    {
      
    }
    public void OnMediaPlayerEvent(Media source, MediaEvent.Type type, MediaError error)
    {
        Debug.Log("[들어온다이벤트]   :   " + type.ToString());
        switch (type)
        {
            case MediaEvent.Type.Closed: break;
            case MediaEvent.Type.OpeningStarted: break;
            case MediaEvent.Type.PreloadingToMemoryStarted: break;
            case MediaEvent.Type.PreloadingToMemoryFinished: break;
            case MediaEvent.Type.Opened:
                if(_isAutoStart)
                    PlayStart();
                break;
            case MediaEvent.Type.OpenFailed: break;
            case MediaEvent.Type.VideoRenderTextureCreated: break;
            case MediaEvent.Type.PlaybackStarted: break;
            case MediaEvent.Type.PlaybackStopped:
                if (_SubVideoPlayer != null){
                    _SubVideoPlayer.PlayStart();
                    Destroy(gameObject);
                }
                else
                {
                    if (_SeltDestroy)
                        Destroy(gameObject);                    
                }
                
                break;
            case MediaEvent.Type.PlaybackEndReached: break;
            case MediaEvent.Type.PlaybackSuspended: break;
            case MediaEvent.Type.PlaybackResumed: break;
            case MediaEvent.Type.PlaybackNewLoop: break;
            case MediaEvent.Type.PlaybackErrorOccured: break;
        }
    }
    void Update()
    {
  
    }

    public void PlayStart()
    {
        m_CanvasGroup.alpha = 1.0f;
        m_MediaPlayer.Play();
        if (_isLoop)
            m_MediaPlayer.Loops = -1;
        else
            m_MediaPlayer.Loops = 1;
       
    }
    public void PlayStop()
    {

    }
    public void PlayColose()
    {

    }
}
