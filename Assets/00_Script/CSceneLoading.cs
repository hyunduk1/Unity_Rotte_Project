using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CSceneLoading : MonoBehaviour
{
    [SerializeField]
    public Image _ProgressImage;
    public TMP_Text _CountText;
    public TMP_Text _ResourceName;

    void Awake()
    {
    }
    void Start()
    { 
        StartCoroutine("LoadingNextScene");
    }

    IEnumerator LoadingNextScene()
    {     
        int nTotalResource = CConfigMng.Instance._strResourceName.Length;
        int nCurrentCount = 0;
        bool bFinish = true;
        while (bFinish) {
            yield return null;
            if(nCurrentCount>=nTotalResource) {
                _ProgressImage.fillAmount = 1.0f;
                _CountText.text = nTotalResource.ToString() + " / " + nTotalResource.ToString();
                CUIPanelMng.Instance.StartContents();
                bFinish = false;
                Destroy(gameObject);
                yield return new WaitForSeconds(0.1f);
            }
            else{
                _CountText.text = nCurrentCount.ToString() + " / " + nTotalResource.ToString();
                _ResourceName.text = CConfigMng.Instance._strResourceName[nCurrentCount];

                _ProgressImage.fillAmount = (float)nCurrentCount / (float)nTotalResource;
                CResourceLodingMng.Instance.LoadingResourceFile(_ResourceName.text);
                nCurrentCount++;
            }
        }
    }
}

