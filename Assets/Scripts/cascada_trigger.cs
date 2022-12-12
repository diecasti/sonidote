using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cascada_trigger : MonoBehaviour
{
    [SerializeField]
    A_Cascada elScript;

    private void OnTriggerEnter(Collider other)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", 1, true);

        elScript.inside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", 0, true);

        elScript.inside = false;
    }

}
