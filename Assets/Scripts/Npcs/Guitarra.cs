using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guitarra : MonoBehaviour
{
    [SerializeField]
    FMODUnity.StudioEventEmitter emisor_guitarra;

    bool apagada = false;
    private float timer;
    private float tiempo_guitarra = 3;


    // Start is called before the first frame update
    void Start()
    {
        timer = tiempo_guitarra;
    }

    // Update is called once per frame
    void Update()
    {
        if(apagada)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                apagada = false;
                emisor_guitarra.Play();
                timer = tiempo_guitarra;
            }
        }
    }

    public void apagar_guitarra()
    {
        emisor_guitarra.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        apagada = true;
    }
}
