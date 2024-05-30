using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIImageToggle : MonoBehaviour
{

    public GameObject[] _Toggle;
    public bool _bFirstEnable=false;
    // Start is called before the first frame update
    void Start()
    {
        IsOn(_bFirstEnable);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void IsOn(bool bEnable)
    {
        if (bEnable == true)
        {
            _Toggle[0].SetActive(false);
            _Toggle[1].SetActive(true);
        }
        else
        {
            _Toggle[0].SetActive(true);
            _Toggle[1].SetActive(false);
        }
    }
}
