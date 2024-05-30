using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
public enum BUNDLE_RESOURCE
{
    START_SEQ_01,
    START_SEQ_02,
    START_SEQ_03,
    START_SEQ_04,
    START_SEQ_05,
    START_SEQ_06,

    QNA_BOOTTOM_CHAR_1_LOOP,
    QNA_BOOTTOM_CHAR_2_LOOP,
    QNA_BOOTTOM_CHAR_3_LOOP,
    QNA_BOOTTOM_CHAR_1,
    QNA_BOOTTOM_CHAR_2,
    QNA_BOOTTOM_CHAR_3,
    QNA_LEFT_CHAR,
    QNA_LEFT_CHAR_LOOP,
    QNA_RIGHT_CHAR,
    QNA_RIGHT_CHAR_LOOP,

    ANALYZING_SEQ,
    ANALYZING_EFFECT,

    ENDING_BEAR,
    ENDING_EAGLE,
    ENDING_ELEPHANT,
    ENDING_GAZZLE,
    ENDING_JAGUAR,
    ENDING_KOA,
    ENDING_RACOON,
    ENDING_SQUA,

    IDLE_TOUCH_1,
    IDLE_TOUCH_2,
    IDLE_TOUCH_3,
    IDLE_TOUCH_4,
    TOTAL_BUNDLE,
    NONE_BUNDLE
}
public class CResourceLodingMng : MonoBehaviour
{
    private static CResourceLodingMng _instance;
    public static CResourceLodingMng Instance { get { return _instance; } }

    private Dictionary<int, Sprite[]> m_ListResource;
    private int m_nAssetBundleCount = 0;

    // Start is called before the first frame update
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

        m_ListResource = new Dictionary<int, Sprite[]>();

    }


    void Start()
    {
     

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void LoadingResourceFile(string strBundleName)
    {
        AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(
                        Path.Combine(Application.streamingAssetsPath +
                        "/01_AssetBundle",
                        strBundleName
                        ));

        if (myLoadedAssetBundle == null){
            Debug.Log("Failed to load AssetBundle! : " + CConfigMng.Instance._strResourceName[m_nAssetBundleCount]);
            return;
        }
        m_ListResource.Add(m_nAssetBundleCount, myLoadedAssetBundle.LoadAllAssets<Sprite>());
        m_nAssetBundleCount++;
    }

    public Sprite[] GetResource(int nNumber)
    {
        return m_ListResource[nNumber];
    }
    void LoadingResourceFile()
    {
        for(int i=0; i< CConfigMng.Instance._strResourceName.Length; i++)
        {
            Debug.Log(Path.Combine(Application.streamingAssetsPath + "/01_AssetBundle", CConfigMng.Instance._strResourceName[i]));
            AssetBundle myLoadedAssetBundle = AssetBundle.LoadFromFile(
                         Path.Combine(Application.streamingAssetsPath+
                         "/01_AssetBundle",
                         CConfigMng.Instance._strResourceName[i]
                         ));
           
            if (myLoadedAssetBundle == null){
                Debug.Log("Failed to load AssetBundle! : " + CConfigMng.Instance._strResourceName[i]);
                return;
            }
             m_ListResource.Add(i, myLoadedAssetBundle.LoadAllAssets<Sprite>());
        }
    }
}
;