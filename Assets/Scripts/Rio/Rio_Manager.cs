using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rio_Manager : MonoBehaviour
{
    // Posibles posiciones del emisor
    [SerializeField]
    Transform[] posiciones;

    // Emisor
    [SerializeField]
    GameObject rio;
    FMODUnity.StudioEventEmitter rio_emisor;

    // Jugador
    [SerializeField]
    Transform jugador;
    Vector3 ultima_pos_jugador;
    Vector3 pos_mas_cercana;
    float t = 0f;
    float duration = 3f;

    [SerializeField]
    float velocidad = 1f;

    //radios del rio
    [SerializeField]
    float maxRad = 30;
    float center = 10;
     
    void Start()
    {
        rio_emisor = rio.GetComponent<FMODUnity.StudioEventEmitter>();
        ultima_pos_jugador = jugador.position;
    }

    void Update()
    {
        // Solo hacemos calculos si el jugador se ha movido
        Vector3 actual_pos_jugador = jugador.position;
        if(actual_pos_jugador != ultima_pos_jugador)
        {
            // Actualizamos la ultima posicion
            ultima_pos_jugador = actual_pos_jugador;

            // Elegimos el punto mas cercano al que mover el emisor
            pos_mas_cercana = Vector3.zero;
            float distancia = float.MaxValue;
            for(int i = 0; i <  posiciones.Length; i++)
            {
                // Calculamos cual es la distancia mas cercana 
                float nueva_distancia = Vector3.Distance(posiciones[i].position, actual_pos_jugador);
                if (nueva_distancia < distancia)
                {
                    pos_mas_cercana = posiciones[i].position;
                    distancia = nueva_distancia;
                }
            }            
        }

        // Si no hemos llegado a la siguiente posicion, movemos el emisor
        Vector3 dir = (pos_mas_cercana - rio.transform.position);
        if (dir.magnitude > 0.5)
        {
            rio.transform.Translate(dir.normalized * Time.deltaTime * velocidad);
        }

        // Calculamos la espacialidad del emisor
        float distancia_con_jugador = Vector3.Distance(actual_pos_jugador, rio.transform.position);


        if (distancia_con_jugador <= maxRad)
        {
            //dentro
            if (distancia_con_jugador > center)
            {
                float a = 1.0f + ((center - distancia_con_jugador) / center);
                rio_emisor.EventInstance.setParameterByName("rio_espacio", a);
                //Debug.Log("Dentro: " + a);

                //return a;
            }
            else
            {
                rio_emisor.EventInstance.setParameterByName("rio_espacio", 1);
                //Debug.Log("Dentro");

                //return 1;
            }
        }
        else
        {
            rio_emisor.EventInstance.setParameterByName("rio_espacio", 0);
            //Debug.Log("Fuera");

        }
    }
}
