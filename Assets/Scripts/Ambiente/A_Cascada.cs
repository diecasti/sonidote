using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Cascada : MonoBehaviour
{
    [SerializeField]
    Transform jugador;

    [SerializeField]
    float maxRad = 20;
    [SerializeField]
    float center = 10;

    public bool inside = false;


    // Update is called once per frame
    void Update()
    {


        if (inside)
        {

            float distancia = Vector3.Distance(transform.position, jugador.position);
            float valor = 0;

            if (distancia <= maxRad)
            {
                //dentro
                if (distancia < center)
                {
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", 1, true);
                }
                else
                {
                    valor = 1 - ((distancia - center) / maxRad);
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", valor, true);
                }
            }
            else
            {
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", 0, true);
            }
        }
        else
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Cascada_In", 0, true);

        }
    }
}
