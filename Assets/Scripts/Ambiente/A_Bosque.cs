using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class A_Bosque : MonoBehaviour
{
    [SerializeField]
    Transform jugador;


    [SerializeField]
    float maxRad = 70;
    [SerializeField]
    float center = 10;

    FMODUnity.StudioEventEmitter ambiente_bosque;

    // Start is called before the first frame update
    void Start()
    {
        ambiente_bosque = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    // Update is called once per frame
    void Update()
    {
        float distancia = Vector3.Distance(transform.position, jugador.position);
        float valor_volumen = 0;

        if (distancia <= maxRad)
        {
            //dentro
            if (distancia < center)
            {
                
                ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", 0);

                //return a;
            }
            else
            {
                valor_volumen = ((distancia - center) / maxRad);
                ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", valor_volumen);
                //Debug.Log("Dentro: " + valor_volumen);
                //return 1;
            }
        }
        else
        {
                ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", 1);
        }



        //if (valor_volumen >= 70)
        //    ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", 70);
        //else
        //    ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", valor_volumen);
    }
}
