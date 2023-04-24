using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageOutputterUI : MonoBehaviour
{

    public DamgeOutputTextPrefabUI textPrefabUI;
    public Vector2 spawnDistanceRangeX;
    public Vector2 spawnDistanceRangeY;

    public static DamageOutputterUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void DisplayText(float number)
    {
        var prefab1 = Instantiate(textPrefabUI, transform);
        var rt1 = prefab1.GetComponent<RectTransform>();

        Vector3 v1 = rt1.anchoredPosition;
        v1.x = Random.Range(spawnDistanceRangeX.x, spawnDistanceRangeX.y);
        v1.y = Random.Range(spawnDistanceRangeY.x, spawnDistanceRangeY.y);

        rt1.localScale = Vector3.one;
        rt1.anchoredPosition = v1;
        prefab1.text.text = Mathf.RoundToInt(number).ToString();
        prefab1.gameObject.SetActive(true);

        Destroy(prefab1, 3f);
    }

}
