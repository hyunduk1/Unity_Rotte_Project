using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PROTOCOL
{
    SND_START_MOVIE = 0,      // {"id":0}
    SND_CURRENT_FRAME=1,
    SND_HEART_BEAT=2,

    RCV_START_MOVIE = 0,      // {"id":0}
    RCV_CURRENT_FRAME = 1,
    RCV_HEART_BEAT = 2
}

