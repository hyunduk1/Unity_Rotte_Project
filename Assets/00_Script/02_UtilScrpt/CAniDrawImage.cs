using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAniDrawImage : MonoBehaviour 
{


    public bool _Looping = false;
    public bool _AutoPlaying = false;
   
    public RawImage _DrawImage;

    private Sprite[] _FirstImageArray;
    private Sprite[] _LoopImageArray;

    public BUNDLE_RESOURCE _FirstImageName;
    public BUNDLE_RESOURCE _LoopImageName;


    private int   m_nCurrentCount = 0;
    private int   m_nTotalAniCount = 0;
    private float m_fCurrentElapsTime = 0.0f;
    private float m_fFrameTime = 0.0f;
    private bool m_bSubLoopMotion = false;

    public SOUND_NAME _StartSound = SOUND_NAME.NONE_SOUND;
    public SOUND_NAME _LoopSound = SOUND_NAME.NONE_SOUND;

    public bool _FirstSoundLoop = false;
    public bool _SecondSoundLoop = false;
   
    void Start()
    {
        _FirstImageArray = CResourceLodingMng.Instance.GetResource((int)_FirstImageName);

        if(_LoopImageName!= BUNDLE_RESOURCE.NONE_BUNDLE)
            _LoopImageArray = CResourceLodingMng.Instance.GetResource((int)_LoopImageName);
        
        m_nTotalAniCount = _FirstImageArray.Length;
        m_fFrameTime =(float)(1.0f / (float)CConfigMng.Instance._nSpriteAniTime);
        if(_StartSound != SOUND_NAME.NONE_SOUND){
            if(_FirstSoundLoop==true)
                CSoundMng.Instance.PlaySound(_StartSound.ToString(),true);
            else 
                CSoundMng.Instance.PlaySound(_StartSound.ToString());
        }
    }
    public void PlayAnimation()
    {
        _AutoPlaying = true;
    }
    private void OnDestroy()
    {
        if (CSoundMng.Instance == null)
            return;

        CSoundMng.Instance.StopSoundName(_StartSound.ToString());
        CSoundMng.Instance.StopSoundName(_LoopSound.ToString());
    }
    private bool m_bOneShot = false;
    void Update()
    {
        if (_AutoPlaying){
            m_fCurrentElapsTime += Time.deltaTime;
            m_nCurrentCount = (int)(m_fCurrentElapsTime / m_fFrameTime);
            if (m_nCurrentCount >= m_nTotalAniCount){
                if (_Looping == true){
                    m_fCurrentElapsTime = 0.0f;
                    m_nCurrentCount = 0;
                    if (_LoopImageName != BUNDLE_RESOURCE.NONE_BUNDLE){
                        m_bSubLoopMotion = true;
                        m_nTotalAniCount = _LoopImageArray.Length;
                        if (_LoopSound != SOUND_NAME.NONE_SOUND){
                            if (m_bOneShot == false)
                            {
                                if(_SecondSoundLoop==true)
                                    CSoundMng.Instance.PlaySound(_LoopSound.ToString(), true);
                                else
                                    CSoundMng.Instance.PlaySound(_LoopSound.ToString());
                            }
                            m_bOneShot = true;
                        }
                    }
                }else{
                   Destroy(gameObject);
                    return;
                }
            }
            

            if (m_bSubLoopMotion)
                _DrawImage.texture = _LoopImageArray[m_nCurrentCount].texture;
            else
                _DrawImage.texture = _FirstImageArray[m_nCurrentCount].texture;
        }
    }
}
