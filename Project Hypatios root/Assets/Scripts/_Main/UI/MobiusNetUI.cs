using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public abstract class MobiusApp : MonoBehaviour
{

    public MobiusNetUI mobiusOS;
    public MobiusApp_SO connectedApp;
    public RectTransform rectTransform;

    private bool _initiateDragWindow = false;
    private bool _initiateResizing = false;
    private Vector2 _deltaDragPos;
    private float _cooldownDrag = 0.2f;
    private float _cooldownResize = 0.2f;

    public virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void OpenWindow()
    {
        gameObject.EnableGameobject(true);
    }

    public virtual void CloseWindow()
    {
        gameObject.EnableGameobject(false);
    }

    public void Maximize()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 sizeWindow = rectTransform.sizeDelta;
        Vector2 canvasSize = mobiusOS.rt_Canvas.sizeDelta;
        Vector2 netSizeLimit = canvasSize - sizeWindow;

        pos.x = mobiusOS.border.x;
        pos.y = (canvasSize.y - mobiusOS.border.y);
        rectTransform.anchoredPosition = pos;

        bool isMaximized = IsWindowMaximized();



        if (isMaximized == false)
        {
            ResizeWindow(canvasSize);
        }
        else
        {
            ResizeWindow(connectedApp.minimumWindowSize);
        }
    }

    public void Exit()
    {
        gameObject.EnableGameobject(false);
        mobiusOS.ExitApp(this);
    }

    public void StartDrag()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        var mousePos = Hypatios.UI.GetMousePositionScaled();
        Vector2 mousePosOnCanvas = new Vector2(mousePos.x, mousePos.y);

        _deltaDragPos = pos - mousePosOnCanvas;
        //_deltaDragPos.y = -_deltaDragPos.y;
        _initiateDragWindow = true;
        _cooldownDrag = 0.2f;

        transform.SetAsLastSibling();

    }

    public void EndDrag()
    {
        _initiateDragWindow = false;
        _initiateResizing = false;

    }

    public void StartResizing()
    {
        _initiateResizing = true;
        _cooldownResize = 0.2f;

    }

    public void ResizeWindow(Vector2 targetCanvasResize)
    {
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 sizeWindow = rectTransform.sizeDelta;
        Vector2 canvasSize = mobiusOS.rt_Canvas.sizeDelta;
        Vector2 resizeLimit = canvasSize - pos;
        resizeLimit.y = pos.y;

        if (connectedApp.useCustomMinSize == false)
        {
            targetCanvasResize.x = Mathf.Clamp(targetCanvasResize.x, mobiusOS.minimumWindowSize.x, resizeLimit.x);
            targetCanvasResize.y = Mathf.Clamp(targetCanvasResize.y, mobiusOS.minimumWindowSize.y, resizeLimit.y);
        }
        else
        {

            targetCanvasResize.x = Mathf.Clamp(targetCanvasResize.x, connectedApp.minimumWindowSize.x, resizeLimit.x);
            targetCanvasResize.y = Mathf.Clamp(targetCanvasResize.y, connectedApp.minimumWindowSize.y, resizeLimit.y);
        }


        rectTransform.sizeDelta = targetCanvasResize;
        sizeWindow = targetCanvasResize;
    }

    public bool IsWindowMaximized()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 sizeWindow = rectTransform.sizeDelta;
        Vector2 canvasSize = mobiusOS.rt_Canvas.sizeDelta;
        Vector2 resizeLimit = canvasSize - (pos + sizeWindow);

        if (resizeLimit.x < 3f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual void Update()
    {
        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 sizeWindow = rectTransform.sizeDelta;
        Vector2 canvasSize = mobiusOS.rt_Canvas.sizeDelta;
        Vector2 netSizeLimit = canvasSize - sizeWindow;

        //Resizing window position
        {

            if (_cooldownResize > 0f)
            {
                _cooldownResize -= Time.deltaTime;
            }

            if (_cooldownResize <= 0f && Input.GetMouseButton(0) == false)
            {
                _initiateResizing = false;
            }

            if (_initiateResizing)
            {
                var mousePos = Hypatios.UI.GetMousePositionScaled();
                Vector2 mousePosOnCanvas = new Vector2(mousePos.x, mousePos.y);
                Vector2 targetCanvasResize = mousePosOnCanvas - pos;
                targetCanvasResize.y = -targetCanvasResize.y;
                targetCanvasResize.x = Mathf.Clamp(targetCanvasResize.x, mobiusOS.minimumWindowSize.x, canvasSize.x);
                targetCanvasResize.y = Mathf.Clamp(targetCanvasResize.y, mobiusOS.minimumWindowSize.y, canvasSize.y);
 

                rectTransform.sizeDelta = targetCanvasResize;
                sizeWindow = targetCanvasResize;
                netSizeLimit = canvasSize - sizeWindow;
            }

            //auto resize
            {
                Vector2 resizeLimit = canvasSize - pos;
                Vector2 targetCanvasResize = sizeWindow;
                resizeLimit.y = pos.y;

                if (connectedApp.useCustomMinSize == false)
                {
                    targetCanvasResize.x = Mathf.Clamp(targetCanvasResize.x, mobiusOS.minimumWindowSize.x, resizeLimit.x);
                    targetCanvasResize.y = Mathf.Clamp(targetCanvasResize.y, mobiusOS.minimumWindowSize.y, resizeLimit.y);
                }
                else
                {

                    targetCanvasResize.x = Mathf.Clamp(targetCanvasResize.x, connectedApp.minimumWindowSize.x, resizeLimit.x);
                    targetCanvasResize.y = Mathf.Clamp(targetCanvasResize.y, connectedApp.minimumWindowSize.y, resizeLimit.y);
                }


                rectTransform.sizeDelta = targetCanvasResize;
                sizeWindow = targetCanvasResize;
                netSizeLimit = canvasSize - sizeWindow;
            }
        }

        //Dragging & adjusting window position
        {
            if (_cooldownDrag > 0f)
            {
                _cooldownDrag -= Time.deltaTime;
            }

            if (_cooldownDrag <= 0f && Input.GetMouseButton(0) == false)
            {
                _initiateDragWindow = false;
            }

            if (_initiateDragWindow)
            {
                var mousePos = Hypatios.UI.GetMousePositionScaled();
                Vector2 mousePosOnCanvas = new Vector2(mousePos.x, mousePos.y);
                Vector2 posTarget = mousePosOnCanvas + _deltaDragPos;
                pos = posTarget;
            }


            //clamp pos
            if (pos.y < sizeWindow.y)
            {
                pos.y = sizeWindow.y;
            }

            if (pos.y > (canvasSize.y - mobiusOS.border.y))
            {
                pos.y = (canvasSize.y - mobiusOS.border.y);
            }

            if (pos.x < mobiusOS.border.x)
            {
                pos.x = mobiusOS.border.x;
            }

            if (pos.x > (netSizeLimit.x))
            {
                pos.x = netSizeLimit.x;
            }


            rectTransform.anchoredPosition = pos;
        }
    }


}

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
        InstallApps();
        ResetAppWindows();
        InitiateDesktopIcons();
        if (hasStarted == false) return;
        RefreshUI();
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

    #region Refresh UI

    public void RefreshUI()
    {


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

    #endregion



}
