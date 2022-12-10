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

    float angleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        return dir;
    }
    public static float CalculateAngle180_v3(Vector3 fromDir, Vector3 toDir)
    {
        Vector3 angle = Quaternion.FromToRotation(fromDir, toDir).eulerAngles;

        //if (angle > 180) { angle -= 360f; }
       /* Debug.Log("x: " + angle.x);
        Debug.Log("y: " +  angle.y);
        Debug.Log("z: " + angle.z);*/

        return 0;
    }

    public float worldToView(Vector3 posObject)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(posObject);

        float result = 1.0f;
        float x = 0, y = 0;


        //dentro de la camara?
        if (viewPos)
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





        //if (viewPos.x > 0.3f && viewPos.x < 0.7f)
        //{
        //    if (viewPos.x > 0.4f && viewPos.x < 0.6f)
        //    {
        //        x = 1.0f;
        //    }
        //    else
        //    {
        //        if (viewPos.x < 0.4f)
        //        {
        //            x = (viewPos.x - 0.3f )/ (0.4f - 0.3f);
        //        }
        //        else if (viewPos.x > 0.6f)
        //        {
        //            x = (viewPos.x - 0.7f )/ (0.6f - 0.7f);
        //        }
        //    }
        //}

        //if (viewPos.y > 0.3f && viewPos.y < 0.7f)
        //{
        //    if (viewPos.y > 0.4f && viewPos.y < 0.6f)
        //    {
        //        y = 1.0f;
        //    }
        //    else
        //    {
        //        if (viewPos.y < 0.4f)
        //        {
        //            y = (viewPos.y - 0.3f) / (0.4f - 0.3f);
        //        }
        //        else if (viewPos.y > 0.6f)
        //        {
        //            y = (viewPos.y - 0.7f) / (0.6f - 0.7f);
        //        }
        //    }
        //}

        //return (result * x * y);
    }
}
