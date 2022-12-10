using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCatcher : MonoBehaviour
{


    public GameObject[] objectives;
    public float[] intesity;
    public float maxRad = (1.0f/3.0f);
    public float center = (1.0f/6.0f);
    public FMODUnity.StudioEventEmitter emisor;
    Camera cam;

    FMOD.Studio.EventInstance signals;
    private
    bool active = false;



    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        intesity = new float[objectives.Length];
        emisor.Play();
        signals = emisor.EventInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            active = !active;
            signals.setParameterByName("CatcherOn", active ? 1 : 0.04f);
        }

        for (int i = 0; i < objectives.Length; i++)
        {
            //intesity[i] = angleDir(this.transform.forward, (this.transform.position - objectives[i].transform.position ).normalized, this.transform.up);
            //intesity[i] = CalculateAngle180_v3(this.transform.forward, (this.transform.position - objectives[i].transform.position).normalized);
            intesity[i] = worldToView(objectives[i].transform.position);
            signals.setParameterByName("s" + (i + 1), intesity[i]);
        }

    }

    public float worldToView(Vector3 posObject)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(posObject);

        float result = 1.0f;
        float x = 0, y = 0;


        //dentro de la camara?
        if (true)
        { 
            //centro de la camara
            float dist = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(viewPos.x, viewPos.y));

            //la distancia estara normalizada, entre 0, y root(2) 1.414
            //          con un tercio de la pantalla esta bien 1/3f

            if (dist <= maxRad)
            {
                if (dist > center)
                {
                    float a = 1.0f + ((center - dist)/ center);
                    return a;
                }
                else
                {
                    return 1;
                }
            }

        }

        return 0;
    }
}
