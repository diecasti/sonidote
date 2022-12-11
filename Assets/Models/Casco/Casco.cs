using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Casco : MonoBehaviour
{
    [SerializeField]
    Volume post_procesado;

    [SerializeField]
    GameObject luz;

    Animator anim;
    bool abierto = false;
    bool luz_activa = false;

    LensDistortion Distorsion;

    FMOD.Studio.EventInstance respiracion;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        respiracion = FMODUnity.RuntimeManager.CreateInstance("event:/personaje/Aparejos/respirar");
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
            configuracion_casco();
            abierto = !abierto;
            anim.SetBool("cerrado",abierto);
        }
        if (abierto)
        {
            // Linterna
            if(Input.GetKeyDown(KeyCode.R))
            {
                luz_activa = !luz_activa;
                luz.SetActive(luz_activa);
                sonido_linterna();
            }
        }
    }

    void configuracion_casco()
    {
        LensDistortion lente_casco;

        post_procesado.profile.TryGet<LensDistortion>(out lente_casco);

        lente_casco.active = abierto;
    }

    void sonido_abrir()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/personaje/Aparejos/abrir_casco");
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("casco",0,true);
        respiracion.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    void sonido_cerrar()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("casco",1,true);
        FMODUnity.RuntimeManager.PlayOneShot("event:/personaje/Aparejos/cerrar_casco");
        respiracion.start();
    }

    void sonido_linterna()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/personaje/Aparejos/linterna");
    }
}
