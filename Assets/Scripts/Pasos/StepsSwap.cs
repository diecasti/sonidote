// Script extraido y adaptado del canal Natty Creations

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum tipo_pisada { HIERBA, TIERRA, MADERA, ROCA, AGUA };

public class StepsSwap : MonoBehaviour
{

    [SerializeField]
    Terrain terrain;
    [SerializeField]
     TextMeshProUGUI testo;

    TerrainChecker checker;

    string current_layer;

    // Start is called before the first frame update
    void Start()
    {
        checker = new TerrainChecker(); 
    }


    public tipo_pisada CheckLayers()
    {
        tipo_pisada material = tipo_pisada.HIERBA;
        
        // Lanzamos un raycast para ver que tenemos debajo
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            // Si existe el terreno
            if(hit.transform.GetComponent<Terrain>())
            {
                current_layer = checker.GetLayerName(transform.position, terrain);
            }
        }

        switch(current_layer)
        {
            case "grass_layer":
                material = tipo_pisada.HIERBA;
                break;
            case "dirt_layer":
                material = tipo_pisada.TIERRA;
                break;
            case "wood_layer":
                material = tipo_pisada.MADERA;
                break;
            case "rock_layer":
                material = tipo_pisada.ROCA;
                break;
            case "water_layer":
                material = tipo_pisada.AGUA;
                break;
        }

        testo.text = current_layer;
        return material;
    }
}
