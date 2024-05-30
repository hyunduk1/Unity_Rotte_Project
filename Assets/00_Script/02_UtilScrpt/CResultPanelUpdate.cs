using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CResultPanelUpdate : MonoBehaviour
{
    
    public GameObject[] _TypeObject;



    // Start is called before the first frame update
    void Start()
    {
        _TypeObject[CMainMng.Instance.GetResultType()].SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {

    }

}
