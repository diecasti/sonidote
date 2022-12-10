using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Hablar_Manager : MonoBehaviour
{
    [SerializeField]
    Camera camara;

    [SerializeField]
    GameObject[] nombres_textos;

    [SerializeField]
    GameObject[] conversaciones;

    [SerializeField]
    GameObject tuto_hablar;

    [SerializeField]
    private float distancia_minima = 5;

    bool hay_texto = false;
    bool hay_conversacion = false;
    bool se_puede_tuto = true;
    private float timer = 0;
    private float c_time = 5;


    void Start()
    {
        for(int i = 0; i <  nombres_textos.Length; i++)
        {
            nombres_textos[i].SetActive(false);

        }
        timer = c_time;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camara.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            // Miramos que hayamos chocado con un astronauta
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Astronaut"))
            {
                // Comprobamos que este dentro de un radio
                float distancia = (transform.position - hit.transform.position).magnitude;
                if ((distancia <= distancia_minima))
                {
                    hay_texto = true;
                    if (se_puede_tuto)
                        tuto_hablar.SetActive(true);

                    // Comprobamos con que astronauta ha impactado
                    string tag_name = hit.transform.tag;
                    FMODUnity.StudioEventEmitter emisor_voz = hit.transform.gameObject.GetComponent<FMODUnity.StudioEventEmitter>();

                    // Dependiendo del tag del astronauta hacemos unas cosas u otras
                    int indice_astronauta = 0;
                    switch (tag_name)
                    {
                        // Hoguera bosque
                        case "a1":
                            Debug.Log("aricon1");
                            indice_astronauta = 0;
                            break;

                        // Guitarra
                        case "a2":
                            Debug.Log("aricon2");
                            indice_astronauta = 1;
                            break;

                        // Hoguera pueblo
                        case "a3":
                            indice_astronauta = 2;
                            break;

                        // Pueblo
                        case "a4":
                            indice_astronauta = 3;
                            break;

                        // Roca
                        case "a5":
                            indice_astronauta = 4;
                            break;
                    }
                    nombres_textos[indice_astronauta].SetActive(true);

                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        if (!emisor_voz.IsPlaying())
                        {
                            emisor_voz.Play();
                            emisor_voz.EventInstance.setParameterByName("num_conversacion", indice_astronauta);
                            conversaciones[indice_astronauta].SetActive(true);
                            hay_conversacion = true;
                            se_puede_tuto = false;
                            tuto_hablar.SetActive(false);
                        }
                    }
                }
            }
            // resetear textos y conversaciones
            else if (hay_texto)
            {
                for (int i = 0; i < nombres_textos.Length; i++)
                {
                    nombres_textos[i].SetActive(false);

                }
                tuto_hablar.SetActive(false);
                hay_texto = false;
            }
            if(hay_conversacion)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    for (int i = 0; i < conversaciones.Length; i++)
                    {
                        conversaciones[i].SetActive(false);

                    }
                    timer = c_time;
                    hay_conversacion = false;
                    se_puede_tuto = true;
                }
            }


        }

        
        
    }
}
