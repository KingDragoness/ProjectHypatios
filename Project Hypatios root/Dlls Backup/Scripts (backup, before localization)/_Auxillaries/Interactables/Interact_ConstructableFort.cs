using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_ConstructableFort : MonoBehaviour
{

    public Fortification_WoodenPlank prefabTemplate;
    public Fortification_WoodenPlank currentFort;
    public GameObject touchable;

    private float _cooldownCheck = 0.2f;

    private void Update()
    {
        _cooldownCheck -= Time.deltaTime;
        if (_cooldownCheck > 0)
        {
            return;
        }

        _cooldownCheck = 0.2f;

        if (currentFort == null)
            touchable.gameObject.SetActive(true);
        else
            touchable.gameObject.SetActive(false);
    }

    public void BuildPlank()
    {
        var prefab1 = Instantiate(prefabTemplate, this.transform);
        prefab1.gameObject.SetActive(true);
        prefab1.transform.position = transform.position;
        prefab1.transform.rotation = transform.rotation;
        currentFort = prefab1;
    }

}
