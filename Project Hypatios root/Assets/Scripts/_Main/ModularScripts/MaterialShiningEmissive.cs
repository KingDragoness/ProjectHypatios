using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShiningEmissive : MonoBehaviour
{

    public List<Renderer> targetRenderers = new List<Renderer>();
    [ColorUsage(true, true)]
    public Color emissiveMin;
    [ColorUsage(true, true)]
    public Color emissiveMax;
    public float time = 1;

    private List<Material> targetMaterial = new List<Material>();

    private void Start()
    {
        foreach (var renderer in targetRenderers)
        {
            targetMaterial.Add(renderer.material);
        }

    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        float r = emissiveMax.r - emissiveMin.r;
        float g = emissiveMax.g - emissiveMin.g;
        float b = emissiveMax.b - emissiveMin.b;
        r = Mathf.Lerp(emissiveMin.r, emissiveMax.r, Mathf.PingPong(Time.time / time, 1));
        g = Mathf.Lerp(emissiveMin.g, emissiveMax.g, Mathf.PingPong(Time.time / time, 1));
        b = Mathf.Lerp(emissiveMin.b, emissiveMax.b, Mathf.PingPong(Time.time / time, 1));
       // r += emissiveMin.r;
      //  g += emissiveMin.g;
       // b += emissiveMin.b;


        foreach (Material m in targetMaterial)
        {
            Color newColor = m.GetColor("_EmissionColor");
            newColor.r = r;
            newColor.g = g;
            newColor.b = b;
            m.SetColor("_EmissionColor", newColor);
        }
    }

}
