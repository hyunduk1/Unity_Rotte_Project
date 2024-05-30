using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CShakeEffect : MonoBehaviour
{
    [SerializeField] GameObject[] shakeObjects;
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeDuration;

    private void Update()
    {
      
    }
    public void ShakeStart()
    {
        Shake(shakeObjects);
    }

    public void ResetShake()
    {
        foreach (var shakeObject in shakeObjects)
        {
            iTween.Stop(shakeObject);
        }
    }
    private void Shake(GameObject[] shakeObjects)
    {
        foreach (var shakeObject in shakeObjects)
        {
            // iTween
            iTween.ShakePosition(shakeObject, Vector3.one * shakeAmount, shakeDuration);
        }
    }
}
