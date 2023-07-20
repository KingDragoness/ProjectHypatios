using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ModularUI_LineGraph : MonoBehaviour
{

    //560 / 30 (space) = 18 total dots

    public Wire wirePrefab;
    public RectTransform dotPrefab;
    public Text labelDividerPrefab;
    public Text labelVerticalPrefab;
    public int dot_offsetX = 10;
    public int dot_spaceX = 30;
    public int minY = 30;
    public int maxY = 230;
    public int verticalLabelAmount = 6;
    public Transform parentDot;
    public Transform parentWire;
    public Transform parentLabels;
    public Transform parentVerticalLabels;

    public List<RectTransform> allDots = new List<RectTransform>();
    public List<Wire> allWires = new List<Wire>();
    public List<Text> allLabelDivider = new List<Text>();
    public List<Text> allLabelVerticals = new List<Text>();

    private void Start()
    {
        wirePrefab.gameObject.SetActive(false);
        dotPrefab.gameObject.SetActive(false);
        labelDividerPrefab.gameObject.SetActive(false);
        labelVerticalPrefab.gameObject.SetActive(false);
    }

    public void ShowLineGraph(float[] data, string[] labelString)
    {
        foreach(var dot in allDots)
        {
            if (dot != null) Destroy(dot.gameObject);
        }
        foreach (var wire in allWires)
        {
            if (wire != null) Destroy(wire.gameObject);
        }
        foreach (var label in allLabelDivider)
        {
            if (label != null) Destroy(label.gameObject);
        }
        foreach (var label in allLabelVerticals)
        {
            if (label != null) Destroy(label.gameObject);
        }

        allDots.Clear();
        allWires.Clear();
        allLabelDivider.Clear();
        allLabelVerticals.Clear();

        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        foreach(var value in data)
        {
            if (value > maxValue)
                maxValue = value;

            if (value < minValue)
                minValue = value;
        }

        for(int x = 0; x < data.Length; x++)
        {
            float yPos = data[x];
            {
                yPos = Mathf.InverseLerp(minValue, maxValue, data[x]);
                yPos = Mathf.Lerp(minY, maxY, yPos);
            }
            RectTransform dot1 = Instantiate(dotPrefab, parentDot);
            dot1.gameObject.SetActive(true);
            dot1.anchoredPosition = new Vector2(dot_offsetX + (x * dot_spaceX), yPos);
            allDots.Add(dot1);

            if (string.IsNullOrEmpty(labelString[x]) == false)
            {
                var label = Instantiate(labelDividerPrefab, parentLabels);
                label.gameObject.SetActive(true);
                label.rectTransform.anchoredPosition = new Vector2(dot_offsetX + (x * dot_spaceX), label.rectTransform.anchoredPosition.y);
                label.text = labelString[x];
                allLabelDivider.Add(label);
            }
        }

        for (int x = 1; x < allDots.Count; x++)
        {
            var dot = allDots[x];
            var prevDot = allDots[x - 1];

            Wire wire = Instantiate(wirePrefab, parentWire);
            wire.gameObject.SetActive(true);
            wire.origin = prevDot.transform;
            wire.target = dot.transform;
            allWires.Add(wire);
        }

        //Generate vertical markers
        float gapVertical = (maxY - minY) / verticalLabelAmount;
        float perValueIncrement = (maxValue - minValue) / verticalLabelAmount;

        for(int x = 0; x <= verticalLabelAmount; x++)
        {
            float y = minY + (gapVertical * x);
            float value = minValue + (perValueIncrement * x);
            if (value < 100)
            {
                value = Mathf.Round(value * 10) / 10;
            } else { value = Mathf.Round(value); }
            var label = Instantiate(labelVerticalPrefab, parentVerticalLabels);
            label.gameObject.SetActive(true);
            label.rectTransform.anchoredPosition = new Vector2(label.rectTransform.anchoredPosition.x, y);
            label.text = $"{value}";
            allLabelVerticals.Add(label);
        }

    }

}
