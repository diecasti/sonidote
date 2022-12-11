using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cascada_trigger : MonoBehaviour
{
    [SerializeField]
    A_Cascada elScript;

    private void OnTriggerEnter(Collider other)
    {
        elScript.inside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        elScript.inside = false;
    }

}
