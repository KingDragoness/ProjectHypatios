using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Chamber6_Piring : MonoBehaviour
{
    public List<Chamber_Level6.Ingredient> ingredients = new List<Chamber_Level6.Ingredient>();
    public AudioSource audio_piring;
    public WaypointsFree.WaypointsTraveler waypointScript;
    public delegate void onTransferPlate(Transform owner);
    public event onTransferPlate OnTransferPlate;

    public GameObject[] recipeVisuals;

    private void Start()
    {
        Refresh();
    }

    public bool HasIngredient(Chamber_Level6.Ingredient ingredient)
    {
        return ingredients.Contains(ingredient);
    }

    public void ChangeOwnership(Transform owner)
    {
        OnTransferPlate?.Invoke(owner);
    }

    [ContextMenu("Refresh Visuals")]
    public void Refresh()
    {
        foreach(var model3d in recipeVisuals)
        {
            model3d.gameObject.SetActive(false);
        }

        foreach(var recipe in ingredients)
        {
            int i = (int)recipe;
            recipeVisuals[i].gameObject.SetActive(true);
        }
    }

    public void CopyIngredient(Chamber6_Piring piring)
    {
        ingredients.AddRange(piring.ingredients);
        Refresh();
    }

    public void AddIngredient(Chamber_Level6.Ingredient ingredient)
    {


        if (HasIngredient(ingredient)) return;

        ingredients.Add(ingredient);

        Refresh();
    }

    public void ClearIngredients()
    {
        ingredients.Clear();

        Refresh();
    }
}
