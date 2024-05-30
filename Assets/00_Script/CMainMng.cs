using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum QUESTION_CATEGORY
{
    FIRST_ANSWER=0,
    SECOND_ANSER = 1,
    THIRD_ANSER = 2
}

[System.Serializable]
public struct CATEGORY_TYPE
{
    public QUESTION_CATEGORY Category;
};

public class CMainMng : MonoBehaviour
{
    private static CMainMng _instance;
    public static CMainMng Instance { get { return _instance; } }

    public Sprite[] _BgSprite;
    public Image _BgImage;

    public GameObject _DonotTouchWindow;
    private bool[] m_byAnswerDataType;

    private List<Dictionary<string, object>> m_listDataFile;
    private List<Dictionary<string, object>> m_listInfoDataFile;

    private List<Dictionary<string, object>> m_listInfoQuestionText;
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
        m_byAnswerDataType = new bool[3];
        ReadDataFile();
    }

    void Start()
    {
        ContentsTypeChange();
    }

    int _ScreenCount = 0;
    void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.P))
        { string strFileName= _ScreenCount.ToString()+"_ScreenCapture.png";
            ScreenCapture.CaptureScreenshot(strFileName);
            _ScreenCount++;
        }
    }

    //--------------------------------------------------------------------------
    ///donot Touch 딜레이!  _fTrasionsSpeed
    //--------------------------------------------------------------------------
    public void EnalbeDonotTouchWindow(bool bEnable)
    {
        _DonotTouchWindow.SetActive(bEnable);
        Invoke("DisableDonotTouchWindow", CConfigMng.Instance._fTrasionsSpeed);
    }

    public void DonotTouchWindow(bool bEnable)
    {
        _DonotTouchWindow.SetActive(bEnable);
        
        Invoke("DisableDonotTouchWindow", CConfigMng.Instance._fHyundukuTouch);
    }
    public bool GetDoNotTouchWindowState()
    {
        return _DonotTouchWindow.activeSelf;
    }
    public void DisableDonotTouchWindow()
    {
        _DonotTouchWindow.SetActive(false);

    }
    
    //--------------------------------------------------------------------------
    //백그라운드 선택 메모장 _nContentsType
    //--------------------------------------------------------------------------
    public void ContentsTypeChange()
    {
        //_ArrayBgImage[CConfigMng.Instance._nContentsType].SetActive(true);
        _BgImage.sprite = _BgSprite[CConfigMng.Instance._nContentsType];
    }

    private void CheckHealthType()
    {
        List<Dictionary<string, object>> data_Dialog = CCSVReader.Read("QuestionEnd");

        for (int i = 0; i < data_Dialog.Count; i++)
        {
            print(data_Dialog[i]["Content"].ToString());
        }
    }

    public void SetQuestionType(byte byCategory, bool bEnable)
    {
        m_byAnswerDataType[byCategory] = bEnable;
    }
    private void ReadDataFile()
    {

        m_listDataFile         = CCSVReader.ReadStreamAssetFolder(Application.streamingAssetsPath + "/QuestionEnd.CSV");
        m_listInfoDataFile     = CCSVReader.ReadStreamAssetFolder(Application.streamingAssetsPath + "/ResultDataInfoText.CSV");
        m_listInfoQuestionText = CCSVReader.ReadStreamAssetFolder(Application.streamingAssetsPath + "/QuestionText.CSV");

    }

    public string GetCharacterTypeMainText(byte byType)
    {
        return m_listInfoDataFile[byType]["TITLE_TEXT"].ToString();
    }
    public string GetCharacterTypeSubText(byte byType)
    {
        return m_listInfoDataFile[byType]["SUB_TEXT"].ToString();
    }

    private byte GetBooleanToByte(bool byValue)
    {
        if (byValue == true)
            return 1;

        return 0;
    }

    public string GetQuestionText(byte byIndex, bool bType)
    {
        string tempString;
        if(bType == false)
            tempString = m_listInfoQuestionText[byIndex]["Left"].ToString();
        else
            tempString = m_listInfoQuestionText[byIndex]["Right"].ToString();

        return tempString;
    }



    public byte GetResultType()
    {
        byte byType = 0;

        string strResultType = GetBooleanToByte(m_byAnswerDataType[0]).ToString() +
                               GetBooleanToByte(m_byAnswerDataType[1]).ToString() +
                               GetBooleanToByte(m_byAnswerDataType[2]).ToString();
        for (int i = 0; i < m_listDataFile.Count; i++)
        {
            string strValue =
                      m_listDataFile[i]["Q1"].ToString() +
                      m_listDataFile[i]["Q2"].ToString() +
                      m_listDataFile[i]["Q3"].ToString();
            if (strValue == strResultType)
            {
                return byte.Parse(m_listDataFile[i]["RESULT"].ToString());
            }
        }
        return byType;
    }
}

