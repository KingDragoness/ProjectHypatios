using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_PowerGenerator : MonoBehaviour
{

    public Interact_Container container;
    public List<GameObject> lightings = new List<GameObject>();

    private bool isOff = false;

    private void Update()
    {
        if (isOff) return;
        if (container.inventory.SearchByID("Material_NuclearMaterial") == null)
        {
            foreach (var light in lightings)
            {
                light.gameObject.SetActive(false);
            }

            isOff = true;
        }
    }

}
