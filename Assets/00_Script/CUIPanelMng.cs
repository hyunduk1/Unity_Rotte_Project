 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WINOOW 
{
    PANEL_TOP=0,
    PANEL_FIRST,
    PANEL_SECOND,
    PANEL_THIRD,
    PANEL_ANALYZING,
    PANEL_RESOULT 
}

public class CUIPanelMng : MonoBehaviour
{
    private static CUIPanelMng _instance;
    public static CUIPanelMng Instance { get { return _instance; } }



    public GameObject _InsertNode;
    public GameObject _InsertNaviNode;
    public GameObject _InsertIDLEMovieNode;
    public GameObject _IndsertEffectNode;
    private List<string> m_strPanelName = null;
    private Dictionary<string, GameObject> m_ListPrefabs;

    private GameObject m_objCurrentObject;
    private GameObject m_objNaviGation;
    private CUIPanelNavigation m_NavigationPoint;


    public bool m_bIsDebug = false;
    private float m_fResetCurrentTime = 0.0f;
    private bool m_bCheckIdleTime = false;
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
        
        m_ListPrefabs = new Dictionary<string, GameObject>();
        m_strPanelName = new List<string>();
        m_objCurrentObject = null;

        LoadPrefabs("", "00_PanelTop");
        LoadPrefabs("", "01_PanelFirstQuesition");
        LoadPrefabs("", "02_PanelSecondQuesition");
        LoadPrefabs("", "03_PanelThirdQuesition");
        LoadPrefabs("", "04_PanelAnalyzing");
        LoadPrefabs("", "05_PanelResoult");
        LoadPrefabs("", "06_PanelNaviGation");
        LoadPrefabs("", "07_PaneIdleMovielServer");
        LoadPrefabs("", "08_PaneIdleMovielClient");
        LoadPrefabs("", "09_ResoultEffectBoom");
        LoadPrefabs("", "12_LeftFirstAndRight");
        LoadPrefabs("", "13_RightFirstAndLeft");
        LoadPrefabs("", "15_VideoSyncPlayerServer");
       
    }
    
    public void Start()
    {
      
    }

    public void StartContents()
    {
        if (m_bIsDebug == false)
        {
           InsertIdleMoviePanel();
        }
    }
    public void NextModeIdle()
    {
        if (m_objCurrentObject != null)
        {
            m_objCurrentObject.GetComponent<CUIPanel>().FadeOutWindow();
        }
        //--------------네비게이션--------------2022.12.14현덕--
        FadeOutNaviGation();
        //------------------------------------------------------
        CMovieSyncPlayer.Instance.EnableIDLEWindow();
        m_fResetCurrentTime = 0.0f;
        m_bCheckIdleTime = false;
    }
    public void Update()
    {
        if (m_bCheckIdleTime == false)
            return;
        //------------------------------------------------------------------------------------------
        //idle 모드 전환시간이 되면 m_fResetCurrentTime 시간이 0.0초가
        //되고 m_bCheckIdleTime == false 되면서 리턴
        //------------------------------------------------------------------------------------------
        m_fResetCurrentTime += Time.deltaTime;
        if(m_fResetCurrentTime > CConfigMng.Instance._fIdleModeChangeTime)
        {
            Debug.Log("IDLE MODE 진입하라");
            if (m_objCurrentObject != null)
            {
                m_objCurrentObject.GetComponent<CUIPanel>().FadeOutWindow();
            }
            //--------------네비게이션--------------2022.12.14현덕--
            FadeOutNaviGation();
            //------------------------------------------------------

            CMovieSyncPlayer.Instance.EnableIDLEWindow();
            m_fResetCurrentTime = 0.0f;
            m_bCheckIdleTime = false;
        }
    }

    public void ResetIdleCheckTime()
    {
        m_fResetCurrentTime = 0.0f;
    }
    public  void SetIdleCheckTimeEnable(bool bEnable)
    {
        ResetIdleCheckTime();
        m_bCheckIdleTime = bEnable;
    }
    public void SetNaviGationPoint(CUIPanelNavigation pNav)
    {
        m_NavigationPoint = pNav;
    }

    public CUIPanelNavigation GetNaviGationPoint()
    {
        return m_NavigationPoint;
    }
    public void LoadPrefabs(string strFolderName, string strFileName)
    {
        GameObject tempObject = Resources.Load("01_Panel/" + strFolderName + strFileName) as GameObject;

        if (tempObject != null)
        {
            m_ListPrefabs.Add(strFileName, tempObject);
            m_strPanelName.Add(strFileName);
        }
        else
        {
        }
    }

    //----------------------------------------------------------------
    //아이들 노드
    //----------------------------------------------------------------
    public void InsertIdleMoviePanel()
    {
        GameObject tempWindow;
        /*
        if (CConfigMng.Instance._bIsMediaServer)
            tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs["07_PaneIdleMovielServer"]) as GameObject;
        else
            tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs["08_PaneIdleMovielClient"]) as GameObject;
        */

        tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs["15_VideoSyncPlayerServer"]) as GameObject;

        
        tempWindow.transform.SetParent(_InsertIDLEMovieNode.transform);
        RectTransform rectTransform = tempWindow.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0.0f);
        rectTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        tempWindow.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        tempWindow.GetComponent<CMovieSyncPlayer>().EnableIDLEWindow();
    }
    //----------------------------------------------------------------
    //네비게이션 노드
    //----------------------------------------------------------------

    public void InsertEffectPanel()
    {
        GameObject tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs["09_ResoultEffectBoom"]) as GameObject;
        tempWindow.transform.SetParent(_IndsertEffectNode.transform);
        RectTransform rectTransform = tempWindow.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0.0f);
        rectTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        tempWindow.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void InsertNaviGation()
    {
        if (m_objNaviGation != null)
        {
            m_objNaviGation.GetComponent<CUIPanel>().FadeOutWindow();
        }
        GameObject tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs["06_PanelNaviGation"]) as GameObject;
        tempWindow.transform.SetParent(_InsertNaviNode.transform);

        RectTransform rectTransform = tempWindow.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0.0f);
        rectTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        tempWindow.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        tempWindow.GetComponent<CUIPanel>().FadeInWindow();
        m_NavigationPoint = tempWindow.GetComponent<CUIPanelNavigation>();
        m_objNaviGation = tempWindow;
    }
    public void  FadeOutNaviGation()
    {
        if (m_objNaviGation != null){
            m_objNaviGation.GetComponent<CUIPanel>().FadeOutWindow();
            m_objNaviGation = null;
            m_NavigationPoint = null;
        }
    }

    public void DrawNaviInfoCategory(NAVI_STATE state)
    {
        if (m_objNaviGation != null){
            m_NavigationPoint.DrawNaviInfoCategory(state);
        }
    }  
    //----------------------------------------------------------------
    //MainPanel 노드
    //----------------------------------------------------------------
    public void InsertNextEventPanel(string strTypeName)
    {
        if (m_objCurrentObject != null)
        {
            m_objCurrentObject.GetComponent<CUIPanel>().FadeOutWindow();
        }
        ResetIdleCheckTime();
        GameObject tempWindow = MonoBehaviour.Instantiate(m_ListPrefabs[strTypeName]) as GameObject;
        tempWindow.transform.SetParent(_InsertNode.transform);
        RectTransform rectTransform = tempWindow.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0.0f);
        rectTransform.anchoredPosition3D = new Vector3(0.0f, 0.0f, 0.0f);
        tempWindow.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        tempWindow.GetComponent<CUIPanel>().FadeInWindow();
        m_objCurrentObject = tempWindow;
    }

    public GameObject GetPrefabsRef(string strPrefName)
    {
        return m_ListPrefabs[strPrefName];
    }
    public void InsertNextEventPanel(int nNumber)
    {
        InsertNextEventPanel(m_strPanelName[nNumber]);
    }
    public void InsertNextHomeButton()
    {
        InsertNextEventPanel("00_PanelTop");
    }

    public void EnableIDLEWindow()
    {

    }
    public void DisableIDLEWindow()
    {

    }




    //Q1,Q2,Q3,RESULT
}
