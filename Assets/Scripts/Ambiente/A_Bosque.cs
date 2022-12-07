using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class A_Bosque : MonoBehaviour
{
    [SerializeField]
    Transform jugador;

    [SerializeField]
    TextMeshProUGUI distancia_t;

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
        float valor_volumen = distancia.magnitude;
        distancia_t.text = valor_volumen.ToString();

        if(valor_volumen >= 70)
            ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", 70);
        else
            ambiente_bosque.EventInstance.setParameterByName("distancia_bosque", valor_volumen);
    }
}
