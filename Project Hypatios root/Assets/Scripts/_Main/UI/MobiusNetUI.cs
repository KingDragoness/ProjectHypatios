using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class MobiusNetUI : MonoBehaviour
{

    public enum Mode
    {
        None,
        StockExchange
    }

    public Mode currentMode;
    public MobiusNetUI_StockExchange StockExchange;

    private bool hasStarted = false;

    private void Start()
    {

        if (hasStarted == false) RefreshUI();
        hasStarted = true;
    }

    private void OnEnable()
    {
        if (hasStarted == false) return;
        ChangeMode(0);
        RefreshUI();
    }


    #region Refresh UI

    public void RefreshUI()
    {


    }

    #endregion

    #region Modes

    public void ChangeMode(int _mode)
    {
        currentMode = (Mode)_mode;
        UpdateMode();
    }

    private void UpdateMode()
    {
        if (currentMode == Mode.None)
        {
            StockExchange.gameObject.SetActive(false);
        }

        if (currentMode == Mode.StockExchange)
        {
            if (StockExchange.gameObject.activeSelf == false) StockExchange.gameObject.SetActive(true);
        }
     
    }

    #endregion

}
