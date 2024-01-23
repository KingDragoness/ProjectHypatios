using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class NewDialogueButton : MonoBehaviour
{


    public DialogCommandEntry.Type type;
    [ShowIf("type", DialogCommandEntry.Type.Message)] public Text text_Message;
    [ShowIf("type", DialogCommandEntry.Type.Timer)] public Slider slider_timer;
    [ShowIf("type", DialogCommandEntry.Type.ResponseSelection)] public Text text_ResponseSelect;
    public CanvasGroup cg;

    private float timeToClose = 1f;
    private bool isClosing = false;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    private RectTransform _rectTransform;

    private void Update()
    {
        if (isClosing)
        {
            timeToClose -= Time.deltaTime;

            cg.alpha = timeToClose;

            if (timeToClose <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }


    public void WipeDeleteButton()
    {
        isClosing = true;
    }

}
