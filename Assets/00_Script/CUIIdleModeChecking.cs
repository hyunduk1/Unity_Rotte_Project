using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CUIIdleModeChecking : MonoBehaviour
{
    public float _DrawRotationTime =0.3f ;
    public float _DrawTime = 10.0f;
    private int _CountReset = 5;
    public float _fRotationSpeed = 0.1f;
    public GameObject _RotationObj;
    public GameObject _TextObject;
    public TMP_Text _CountText;
    private bool _bRotation = false;
    private CShakeEffect m_ShakeEffect;
    private bool m_bShakingState = false;
    // Start is called before the first frame update
    void Start()
    {
        m_ShakeEffect = transform.GetComponent<CShakeEffect>();
        Invoke("DrawCount", _DrawTime);
    }

    public void DrawRotationCount()
    {

    }
    public void DrawCount()
    {
        m_ShakeEffect.ShakeStart();
        _bRotation = true;
        _RotationObj.transform.GetComponent<CanvasGroup>().alpha = 1.0f;
        _TextObject.transform.GetComponent<CanvasGroup>().alpha = 1.0f;
        m_bShakingState = true;
        InvokeRepeating("CountSet", 0.0f,1.0f);
    }


    public void ResetCount()
    {
        if (m_bShakingState == false)
            return;

        m_bShakingState = false;
        m_ShakeEffect.ResetShake();
        CancelInvoke("CountSet");

        _CountReset = 5;
        _bRotation = false;
        _RotationObj.transform.GetComponent<CanvasGroup>().alpha = 0.0f;
        _TextObject.transform.GetComponent<CanvasGroup>().alpha = 0.0f;
        m_bShakingState = false;
        CUIPanelMng.Instance.ResetIdleCheckTime();
        Invoke("DrawCount", _DrawTime);
    }

    public void CountSet()
    {

        if (CMainMng.Instance.GetDoNotTouchWindowState() == true)
            return;

        if (_CountReset == 0){
            CMainMng.Instance.DonotTouchWindow(true);
            CUIPanelMng.Instance.NextModeIdle();
            CSoundMng.Instance.PlaySound("TIME_COUNT_END");
            return;
        }
        CSoundMng.Instance.PlaySound(SOUND_NAME.TIME_COUNT.ToString());
        _CountText.text = _CountReset.ToString();
        _CountReset--;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bRotation == true)
        {
            RectTransform rectTransformText = _RotationObj.transform.GetComponent<RectTransform>();
            rectTransformText.Rotate(new Vector3(0.0f, 0.0f, _fRotationSpeed * Time.deltaTime));
        }
        
    }
}
