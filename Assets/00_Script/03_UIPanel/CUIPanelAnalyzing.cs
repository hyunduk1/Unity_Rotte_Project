using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIPanelAnalyzing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("AutoNextPanelEffect", CConfigMng.Instance._fAlnaylizingTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AutoNextPanelInsert()
    {
        CUIPanelMng.Instance.DrawNaviInfoCategory(NAVI_STATE.STATE_CATEGORY_RESULT);
        CUIPanelMng.Instance.InsertNextEventPanel((int)WINOOW.PANEL_RESOULT);
    }
    public void AutoNextPanelEffect()
    {
        Invoke("AutoNextPanelInsert", 0.5f);
        CUIPanelMng.Instance.InsertEffectPanel();
    }
}
