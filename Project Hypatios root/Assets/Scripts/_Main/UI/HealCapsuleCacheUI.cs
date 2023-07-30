using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class HealCapsuleCacheUI : MonoBehaviour
{

    public RectTransform prefabHealIcon;
    public float RefreshTime = 0.05f;
    private float _refreshTimer = 0.05f;

    public List<RectTransform> allHealCapsules = new List<RectTransform>();


    void Update()
    {
        _refreshTimer -= Time.deltaTime;
        
        if (_refreshTimer < 0f)
        {
            UpdateUI();
            _refreshTimer = RefreshTime;
        }
    }

    public void UpdateUI()
    {
        //only 3 icons
        //4 heal capsules
        int totalCapsules = Hypatios.Player.Health.CachedHealCapsules;

        foreach(var icon in allHealCapsules)
        {
            icon.gameObject.SetActive(false);
        }    

        for(int x = 0; x < totalCapsules; x++)
        {
            RectTransform newIcon = null;
           
            if (x >= allHealCapsules.Count)
            {
                newIcon = Instantiate(prefabHealIcon, transform);
                allHealCapsules.Add(newIcon);
            }
            else
            {
                newIcon = allHealCapsules[x];
            }

            newIcon.gameObject.SetActive(true);
        }

    }
}
