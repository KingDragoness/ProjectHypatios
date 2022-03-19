using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator2 : MonoBehaviour
{

    RectTransform rect;
    CanvasGroup canvas;
    Transform player;
    Camera cam;
    Quaternion targetRot;
    float curTime;
    float timeToDestroy = 3f;
    Transform target;
    bool targetHasBeenSet;

    Vector3 tempv3ForMissingT;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        canvas = GetComponent<CanvasGroup>();
        canvas.alpha = 1f;
    }

    private void Update()
    {
        if (targetHasBeenSet)
        {
            Vector3 targetVector1 = Vector3.zero;

            if (target == null)
            {
                //Destroy(gameObject);

                targetVector1 = tempv3ForMissingT;
            }
            else
            {
                targetVector1 = target.position;
            }

            Vector2 dir = new Vector2(player.position.x - targetVector1.x, player.position.z - targetVector1.z);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float angleAfterConsideringCamera = angle + cam.transform.eulerAngles.y + 180f;
            curTime += Time.deltaTime;
            if (curTime > timeToDestroy)
            {
                canvas.alpha -= Time.deltaTime;
            }
            if (canvas.alpha <= 0f)
            {
                Destroy(gameObject);
            }

            rect.rotation = Quaternion.Euler(0f, 0f, angleAfterConsideringCamera);
        }
        
    }

    public void SetTarget(Transform target2)
    {
        target = target2;
        tempv3ForMissingT = target2.transform.position;
        targetHasBeenSet = true;
    }

}
