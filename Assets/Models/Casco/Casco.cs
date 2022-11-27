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

    FMOD.Studio.EventInstance abrir;
    FMOD.Studio.EventInstance cerrar;
    FMOD.Studio.EventInstance respiracion;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        luz = transform.GetChild(0).gameObject;
        abrir = FMODUnity.RuntimeManager.CreateInstance("event:/abrir_casco");
        cerrar = FMODUnity.RuntimeManager.CreateInstance("event:/cerrar_casco");
        respiracion = FMODUnity.RuntimeManager.CreateInstance("event:/respirar");
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
            {
                anim.SetTrigger("close");
            }
            else
            {
                anim.SetTrigger("open");
            }
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

    void sonido_abrir()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/abrir_casco");
        respiracion.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    void sonido_cerrar()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/cerrar_casco");
        respiracion.start();
    }
}
