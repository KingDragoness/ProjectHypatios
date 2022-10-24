using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMaterialChanger : MonoBehaviour
{
    public List<MeshRenderer> targetRenderers = new List<MeshRenderer>();
    public Color albedoColor;
    [ColorUsage(true, true)]
    public Color emissionColor;
    public bool isUsingAlbedo = true;
    public bool isUsingEmission = false;

    private List<Material> targetMaterial = new List<Material>();

    private void Start()
    {
        foreach (var renderer in targetRenderers)
        {
            targetMaterial.Add(renderer.material);
        }

        ResetMaterial();
    }

    private void Update()
    {
        //if (Mathf.RoundToInt(Time.time*10) % 2 == 0) return;
        //can be use later

        foreach (Material m in targetMaterial)
        {
            if (isUsingAlbedo) m.SetColor("_Color", albedoColor);
            if (isUsingEmission) m.SetColor("_EmissionColor", emissionColor);
        }
    }

    public void ResetMaterial()
    {
        foreach (Material m in targetMaterial)
        {
            if (isUsingAlbedo) m.SetColor("_Color", albedoColor);
            if (isUsingEmission) m.SetColor("_EmissionColor", emissionColor);
        }
    }

}
