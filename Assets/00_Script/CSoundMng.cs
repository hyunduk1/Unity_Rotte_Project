using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SOUND_NAME
{
    TALK_BOX_01,
    TALK_BOX_02,
    TALK_BOX_SELECT,
    BUTTON_DEFAULT,
    BUTTON_ANSWER,
    TIME_COUNT,

    START_MOTION_01,
    START_MOTION_02,
    START_MOTION_03,
    START_MOTION_04,
    START_MOTION_LOOP,

    ANALYZING_LOOP,
    ANALYZING_LOOP_SUB,

    ANALYZING_EFFECT,

    RESULT_BEAR_LOOP,
    BOTTOM_MOTION_IN,
    BOTTOM_MOTION_LOOP,
    RESULT_EAGLE_LOOP,
    RESULT_ELEPHANT_LOOP,
    RESULT_GAZZLE_LOOP,
    RESULT_JAGUAR_LOOP,
    RESULT_KOA_LOOP,
    LEFT_MOTION_IN_LOOP,
    RESULT_RACOON_LOOP,
    RIGHT_MOTION_IN_LOOP,
    RESULT_SQUA_LOOP,
    NONE_SOUND
  
}


public enum SOUND_BGM
{
    BG_01
}

public class CSoundMng : MonoBehaviour
{
    private static CSoundMng _instance;
    public static CSoundMng Instance { get { return _instance; } }

    public int              _MaxAudioSourceCount = 3;
    public AudioClip[]      _AudioBgmClipArray;
    public AudioClip[]      _AudioSFXClipArray;
    public float            _fBGMVolume=1.0f;
    public float            _fSFXVolume = 1.0f;
    private AudioSource     m_AudioBGMsource;
    private AudioSource[]   m_AudioSFXsource;
    private AudioClip       m_AudioCrossClip;//바뀌는 클립
    private bool            M_bIsChanging = false;
    private float           m_fStartTime;
    public float            _fCrossVolumeSpeed;

    void Awake()
    {
        if (_instance == null){
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
 
    void Start()
    {
        m_AudioBGMsource = gameObject.AddComponent<AudioSource>();
        m_AudioBGMsource.volume = _fBGMVolume;
        m_AudioBGMsource.playOnAwake = false;
        m_AudioBGMsource.loop = true;
        m_AudioSFXsource = new AudioSource[_AudioSFXClipArray.Length];
              
        for (int i = 0; i < m_AudioSFXsource.Length; i++){
            m_AudioSFXsource[i] = gameObject.AddComponent<AudioSource>();
            m_AudioSFXsource[i].playOnAwake = false;
            m_AudioSFXsource[i].volume = _fSFXVolume;
        }

        if(CConfigMng.Instance._bBgSound)
            PlayBGMSound(SOUND_BGM.BG_01.ToString(),true);

        SetBGMVolume(CConfigMng.Instance._fBgVolume);
        SetSFXVolume(CConfigMng.Instance._fSFXVolume);
    }
   
    public void PlaySound(string name, bool loop = false, float pitch = 1)
    {
        for (int i = 0; i < _AudioSFXClipArray.Length; i++){
            if (_AudioSFXClipArray[i].name == name) {
                AudioSource tempSound = GetEmptyAudioSource();       
                tempSound.loop = loop;              
                tempSound.pitch = pitch;
                tempSound.clip = _AudioSFXClipArray[i];
                tempSound.Play();
                return;
            }
        }
    }

    public void StopSoundName(string name)
    {
        if (name == SOUND_NAME.NONE_SOUND.ToString())
            return;
     
        for (int i = 0; i < m_AudioSFXsource.Length; i++){
            if (m_AudioSFXsource[i].isPlaying == true){
                if (m_AudioSFXsource[i].clip.name == name){
                    m_AudioSFXsource[i].Stop();
                }
            }
        }
    }
    private AudioSource GetEmptyAudioSource()
    {
        int nSelectIndex = 0;
        float lageProgress = 0;
        for (int i = 0; i < m_AudioSFXsource.Length; i++){
            if (!m_AudioSFXsource[i].isPlaying){
                return m_AudioSFXsource[i];
            }
            float progress = m_AudioSFXsource[i].time / m_AudioSFXsource[i].clip.length;
            if (progress > lageProgress && !m_AudioSFXsource[i].loop){
                nSelectIndex = i;
                lageProgress = progress;
            }
        }
        return m_AudioSFXsource[nSelectIndex];
    }

  
    public void PlayBGMSound(string name, bool isSmooth = false)
    {
        m_AudioCrossClip = null;
        for (int i = 0; i < _AudioBgmClipArray.Length; i++){
            if (_AudioBgmClipArray[i].name == name){
                m_AudioCrossClip = _AudioBgmClipArray[i];
            }
        }

        if (m_AudioCrossClip == null)
            return;

        if (!isSmooth){
            m_AudioBGMsource.clip = m_AudioCrossClip;
            m_AudioBGMsource.Play();
        }
        else{
            m_fStartTime = Time.time;
            M_bIsChanging = true;
        }
    }

    public string GetRandomBGMName()
    {
        return _AudioBgmClipArray[Random.Range(0, _AudioBgmClipArray.Length)].name;
    }

    private void Update()
    {
        if (!M_bIsChanging) return;

        float progress = (Time.time - m_fStartTime) * _fCrossVolumeSpeed;
        m_AudioBGMsource.volume = Mathf.Lerp(_fBGMVolume, 0, progress);

        if (progress > 1){
            M_bIsChanging = false;
            m_AudioBGMsource.volume = _fBGMVolume;
            m_AudioBGMsource.clip = m_AudioCrossClip;
            m_AudioBGMsource.Play();
        }
    }

    public void StopBGMSound()
    {
        m_AudioBGMsource.Stop();
    }

    public void SetPitch(float pitch)
    {
        m_AudioBGMsource.pitch = pitch;
    }

    public float[] GetAudioSample(int sampleCount, FFTWindow fft)
    {
        float[] samples = new float[sampleCount];

        m_AudioBGMsource.GetSpectrumData(samples, 0, fft);

        if (samples != null)
            return samples;
        else
            return null;
    }

    //볼륨

    public void SetBGMVolume(float fVolume)
    {
        _fBGMVolume = fVolume;
        m_AudioBGMsource.volume = fVolume;
    }

    public void SetSFXVolume(float fVolume)
    {
        _fSFXVolume = fVolume;
        for (int i = 0; i < m_AudioSFXsource.Length; i++){
            m_AudioSFXsource[i].volume = fVolume;
        }
    }
}



/*
    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSoundMng : MonoBehaviour
{
    private static CSoundMng _instance;
    public static CSoundMng Instance { get { return _instance; } }


    public GameObject _SoundNode;

    private GameObject m_ObjSound;

    private List<string> m_strSoundlName = null;
    private Dictionary<string, AudioClip> m_ListSounds;


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
        m_ListSounds = new Dictionary<string, AudioClip>();
        m_strSoundlName = new List<string>();
        LoadSound("", "00_Sound");
        LoadSound("", "01_Sound");
        LoadSound("", "02_Sound");
        LoadSound("", "_00_Button");
        LoadSound("", "_00_Crash");
        LoadSound("", "videoplayback");
        LoadSound("", "ALLButton");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            InsertBgmSoundPanel("_00_Button");
        if (Input.GetKeyDown(KeyCode.N))
            InsertBgmSoundPanel("02_Sound");
    }


    //----------------------------------------------------------
    //객체화
    //----------------------------------------------------------
    public void InsertBgmSoundPanel(string FileName, bool Looping = false)
    {
        if(m_ObjSound != null)
        {
            BGM_VolumeFade(m_ObjSound.GetComponent<AudioSource>(), 1.0f);
            Destroy(m_ObjSound);
        }
        GameObject tempObj = new GameObject();
        tempObj.name = "Sound(clone)";
        tempObj.transform.SetParent(_SoundNode.transform);
        AudioSource tempAudio = tempObj.AddComponent<AudioSource>();
        if (tempAudio != null)
        {
            tempAudio.clip = Resources.Load("02_Sound/" + FileName) as AudioClip;
            tempAudio.Play();
            tempAudio.loop = Looping;
            //tempAudio.PlayOneShot(Resources.Load("Sound/" + FileName) as AudioClip);
        }
        else
        {
            Debug.LogError("-----" + FileName + "-----존재하지 않습니다!");
        }
         m_ObjSound = tempObj ;
    }
    public void LoadSound(string strFolderName, string strFileName)
    {
        AudioClip tempObject = Resources.Load("Sound/" + strFolderName + strFileName) as AudioClip;

        if (tempObject != null)
        {
            m_ListSounds.Add(strFileName, tempObject);
            m_strSoundlName.Add(strFileName);
        }
        else
        {
        }
    }


    //----------------------------------------------------------
    //오디오 볼륨 페이드 아웃
    //----------------------------------------------------------
    public static IEnumerator BGM_VolumeFade(AudioSource TypeAudio, float FadeTime)
    {
        float fStartVolume = TypeAudio.volume;
        while(TypeAudio.volume > 0)
        {
            
            TypeAudio.volume -= fStartVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        TypeAudio.clip = null;
        
    }/*
}*/