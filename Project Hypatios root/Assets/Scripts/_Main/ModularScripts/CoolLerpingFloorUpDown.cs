using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolLerpingFloorUpDown : MonoBehaviour
{

    public GameObject prefab;
    public float dist = 1.2f;
    public int x_Count = 1;
    public int y_Count = 1;
    public float amplitude = 0.5f;
    public float amplitudeRandom = 4f;
    public float frequency = 1f;
    public float perlinEffect = 1f;
    public float rowEffect = 1f;

    private List<GameObject> allPrefabs = new List<GameObject>();
    private List<float> allCube_Height = new List<float>();
    private float time = 1f;

    private Texture2D noiseTex;
    private Color[] pix;

    private void Start()
    {
        for (int x = 0; x < x_Count; x++)
        {
            for (int y = 0; y < y_Count; y++)
            {
                GameObject go = Instantiate(prefab, transform);
                Vector3 pos = transform.position;
                pos.x += x * dist;
                pos.z += y * dist;

                go.transform.position = pos;
                allPrefabs.Add(go);

                allCube_Height.Add(Random.Range(0f, amplitudeRandom));
                float random1 = Random.Range(0f, 1f);

                if (random1 > 0.6f)
                {
                    go.gameObject.SetActive(false);
                }
            }

        }
        GeneratePerlin();
    }

    private void GeneratePerlin()
    {
        noiseTex = new Texture2D(x_Count, y_Count);
        pix = new Color[noiseTex.width * noiseTex.height];
    }

    private bool directionForward = false;

    private void Update()
    {
        if (Time.timeScale <= 0f) return;

 
   

        int index = 0;

        float xStart = 0f;
        float Tau = 2f * Mathf.PI;
        float xFinish = Tau;

        //do some lerp
        for (int x = 0; x < x_Count; x++)
        {
            for (int y = 0; y < y_Count; y++)
            {
                int row = x + 1;
                int column = y + 1;
                int prevBlock = (x * y_Count); //2 * 3 = 6
                int i_index = prevBlock + y;

                float xCoord = x / (float)x_Count;
                float yCoord = y / (float)y_Count;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                float peakAmplitude = allCube_Height[i_index];

                var cube = allPrefabs[i_index];
                float progress = (float)i_index / ((x_Count * y_Count) - 1);
                float xPos = Mathf.Lerp(xStart, xFinish, progress);
                float yPos = amplitude * peakAmplitude * Mathf.Sin((Tau*frequency* xPos) + (Time.timeSinceLevelLoad + (x * rowEffect) + (sample * perlinEffect)));
                yPos += transform.position.y;
                cube.transform.position = new Vector3(cube.transform.position.x, yPos, cube.transform.position.z);
            }

        }

        //foreach (var cube in allPrefabs)
        //{
        //    float yPos = Mathf.Lerp(0, allCube_Height[index], time);
        //    cube.transform.position = new Vector3(cube.transform.position.x, yPos, cube.transform.position.z);
        //    index++;

        //}

    }

}
