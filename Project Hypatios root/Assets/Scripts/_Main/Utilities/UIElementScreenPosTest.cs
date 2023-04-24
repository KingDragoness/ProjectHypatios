using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace TestingPurposes
{

    public class UIElementScreenPosTest : MonoBehaviour
    {
        public Vector3 canvasPos;
        public RectTransform rectTransform;
        public RectTransform copyTarget;
        public float offsetMultiplier = 1;

        [Button("Refresh Rect")]
        public void RefreshRect()
        {
            canvasPos = RectTransformToScreenSpace(rectTransform);
        }

        [Button("Copy Rect Pos")]
        public void CopyPosition()
        {
            Vector3 sizeDelta = copyTarget.sizeDelta;
            rectTransform.position = copyTarget.position;
            sizeDelta.x /= 2f; sizeDelta.x = Mathf.Abs(sizeDelta.x);
            sizeDelta.y /= 2f;
            Vector3 v3 = sizeDelta * offsetMultiplier;
            v3.y = -v3.y;
            rectTransform.localPosition += v3;
        }

        public Vector3 GetPositionScreenForTooltip(RectTransform transform)
        {
            var canvas = transform.GetComponentInParent<Canvas>();
            copyTarget = transform;
            CopyPosition();
            return canvas.worldCamera.WorldToScreenPoint(rectTransform.position);
        }


        public static Vector3 RectTransformToScreenSpace(RectTransform transform)
        {
            Vector3 v = new Vector3();
            var canvas = transform.GetComponentInParent<Canvas>();
            var rt = canvas.GetComponent<RectTransform>();
            v = canvas.worldCamera.WorldToScreenPoint(rt.TransformPoint(transform.position));
            v -= canvas.transform.position;
            return v;
        }
    }
}