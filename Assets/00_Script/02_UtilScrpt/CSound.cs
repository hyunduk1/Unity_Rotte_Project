using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SOUND_PLAY
{
    public SOUND_NAME SoundType;
    public float DelayTime;
    public bool Loop;
 };

public class CSound : MonoBehaviour
{
    public SOUND_PLAY _StartSound;
    public SOUND_PLAY _EndSound;

    // Start is called before the first frame update
    void Start()
    {
        if (_StartSound.SoundType != SOUND_NAME.NONE_SOUND)
        {
            Invoke("StartSoundPlay", _StartSound.DelayTime);
        }

        if (_EndSound.SoundType != SOUND_NAME.NONE_SOUND){
            Invoke("EndSoundPlay", _EndSound.DelayTime);
        }
    }

    private void OnDestroy()
    {
        CSoundMng.Instance.StopSoundName(_StartSound.SoundType.ToString());
        CSoundMng.Instance.StopSoundName(_EndSound.SoundType.ToString());
    }
    public void StartSoundPlay()
    {
        CSoundMng.Instance.PlaySound(_StartSound.SoundType.ToString(), _StartSound.Loop);
    }
    public void EndSoundPlay()
    {
        CSoundMng.Instance.PlaySound(_EndSound.SoundType.ToString(), _EndSound.Loop);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
