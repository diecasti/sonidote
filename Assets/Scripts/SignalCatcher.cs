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


        //si el objeto esat justo enfrente del jugador, tiramos un raycast cortito para que lo pille (ya que las comprobaciones de posicion camara se hacen con el punto de referencia de origen del objeto y ese puede caer fuera cuando nos acercamos demasiado)
        //tira el ray cast, dios me meo tengo un problema de uretra seguro no paro de ir al baño
        // Creates a Ray from the center of the viewport
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * 10);

        if (Physics.Raycast(ray, 10))
        {
            // Hit Something closer than 10 units away
            RaycastHit[] hits = Physics.RaycastAll(ray, 10);
            foreach (RaycastHit obj in hits)
            {
                int i = 0;
                foreach (GameObject gmobj in objectives)
                {
                    if (gmobj == obj.transform.gameObject)
                    {
                        signals.setParameterByName("s" + (i + 1), 1);

                    }

                    i++;
                }

                //Destroy(obj.transform.gameObject);
            }

        }

    }

    public float worldToView(Vector3 posObject)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(posObject);

        float result = 1.0f;
        float x = 0, y = 0;


        //enfrente de la camara?
        if (viewPos.z > 0)
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
