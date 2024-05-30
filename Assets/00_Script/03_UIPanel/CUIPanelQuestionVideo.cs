using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUIPanelQuestionVideo : MonoBehaviour
{

    public GameObject[] _AnimationObj;
    public float _NextEventAniTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("NextEventMotion", _NextEventAniTime);
    }
    void Update()
    {
        
    }


    public void NextEventMotion()
    {
        Destroy(_AnimationObj[0]);
        _AnimationObj[1].SetActive(true);
    }
}
