using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{


    [Header("Special FPS Mode")]
    public bool isLimitedIntroMode = false;

    [Space]
    public wallRun wallRunScript;
    public float mouseSensitivity = 200f;
    public Transform playerBody;
    float xRot = 0f;
    public float x, y;
    public float maxPointingDistance = 1.5f;
    public LayerMask weaponMask;
    GameObject raycastedObject;
    public float throwingForce = 1f;

    Camera cam;

    public static cameraScript instance;
    private Vector3 originalPos;

    private void Awake()
    {
        if (isLimitedIntroMode == false)
            instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        transform.localPosition = Vector3.zero;

        if (cam != null)
        {
            cam = GetComponentInChildren<Camera>();
        }

        originalPos = transform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public float DEBUG_MAGNITUDE = 5;
    public float DEBUG_DURATION = 1;

    [ContextMenu("TestShake")]
    public void TestShake()
    {
        StartCoroutine(Shake(DEBUG_DURATION, DEBUG_MAGNITUDE));
    }


    public void ShakeCam (float duration, float magnitude)
    {
        StartCoroutine(Shake(duration, magnitude));

    }

    //Camera Shake
    private IEnumerator Shake (float duration, float magnitude)
    {

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            
            yield return null;
        }
        transform.localPosition = originalPos;
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        float modifiedSensitivity = mouseSensitivity;
        gunScript Gun = weaponManager.Instance.gun;


        if (Gun != null)
        {
            if (Gun.isScoping)
            {
                if (Input.GetMouseButton(1) && Gun.canScope)
                {
                    modifiedSensitivity *= 0.25f;
                }
            }
        }

        x = Input.GetAxis("Mouse X") * 20 * modifiedSensitivity * Time.deltaTime;
        y = Input.GetAxis("Mouse Y") * 20 * modifiedSensitivity * Time.deltaTime;

        xRot -= y;
        xRot = Mathf.Clamp(xRot, -85f, 85f);
        transform.localRotation = Quaternion.Euler(xRot, 0f, wallRunScript.tilt);

        playerBody.Rotate(Vector3.up * x);
    }
}