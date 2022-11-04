using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveMaterialScript : MonoBehaviour
{

    public List<Renderer> targetRenderers = new List<Renderer>();
    public float dissolveSpeed = 10;

    private List<Material> dissolveMaterials = new List<Material>();
    private float _dissolveTime = 0;

    private void Start()
    {
        foreach(var renderer in targetRenderers)
        {
            dissolveMaterials.Add(renderer.material);
        }

        ResetMaterial();
    }

    private void Update()
    {
        _dissolveTime += Time.deltaTime * 0.1f * dissolveSpeed;

        foreach (Material m in dissolveMaterials)
        {
            m.SetFloat("_dissolve", _dissolveTime);
            m.SetFloat("_DissolveAmount", _dissolveTime);
        }
    }

    public void ResetMaterial()
    {
        float dissolve = 0;

        foreach (Material m in dissolveMaterials)
        {
            m.SetFloat("_dissolve", dissolve);
            m.SetFloat("_DissolveAmount", dissolve);
        }
    }

}
