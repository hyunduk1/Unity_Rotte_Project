using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;


using System.Net.Sockets;
using System.Net;
public struct IP_INFO
{

    public string strIp;
    public int nPort;

    public IP_INFO(string ip, int port)
    {
        this.strIp = ip;
        this.nPort = port;
    }

    public void DebugLog()
    {
        Debug.LogFormat("IP = {0}  PORT = {1}", this.strIp, this.nPort);
    }
}

public class CConfigMng : MonoBehaviour
{
    private static CConfigMng _instance;
    public static CConfigMng Instance { get { return _instance; } }

    const int HWND_TOPMOST = -2;
    const uint SWP_HIDEWINDOW = 0x0080;
    const uint SWP_SHOWWINDOW = 0x0040;

    private static string strPath;
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);



    private float m_fContentsUpdate; public float _fContentsUpdate { get { return m_fContentsUpdate; } set { m_fContentsUpdate = value; } }
    private float m_fTrasionsSpeed; public float _fTrasionsSpeed { get { return m_fTrasionsSpeed; } set { m_fTrasionsSpeed = value; } }
    private float m_fHyundukTouch; public float _fHyundukuTouch { get { return m_fHyundukTouch; } set { m_fHyundukTouch = value; } }
    private float m_fDelayTime;     public float _fDelayTime { get { return m_fDelayTime; } set { m_fDelayTime = value; } }

    private bool m_isFullScreen;    public bool _isFullScreen { get { return m_isFullScreen; } set { m_isFullScreen = value; } }
    private int m_ScreenSizeX;      public int _ScreenSizeX { get { return m_ScreenSizeX; } set { m_ScreenSizeX = value; } }
    private int m_ScreenSizeY;      public int _ScreenSizeY { get { return m_ScreenSizeY; } set { m_ScreenSizeY = value; } }
    private int m_ScreenPosX;       public int _ScreenPosX { get { return m_ScreenPosX; } set { m_ScreenPosX = value; } }
    private int m_ScreenPosY;       public int _ScreenPosY { get { return m_ScreenPosY; } set { m_ScreenPosY = value; } }

   //  private string m_strMediaServerIP; public string _strMediaServerIP { get { return m_strMediaServerIP; } set { m_strMediaServerIP = value; } }
   // private int m_nMediaServerPort;     public int _nMediaServerPort { get { return m_nMediaServerPort; } set { m_nMediaServerPort = value; } }

    private bool m_bIsMediaServer;      public bool _bIsMediaServer { get { return m_bIsMediaServer; } set { m_bIsMediaServer = value; } }
    private int m_nContentsType;        public int _nContentsType { get { return m_nContentsType; } set { m_nContentsType = value; } }

    
     private float m_fAlnaylizingTime; public float _fAlnaylizingTime { get { return m_fAlnaylizingTime; } set { m_fAlnaylizingTime = value; } }
    private float m_fIdleModeChangeTime; public float _fIdleModeChangeTime { get { return m_fIdleModeChangeTime; } set { m_fIdleModeChangeTime = value; } }

    private bool m_bFpsToString; public bool _bFpsToString { get { return m_bFpsToString; } set { m_bFpsToString = value; } }

    private bool m_bCursorEnable; public bool _bCursorEnable { get { return m_bCursorEnable; } set { m_bCursorEnable = value; } }

    private int m_nSpriteAniTime; public int _nSpriteAniTime { get { return m_nSpriteAniTime; } set { m_nSpriteAniTime = value; } }

    private string[] m_strResourceName; public string[] _strResourceName { get { return m_strResourceName; } set { m_strResourceName = value; } }

    private bool m_bBgSound; public bool _bBgSound { get { return m_bBgSound; } set { m_bBgSound = value; } }


    private float m_fBgVolume; public float _fBgVolume { get { return m_fBgVolume; } set { m_fBgVolume = value; } }

    private float m_fSFXVolume; public float _fSFXVolume { get { return m_fSFXVolume; } set { m_fSFXVolume = value; } }

  //  private string m_strCurrentIP; public string _strCurrentIP { get { return m_strCurrentIP; } set { m_strCurrentIP = value; } }
  //  private int m_nMediaCurrentPort; public int _nMediaCurrentPort { get { return m_nMediaCurrentPort; } set { m_nMediaCurrentPort = value; } }

    private List<IP_INFO> m_listIP; public List<IP_INFO> _listIP { get { return m_listIP; } set { m_listIP = value; } }

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


        m_listIP = new List<IP_INFO>();
        Application.targetFrameRate = 60;
        strPath = Application.dataPath + "/StreamingAssets/Config.ini";

        m_bCursorEnable         = IniReadValuebool("SET_VALUE", "IS_CURSOR_ENABLE");
        m_bFpsToString          = IniReadValuebool("SET_VALUE", "FPS_TOSTRING");
    
        m_isFullScreen          = IniReadValuebool("RESOLUTION_SET", "IS_WINDOW_MODE");
        m_ScreenPosX            = IniReadValueInt("RESOLUTION_SET", "SCREEN_POS_X");
        m_ScreenPosY            = IniReadValueInt("RESOLUTION_SET", "SCREEN_POS_Y");
        m_ScreenSizeX           = IniReadValueInt("RESOLUTION_SET", "SCREEN_SIZE_X");
        m_ScreenSizeY           = IniReadValueInt("RESOLUTION_SET", "SCREEN_SIZE_Y");
         
   
        m_fIdleModeChangeTime   = IniReadValueFloat("CONTENTS_SET", "IDLE_MODE_CHANGE_TIME");
        m_fAlnaylizingTime      = IniReadValueFloat("CONTENTS_SET", "ANALIZYING_END_TIME");
        m_fTrasionsSpeed        = IniReadValueFloat("CONTENTS_SET", "TRANSITION_SPEED");
        m_fHyundukTouch         = IniReadValueFloat("CONTENTS_SET", "HYUNDUKTOUCH");
        m_nContentsType         = IniReadValueInt("CONTENTS_SET", "CONTENTS_TYPE");
        m_nSpriteAniTime        = IniReadValueInt("CONTENTS_SET", "SPRITE_ANI_TIME");
        m_fDelayTime            = IniReadValueFloat("CONTENTS_SET", "DELAY_TIME");

        m_bBgSound      = IniReadValuebool("SOUND_SET", "ENABLE_BG_SOUND");
        m_fBgVolume     = IniReadValueFloat("SOUND_SET", "BG_VOLUME");
        m_fSFXVolume    = IniReadValueFloat("SOUND_SET", "SFX_VOLUME");


        string temServer = IniReadValue("NETWORK_SET", "MEDIA_SERVER_IP");
        string[] strArrayIP = temServer.Split(':');

     //   m_strMediaServerIP = strArrayIP[0];
     //   m_nMediaServerPort = int.Parse(strArrayIP[1]);
     //   string temServer2 = IniReadValue("NETWORK_SET", "CURRENT_PC_IP");
     //   string[] strArrayIP2 = temServer.Split(':');
       // m_strCurrentIP = strArrayIP2[0];
       // m_nMediaCurrentPort = int.Parse(strArrayIP2[1]);

        m_bIsMediaServer = IniReadValuebool("NETWORK_SET", "MEDIA_SERVER_TYPE "); ;

        for (int i = 0; i < 4; i++)
        {
            string tempString = IniReadValue("NETWORK_SET", "PC_NUMBER_IP_0" + i.ToString());
            string[] strArray = tempString.Split(':');
            IP_INFO tempInfo = new IP_INFO(strArray[0], int.Parse(strArray[1]));
            m_listIP.Add(tempInfo);
        }


        m_strResourceName = new string[(int)BUNDLE_RESOURCE.TOTAL_BUNDLE];
        m_strResourceName[0] = IniReadValue("ASSET_BUNDLE", "START_SEQ_01");
        m_strResourceName[1] = IniReadValue("ASSET_BUNDLE", "START_SEQ_02");
        m_strResourceName[2] = IniReadValue("ASSET_BUNDLE", "START_SEQ_03");
        m_strResourceName[3] = IniReadValue("ASSET_BUNDLE", "START_SEQ_04");
        m_strResourceName[4] = IniReadValue("ASSET_BUNDLE", "START_SEQ_05");
        m_strResourceName[5] = IniReadValue("ASSET_BUNDLE", "START_SEQ_06");
        m_strResourceName[6] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_1_LOOP");
        m_strResourceName[7] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_2_LOOP");
        m_strResourceName[8] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_3_LOOP");
        m_strResourceName[9] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_1");
        m_strResourceName[10] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_2");
        m_strResourceName[11] = IniReadValue("ASSET_BUNDLE", "QNA_BOOTTOM_CHAR_3");
        m_strResourceName[12] = IniReadValue("ASSET_BUNDLE", "QNA_LEFT_CHAR");
        m_strResourceName[13] = IniReadValue("ASSET_BUNDLE", "QNA_LEFT_CHAR_LOOP");
        m_strResourceName[14] = IniReadValue("ASSET_BUNDLE", "QNA_RIGHT_CHAR");
        m_strResourceName[15] = IniReadValue("ASSET_BUNDLE", "QNA_RIGHT_CHAR_LOOP");
        m_strResourceName[16] = IniReadValue("ASSET_BUNDLE", "ANALYZING_SEQ");
        m_strResourceName[17] = IniReadValue("ASSET_BUNDLE", "ANALYZING_EFFECT");
        m_strResourceName[18] = IniReadValue("ASSET_BUNDLE", "ENDING_BEAR");
        m_strResourceName[19] = IniReadValue("ASSET_BUNDLE", "ENDING_EAGLE");
        m_strResourceName[20] = IniReadValue("ASSET_BUNDLE", "ENDING_ELEPHANT");
        m_strResourceName[21] = IniReadValue("ASSET_BUNDLE", "ENDING_GAZZLE");
        m_strResourceName[22] = IniReadValue("ASSET_BUNDLE", "ENDING_JAGUAR");
        m_strResourceName[23] = IniReadValue("ASSET_BUNDLE", "ENDING_KOA");
        m_strResourceName[24] = IniReadValue("ASSET_BUNDLE", "ENDING_RACOON");
        m_strResourceName[25] = IniReadValue("ASSET_BUNDLE", "ENDING_SQUA");
        m_strResourceName[26] = IniReadValue("ASSET_BUNDLE", "IDLE_TOUCH_1");
        m_strResourceName[27] = IniReadValue("ASSET_BUNDLE", "IDLE_TOUCH_2");
        m_strResourceName[28] = IniReadValue("ASSET_BUNDLE", "IDLE_TOUCH_3");
        m_strResourceName[29] = IniReadValue("ASSET_BUNDLE", "IDLE_TOUCH_4");


#if UNITY_EDITOR
#else
        SetScreenResolution();
#endif

    }
    private void Start()
    {
      
    }
    public void SetScreenResolution()
    {
     //   Application.targetFrameRate = 60;
        Cursor.visible = m_bCursorEnable;
        Application.runInBackground = true;
        Screen.SetResolution((int)m_ScreenSizeX, (int)m_ScreenSizeY, false);

        SetWindowPos(GetForegroundWindow(), (IntPtr)HWND_TOPMOST, m_ScreenPosX, m_ScreenPosY, m_ScreenSizeX, m_ScreenSizeY, SWP_SHOWWINDOW);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    public static string IniReadValue(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        return temp.ToString();
    }


    public static float IniReadValueFloat(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        float result = 0.0f;
        float.TryParse(temp.ToString(), out result);
        return result;
    }

    public static bool IniReadValuebool(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        if (result == 1)
        {
            return true;
        }
        return false;
    }

    public static int IniReadValueInt(string Section, string Key)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        return result;
    }


    public static int IniReadValueIntTimeData(string Section, string Key, string strDataPath)
    {
        StringBuilder temp = new StringBuilder(255);
        GetPrivateProfileString(Section, Key, "", temp, 255, strDataPath);
        int result = 0;
        int.TryParse(temp.ToString(), out result);
        return result;
    }

    public int GetLocallPort()
    {
        for (int i = 0; i < CConfigMng.Instance._listIP.Count; i++){
            if (GetLocalIP(0) != m_listIP[i].strIp){
                return m_listIP[i].nPort;
            }
        }
        return 8686;
    }
    public string GetLocalIP(int nIndex)
    {
        IPAddress[] addr_arr = Dns.GetHostAddresses(Dns.GetHostName());
        List<string> list = new List<string>();

        foreach (IPAddress address in addr_arr)
        {
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                list.Add(address.ToString());
            }
        }

        if (list.Count == 0)
            return null;

        return list[nIndex];
    }
}



/*
AAAA	0
AAAB	0
AABA    1
AABB	1
ABAA	2
ABAB	2
ABBA	3
ABBB	3
BAAA	4
BAAB	4
BABA	5
BABB	5
BBAA	6
BBAB	6
BBBA	7
BBBB	7
*/