using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class AlcoholEffect : MonoBehaviour
{

    public UnityStandardAssets.ImageEffects.MotionBlur motionblur;
    public PostProcessVolume postFX_75Meter;
    public cameraScript cameraScript;
    public AudioLowPassFilter lowPassFilter;
    public float Meter25_DizzyCam = 2f;
    public float Meter50_DizzyCam = 5f;
    public float Meter75_DizzyCam = 10f;
    public float xScale_25 = 1f;
    public float xScale_50 = 5f;
    public float xScale_75= 10f;

    void Update()
    {
        float heightScale = 0f;
        float xScale = 0f;

        float f = Hypatios.Player.Health.alcoholMeter / 100f;
        postFX_75Meter.weight = Mathf.Lerp(0f, 1f, f);

        if (Hypatios.Player.Health.alcoholMeter > 75f)
        {
            motionblur.enabled = true;
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = 450f;
            postFX_75Meter.gameObject.SetActive(true);
            heightScale = Meter75_DizzyCam;
            xScale = xScale_75;
        }
        else if (Hypatios.Player.Health.alcoholMeter > 50f)
        {
            motionblur.enabled = true;
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = 900f;
            postFX_75Meter.gameObject.SetActive(true);
            heightScale = Meter50_DizzyCam;
            xScale = xScale_50;

        }
        else if (Hypatios.Player.Health.alcoholMeter > 25f)
        {
            motionblur.enabled = true;
            lowPassFilter.enabled = true;
            lowPassFilter.cutoffFrequency = 1500f;
            postFX_75Meter.gameObject.SetActive(false);
            heightScale = Meter25_DizzyCam;
            xScale = xScale_25;

        }

        else
        {
            motionblur.enabled = false;
            lowPassFilter.enabled = false;
            lowPassFilter.cutoffFrequency = 10000f;
            postFX_75Meter.gameObject.SetActive(false);
            heightScale = 0;
        }

        float height = heightScale * Mathf.PerlinNoise(Time.time * xScale, 0.0f);

        cameraScript.externalX = height - (heightScale/ 2f);
        cameraScript.externalY = height - (heightScale / 2f);
    }
}
