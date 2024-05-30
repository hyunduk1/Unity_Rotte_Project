using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIPanelTop : MonoBehaviour
{
    public GameObject[] _TypeVideo;
    public GameObject[] _TalkBox;

    // Start is called before the first frame update
    void Start()
    {
        TypeVideo();
        _TalkBox[Random.Range(0, 2)].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TypeVideo()
    {
        _TypeVideo[CConfigMng.Instance._nContentsType].SetActive(true);
    }
}
