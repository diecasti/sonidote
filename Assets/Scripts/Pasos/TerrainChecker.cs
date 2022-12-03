// Script extraido y adaptado del canal Natty Creations

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChecker : MonoBehaviour
{
    private float[] GetTextureMix(Vector3 player_pos, Terrain t)
    {
        // Obtenemos la indo del terreno
        Vector3 terrain_pos = t.transform.position;
        TerrainData terrain_data = t.terrainData;

        // Posicion del jugador relativa al terreno
        int mapX = Mathf.RoundToInt((player_pos.x - terrain_pos.x) / terrain_data.size.x * terrain_data.alphamapWidth);
        int mapZ = Mathf.RoundToInt((player_pos.z - terrain_pos.z) / terrain_data.size.z * terrain_data.alphamapHeight);

        // Array de 3 dimensiones
        // 1 y 2 representan las coordenadas
        // 3 representa la textura que tiene esa coordenada del mapa
        float[,,] splatMapData = terrain_data.GetAlphamaps(mapX, mapZ, 1, 1);

        // Nos guardamos en un array solo la ultima dimension
        float[] cellmix = new float[splatMapData.GetUpperBound(2) + 1];
        for (int i = 0; i < cellmix.Length; i++)
        {
            cellmix[i] = splatMapData[0, 0, i];
        }

        return cellmix;

    }

    public string GetLayerName(Vector3 player_pos, Terrain t)
    {
        // Obtenemos el array de texturas 
        float[] cellMix = GetTextureMix(player_pos, t);
        float strongest = 0;
        int maxIndex = 0;

        // Lo recorremos guardando cual es la textura que predomina sobre las otras en la posicion del jugador
        for(int i = 0; i < cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }

        // Devolvemos el nombre de la layer sobre la que esta el jugador
        return t.terrainData.terrainLayers[maxIndex].name;
    }
}
