using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;


public class MobiusNetUI : MonoBehaviour
{

    public Vector2 border = new Vector2(78f, 32f); //pivot from top, left
    public Vector2 minimumWindowSize = new Vector2(120f, 60f); //pivot from top, left

    public RectTransform rt_Canvas;
    [Space]
    public List<MobiusApp> all_InstalledApps = new List<MobiusApp>();
    public List<MobiusApp_IconDashboard> all_OpenedMobiusApps = new List<MobiusApp_IconDashboard>();
    public List<MobiusOS_DesktopApp> all_DesktopIcons = new List<MobiusOS_DesktopApp>();
    public MobiusOS_DesktopApp desktopAppPrefab;
    public MobiusApp_IconDashboard dashboardIconPrefab;
    public Transform pivotDesktop;
    public Transform pivotDashboardStart;
    public AudioSource audio_ClickMouse;


    private Interact_MobiusNetworkPC _currentBench;

    public Interact_MobiusNetworkPC CurrentWorkbench { get => _currentBench; set => _currentBench = value; }

    public void SetShopScript(Interact_MobiusNetworkPC _shop)
    {
        _currentBench = _shop;
    }


    private bool hasStarted = false;

    private void Start()
    {
        desktopAppPrefab.gameObject.EnableGameobject(false);
        dashboardIconPrefab.gameObject.EnableGameobject(false);

        if (hasStarted == false) RefreshUI();
        hasStarted = true;
    }

    private void OnEnable()
    {

        if (hasStarted == false) return;
        RefreshUI();
    }

    public void RefreshUI()
    {

        InstallApps();
        ResetAppWindows();
        InitiateDesktopIcons();
    }


    private void OnDisable()
    {
        foreach (var app in all_InstalledApps)
        {
            if (app != null) Destroy(app.gameObject);
        }
        foreach (var desktopIcon in all_DesktopIcons)
        {
            if (desktopIcon != null) Destroy(desktopIcon.gameObject);
        }
        foreach (var startIcon in all_OpenedMobiusApps)
        {
            if (startIcon != null) Destroy(startIcon.gameObject);
        }

        all_InstalledApps.Clear();
        all_DesktopIcons.Clear();
        all_OpenedMobiusApps.Clear();

    }

    private void InstallApps()
    {
        foreach (var app in all_InstalledApps)
        {
            if (app != null) Destroy(app.gameObject);
        }

        all_InstalledApps.Clear();

        InstallApp(MobiusApp_SO.GetMobiusApp(MobiusApp_SO.DefaultApp.StockExchange));

    }

    private void ResetAppWindows()
    {
        foreach(var window in all_InstalledApps)
        {
            window.CloseWindow();
        }
    }

    private void InitiateDesktopIcons()
    {
        foreach(var desktopIcon in all_DesktopIcons)
        {
            if (desktopIcon != null) Destroy(desktopIcon.gameObject);
        }

        all_DesktopIcons.Clear();

        foreach(var app in all_InstalledApps)
        {
            var prefab = Instantiate(desktopAppPrefab, pivotDesktop);
            prefab.gameObject.SetActive(true);
            prefab.mobiusSO = app.connectedApp;
            prefab.appIcon.sprite = app.connectedApp.appLogo;
            prefab.label.text = app.connectedApp.appName;
            prefab.mobiusUI = this;
            all_DesktopIcons.Add(prefab);
        }
    }

    public void InstallApp(MobiusApp_SO _appSO)
    {
        var mobiusApp = Instantiate(_appSO.mobiusAppPrefab, transform);
        mobiusApp.transform.localScale = Vector3.one;
        mobiusApp.transform.rotation = Quaternion.identity;
        mobiusApp.connectedApp = _appSO;
        mobiusApp.minmax_AppLabelName.text = _appSO.appName;
        mobiusApp.minmax_AppIcon.sprite = _appSO.appLogo;
        mobiusApp.mobiusOS = this;

        all_InstalledApps.Add(mobiusApp);
    }

    private void Update()
    {
        if (Hypatios.Input.Fire1.triggered && Hypatios.Input.Fire1.IsPressed())
        {
            audio_ClickMouse?.Play();
        }

        foreach(var icon in all_OpenedMobiusApps)
        {
            if (icon == null) continue;
            if (icon.openedMobiusApp.gameObject.activeInHierarchy)
            {
                icon.i_Minimized.gameObject.EnableGameobject(true);
            }
            else
            {
                icon.i_Minimized.gameObject.EnableGameobject(false);
            }
        }
    }


    public void OpenWindowApp(MobiusApp_SO _appSO)
    {
        MobiusApp mobiusApp = all_InstalledApps.Find(x => x.connectedApp == _appSO);

        if (mobiusApp == null)
        {
            Debug.LogError("Error Mobius App not found!");
            return;
        }

        mobiusApp.OpenWindow();
        mobiusApp.transform.SetAsLastSibling();

        MobiusApp_IconDashboard startMenuIcon = all_OpenedMobiusApps.Find(x => x.openedMobiusApp == mobiusApp);

        if (startMenuIcon == null)
        {
            startMenuIcon = Instantiate(dashboardIconPrefab, pivotDashboardStart);
            startMenuIcon.gameObject.EnableGameobject(true);
            startMenuIcon.mobiusUI = this;
            startMenuIcon.transform.localScale = Vector3.one;
            startMenuIcon.transform.rotation = Quaternion.identity;
            startMenuIcon.openedMobiusApp = mobiusApp;
            startMenuIcon.Refresh();
            all_OpenedMobiusApps.Add(startMenuIcon);
        }
    }

    public void ExitApp(MobiusApp mobiusApp)
    {
        all_OpenedMobiusApps.RemoveAll(x => x == null);
        mobiusApp.rectTransform.sizeDelta = mobiusApp.connectedApp.minimumWindowSize;
        MobiusApp_IconDashboard startMenuIcon = all_OpenedMobiusApps.Find(x => x.openedMobiusApp == mobiusApp);

        if (startMenuIcon != null)
        {
            all_OpenedMobiusApps.Remove(startMenuIcon);
            Destroy(startMenuIcon.gameObject);
        }

        all_OpenedMobiusApps.RemoveAll(x => x == null);
    }




}
