using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SplineMappedEmitter : MonoBehaviour
{
    //todo smooth?
    public void TranslateEmitter(Vector3 pos)
    {
        transform.position = pos;
    }

    public void UpdateSpread(float value)
    {
        //splineMappedEventInstance.setParameterByName(spreadParameter, value);
    }
}