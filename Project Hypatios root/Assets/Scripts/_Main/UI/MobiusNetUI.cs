using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public abstract class MobiusApp : MonoBehaviour
{

    public virtual void OpenWindow()
    {
        gameObject.EnableGameobject(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.EnableGameobject(false);
    }
}

public class MobiusNetUI : MonoBehaviour
{


    public List<MobiusApp> all_IntegratedApp = new List<MobiusApp>();
    public AudioSource audio_ClickMouse;
    public MobiusNetUI_StockExchange StockExchange;

    private Interact_MobiusNetworkPC _currentBench;

    public Interact_MobiusNetworkPC CurrentWorkbench { get => _currentBench; set => _currentBench = value; }

    public void SetShopScript(Interact_MobiusNetworkPC _shop)
    {
        _currentBench = _shop;
    }


    private bool hasStarted = false;

    private void Start()
    {

        if (hasStarted == false) RefreshUI();
        hasStarted = true;
    }

    private void OnEnable()
    {
        ResetAppWindows();
        if (hasStarted == false) return;
        RefreshUI();
    }

    private void ResetAppWindows()
    {
        foreach(var window in all_IntegratedApp)
        {
            window.CloseWindow();
        }
    }



    private void Update()
    {
        if (Hypatios.Input.Fire1.triggered && Hypatios.Input.Fire1.IsPressed())
        {
            audio_ClickMouse?.Play();
        }
    }

    #region Refresh UI

    public void RefreshUI()
    {


    }

    public void OpenWindowApp()
    {

    }

    #endregion

   

}
