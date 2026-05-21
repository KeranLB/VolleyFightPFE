using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LightMaskManager : MonoBehaviour
{
    public static List<LightSource> allLights = new List<LightSource>();

    private Vector4[] positions = new Vector4[8];
    private Vector4[] colors = new Vector4[8];
    private Vector4[] paramsList = new Vector4[8];

    void Update()
    {
        int count = Mathf.Min(allLights.Count, 8);

        // Si aucune lumière, on informe le shader pour éviter les restes visuels
        Shader.SetGlobalInt("_GlobalLightCount", count);

        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            positions[i] = allLights[i].transform.position;

            // On pack la couleur et l'intensité d'émission dans un seul Vector4
            colors[i] = new Vector4(
                allLights[i].volumeColor.r,
                allLights[i].volumeColor.g,
                allLights[i].volumeColor.b,
                allLights[i].emissionIntensity
            );

            // Rayon et Douceur
            paramsList[i] = new Vector4(allLights[i].radius, allLights[i].softness, 0, 0);
        }

        Shader.SetGlobalVectorArray("_GlobalLightPositions", positions);
        Shader.SetGlobalVectorArray("_GlobalLightColors", colors);
        Shader.SetGlobalVectorArray("_GlobalLightParams", paramsList);
    }
}