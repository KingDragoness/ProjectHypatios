using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Chamber6_RestobotAuto : MonoBehaviour
{

    public Chamber_Level6 chamberScript;
    public List<Chamber6_Piring> AllPirings = new List<Chamber6_Piring>();
    public GameObject restoBot;
    public GameObject dishCanvas;
    public Slider UI_dishTimer;
    public float CooldownCook = 10f;
    [ReadOnly] public bool IsRestobotActive = false;

    private float _cooldownCook = 0f;
    private float _updateScript = 0.1f;
    private bool anyPiringAvailable = false;
    private bool isAlreadyActivated = false;

    #region Updates

    public void Update()
    {
        UpdatePlateAvailable();

        if (IsRestobotActive == false) return;
        if (anyPiringAvailable == false) return;

        if (_cooldownCook > 0f)
        {
            UI_dishTimer.value = (_cooldownCook/CooldownCook);
            _cooldownCook -= Time.deltaTime;
            return;
        }


        CookFood();
        _cooldownCook = CooldownCook;
    }

    private void UpdatePlateAvailable()
    {
        if (_updateScript > 0f)
        {
            _updateScript -= Time.deltaTime;
        }
        else if (chamberScript.currentGamemode == Chamber_Level6.Gamemode.Ongoing)
        {
            anyPiringAvailable = false;
            if (restoBot.activeSelf)
            {
                if (isAlreadyActivated == false)
                {
                    dishCanvas.gameObject.SetActive(true);
                }
                IsRestobotActive = true;
                isAlreadyActivated = true;
            }

            foreach (var piring in AllPirings)
            {
                if (IsPlateAvailable(piring))
                {
                    anyPiringAvailable = true;
                }
            }

            _updateScript = 0.1f;
        }
    }

    #endregion

    public bool IsPlateAvailable(Chamber6_Piring piring)
    {
        if (piring.ingredients.Count > 0)
        {
            return false;
        }
        return true;
    }

    public Chamber6_Piring FindAnyFreePiring()
    {
        foreach (var piring in AllPirings)
        {
            if (IsPlateAvailable(piring))
            {
                return piring;
            }
        }

        return null;
    }

    private bool IsAnyValidOrder()
    {
        var listValid = chamberScript.allCustomers;
        listValid.RemoveAll(x => x.OrderTaken);

        foreach (var servo in chamberScript.allServos)
        {
            listValid.RemoveAll(x => servo.IsOrderMatch(x));
        }

        if (listValid.Count == 0)
        {
            return false;
        }

        return true;
    }

    [ContextMenu("Instant cook test")]
    public void CookFood()
    {
        Chamber6_Piring freePiring = FindAnyFreePiring();
        if (freePiring == null) return;
        var listValid = chamberScript.allCustomers;
        listValid.RemoveAll(x => x.OrderTaken);

        foreach (var servo in chamberScript.allServos)
        {
            listValid.RemoveAll(x => servo.IsOrderMatch(x));
        }

        if (listValid.Count == 0)
        {
            //Hypatios.Dialogue.QueueDialogue("There's no valid order.", "SYSTEM", 3);
            return;
        }

        Chamber6_Customer customer = listValid[Random.Range(0, chamberScript.allCustomers.Count)];

        foreach (var ingredient in customer.order.allRecipes)
        {
            freePiring.AddIngredient(ingredient);
        }


    }

    public void GrabPlate(Chamber6_Piring piring)
    {
        if (chamberScript.currentMode == Chamber_Level6.PiringMode.NotPlay)
        {
            chamberScript.mainPiring.ingredients.Clear();
            chamberScript.AmbilPiring();
            return;
        } else if (chamberScript.currentMode == Chamber_Level6.PiringMode.PiringNotTaken)
        {
            chamberScript.mainPiring.ingredients.Clear();
            chamberScript.AmbilPiring();
            chamberScript.mainPiring.Refresh();
        }

        if (chamberScript.mainPiring.HasIngredient(Chamber_Level6.Ingredient.Rice))
        {
            Hypatios.Dialogue.QueueDialogue("Empty your plate first.", "SYSTEM", 3);
            return;
        }

        chamberScript.mainPiring.ingredients.Clear();
        chamberScript.mainPiring.CopyIngredient(piring);
        chamberScript.mainPiring.Refresh();
        piring.ingredients.Clear();
        piring.Refresh();
    }

}
