using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricFogAndMist;

public class RetardUnity_MimicVolumetric : MonoBehaviour
{

    public VolumetricFog toCopyFrom;
    public VolumetricFog volumetricFog;


    private void OnEnable()
    {
        VolumetricFogProfile blankProfile = new VolumetricFogProfile();
        blankProfile.Save(toCopyFrom);
        volumetricFog.profile = blankProfile;
    }

}
