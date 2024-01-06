using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public abstract class MobiusApp : MonoBehaviour
{

    public MobiusNetUI mobiusOS;
    public MobiusApp_SO connectedApp;
    public RectTransform rectTransform;
    [FoldoutGroup("Min Max Tab")] public Text minmax_AppLabelName;
    [FoldoutGroup("Min Max Tab")] public Image minmax_AppIcon;

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

public class MobiusApp_Generic : MobiusApp
{
    

}
