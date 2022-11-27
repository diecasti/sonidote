using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Casco : MonoBehaviour
{
    [SerializeField]
    Volume post_procesado;

    GameObject luz;
    Animator anim;
    bool abierto = true;
    bool luz_activa = true;

    LensDistortion Distorsion;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        luz = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        control_casco();
    }

    void control_casco()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            //configuracion_casco();

            if (abierto)
                anim.SetTrigger("close");
            else
                anim.SetTrigger("open");
            abierto = !abierto;
        }
        if(!abierto)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                luz.SetActive(luz_activa);
                luz_activa = !luz_activa;
            }
        }
    }

    void configuracion_casco()
    {
        LensDistortion lente_casco;

        post_procesado.profile.TryGet<LensDistortion>(out lente_casco);

        lente_casco.active = !abierto;
    }
}
