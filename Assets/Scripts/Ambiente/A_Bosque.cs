using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Bosque : MonoBehaviour
{
    [SerializeField]
    Transform jugador;

    FMODUnity.StudioEventEmitter ambiente_bosque;

    // Start is called before the first frame update
    void Start()
    {
        ambiente_bosque = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distancia = transform.position - jugador.position;
        if (distancia.magnitude < 5)
        {
            Debug.Log("DEBeria ser 2d");
            ambiente_bosque.EventInstance.setParameterByName("distancia", 0);
        }
    }
}
