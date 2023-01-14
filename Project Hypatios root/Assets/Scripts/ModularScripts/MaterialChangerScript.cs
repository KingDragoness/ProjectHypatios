using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChangerScript : MonoBehaviour
{
    
    [System.Serializable]
    public class MaterialProperty
    {
        public Renderer targetRenderer;
        public int index;
        public Material material;
    }

    public List<MaterialProperty> materials = new List<MaterialProperty>();

    public void ChangeAllMaterials()
    {
        foreach(var mat in materials)
        {
            Material[] mats = mat.targetRenderer.sharedMaterials;
            mats[mat.index] = mat.material;
            mat.targetRenderer.sharedMaterials = mats;
        }
    }

    private void OnDestroy()
    {
        
    }

    public void ChangeMaterial(int indexSlot)
    {
        var mat = materials[indexSlot];
        Material[] mats = mat.targetRenderer.sharedMaterials;
        mats[mat.index] = mat.material;
        mat.targetRenderer.sharedMaterials = mats;
    }

    public void ChangeMaterialCustom(int indexSlot, Material _mat)
    {
        var mat = materials[indexSlot];
        Material[] mats = mat.targetRenderer.sharedMaterials;
        mats[mat.index] = _mat;
        mat.targetRenderer.sharedMaterials = mats;
    }
}
