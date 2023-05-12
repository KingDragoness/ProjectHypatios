using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TestAudioVisualizer : MonoBehaviour
{
    [System.Serializable]
    public class Band
    {
        public Slider slider;

    }

    public AudioSource audioSource;
    public float updateStep = 0.1f;
    public float audioSpeed = 5f;
    public int sampleDataLength = 1024;
    public List<Band> bandVisuals = new List<Band>();
    public float buffer_f1 = 0.005f;
    public float buffer_f2 = 1.2f;
    public float pow_band = 1.1f;
    public int bandTotalToCreate = 16;
    public Slider prefabSlider;
    public Transform parent;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float currentLoudness;
    private float[] clipSampleData;
    private float[] _freqBands;
    private float[] _bandBuffers;
    private float[] _bufferDecrease;
    private bool hasInitialized = false;

    // Use this for initialization
    void Awake()
    {

        if (!audioSource)
        {
            return;
        }
        InitializeAudios();
    }

    private void OnEnable()
    {
        if (!audioSource)
        {
            return;
        }
        InitializeAudios();
    }

    private void InitializeAudios()
    {

        if (hasInitialized == true)
        {
            return;
        }

        for (int x = 0; x < bandTotalToCreate; x++)
        {
            Slider newSlider1 = Instantiate(prefabSlider, parent);
            newSlider1.gameObject.SetActive(true);
            Band band1 = new Band();
            band1.slider = newSlider1;
            bandVisuals.Add(band1);
        }

        clipSampleData = new float[sampleDataLength];
        _freqBands = new float[bandVisuals.Count];
        _bandBuffers = new float[_freqBands.Length];
        _bufferDecrease = new float[_freqBands.Length];
        hasInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasInitialized == false)
            return;

        if (audioSource == null)
            return;

        if (audioSource.clip == null)
            return;

        currentUpdateTime += Time.unscaledDeltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.GetSpectrumData(clipSampleData, 0, FFTWindow.Blackman);
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
        }

        currentLoudness = Mathf.MoveTowards(currentLoudness, clipLoudness, audioSpeed * Time.unscaledDeltaTime);


        int SampleSize = bandVisuals.Count;

        for (int x = 0; x < SampleSize; x++)
        {
            Band band = bandVisuals[x];
            float average = 0f;
            //1024 samples divided by 16 = 
            int sampleCount = sampleDataLength / SampleSize; //(int)Mathf.Pow(2, x) * 2;


            for(int j = 0; j < sampleCount; j++)
            {
                int index = (sampleCount * x) + (j);
                if (index > sampleDataLength) break;
                average += clipSampleData[index];
            }

            average /= sampleCount;
            float f1 = Mathf.Pow(x * 10f, pow_band);
            if (x == 0)
            {
                f1 = 2f;
            }
            _freqBands[x] = (average * 10) * f1;
        }

        BandBuffer();


        for (int x = 0; x < bandVisuals.Count; x++)
        {
            Band band = bandVisuals[x];
            band.slider.value = _bandBuffers[x];
        }

    }

    private void BandBuffer()
    {
        int SampleSize = bandVisuals.Count;

        for (int z = 0; z < SampleSize; ++z)
        {
            if (_freqBands[z] > _bandBuffers[z])
            {
                _bandBuffers[z] = _freqBands[z];
                _bufferDecrease[z] = buffer_f1;
            }

            if (_freqBands[z] < _bandBuffers[z])
            {
                _bandBuffers[z] -= _bufferDecrease[z];
                _bufferDecrease[z] *= buffer_f2;
            }
        }
    }
}
