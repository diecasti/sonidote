using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCatcher : MonoBehaviour
{


    public GameObject[] objectives;
    public float[] intesity;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        intesity = new float[objectives.Length];
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objectives.Length; i++)
        {
            //intesity[i] = angleDir(this.transform.forward, (this.transform.position - objectives[i].transform.position ).normalized, this.transform.up);
            //intesity[i] = CalculateAngle180_v3(this.transform.forward, (this.transform.position - objectives[i].transform.position).normalized);
            intesity[i] = worldToView(objectives[i].transform.position);
        }
        //Debug.Log(intesity[0]);
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
        float x = 0, y = 0 ;
       
        if (viewPos.x > 0.3f && viewPos.x < 0.7f)
        {
            if (viewPos.x > 0.4f && viewPos.x < 0.6f)
            {
                x = 1.0f;
            }
            else
            {
                if (viewPos.x < 0.4f)
                {
                    x = (viewPos.x - 0.3f )/ (0.4f - 0.3f);
                }
                else if (viewPos.x > 0.6f)
                {
                    x = (viewPos.x - 0.7f )/ (0.6f - 0.7f);
                }
            }
        }

        if (viewPos.y > 0.3f && viewPos.y < 0.7f)
        {
            if (viewPos.y > 0.4f && viewPos.y < 0.6f)
            {
                y = 1.0f;
            }
            else
            {
                if (viewPos.y < 0.4f)
                {
                    y = (viewPos.y - 0.3f) / (0.4f - 0.3f);
                }
                else if (viewPos.y > 0.6f)
                {
                    y = (viewPos.y - 0.7f) / (0.6f - 0.7f);
                }
            }
        }

        return (result * x * y);
    }
}
