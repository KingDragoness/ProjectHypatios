using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class cameraScript : MonoBehaviour
{


    [Header("Special FPS Mode")]
    public bool isLimitedIntroMode = false;

    [Space]
    public Camera fpsCamera;
    public RenderTexture lowpoly_rt;
    public RenderTexture lowpoly_fps_rt;
    public wallRun wallRunScript;
    public float mouseSensitivity = 200f;
    public float aimAssistSensitivity = 5f;
    public Transform playerBody;
    float xRot = 0f;
    public float x, y;
    public float externalX, externalY;
    public float maxPointingDistance = 1.5f;
    public LayerMask weaponMask;
    public LayerMask cameraMaskAim;
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

    private void BaitTest1()
    {
        Texture texture = null;
    }

    public void LowPolyRT()
    {
        cam.targetTexture = lowpoly_rt;
        fpsCamera.targetTexture = lowpoly_fps_rt;
        fpsCamera.clearFlags = CameraClearFlags.SolidColor;
    }

    public void DisableLowPolyRT()
    {
        cam.targetTexture = null;
        fpsCamera.targetTexture = null;
        fpsCamera.clearFlags = CameraClearFlags.Depth;
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

        if (Input.gyro != null)
        {
            Input.gyro.enabled = true;

            if (SystemInfo.supportsGyroscope)
            {
                Debug.Log("Gyro supported");
            }
        }


        originalPos = transform.localPosition;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
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

    private void Update()
    {
        if (Input.gyro != null)
        {
            //Debug.Log(Input.gyro.enabled);
            //Debug.Log($"{Input.gyro.attitude} + {Input.gyro.rotationRateUnbiased}");
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        float modifiedSensitivity = mouseSensitivity;
        GunScript Gun = Hypatios.Player.Weapon.currentGunHeld;


        if (Gun != null)
        {
            if (Gun.isScoping)
            {
                if (Hypatios.Input.Fire2.IsPressed() && Gun.canScope)
                {
                    modifiedSensitivity *= 0.25f;
                }
            }
        }

        if (Hypatios.Player.disableInput == false)
        {
            var moveVector = Hypatios.Input.Look.ReadValue<Vector2>();
            if (Gamepad.current != null) moveVector += AimAssist();

            x = moveVector.x * 20 * modifiedSensitivity * Time.deltaTime;
            y = moveVector.y * 20 * modifiedSensitivity * Time.deltaTime;
            x += externalX * Time.deltaTime;
            y += externalX * Time.deltaTime;
        }
        else
        {
            x = 0;
            y = 0;
        }


        xRot -= y;
        xRot = Mathf.Clamp(xRot, -85f, 85f);
        transform.localRotation = Quaternion.Euler(xRot, 0f, wallRunScript.tilt);

        playerBody.Rotate(Vector3.up * x);
    }

    private Vector2 AimAssist()
    {
        Vector2 moveVector = new Vector2();

        var entity = Hypatios.Enemy.FindEnemyEntityFromScreen(Alliance.Player, cam);

        EnemyScript newTarget = null;
        if (entity != null)
        {
            newTarget = entity as EnemyScript;
        }

        if (newTarget != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(newTarget.OffsetedBoundWorldPosition);
            Vector3 localPos = cam.transform.InverseTransformPoint(newTarget.transform.position);
            Vector3 crosshair_ScreenPos = new Vector3(Screen.width / 2f, Screen.height / 2f, screenPos.z);
            float dist = Vector3.Distance(new Vector3(Screen.width / 2f, Screen.height / 2f, screenPos.z), screenPos);

            if (localPos.z > 0 && dist < 100)
            {
                Vector3 dir = screenPos - crosshair_ScreenPos;
                dir.Normalize();
                moveVector += new Vector2(dir.x, dir.y) * Time.deltaTime * 0.05f * aimAssistSensitivity;
            }

            var dirReal = newTarget.transform.position - cam.transform.position;
            RaycastHit hit;

            //if blocked
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1000f, cameraMaskAim, QueryTriggerInteraction.Ignore))
            {
                var enemy = hit.collider.GetComponentInParent<EnemyScript>();

                if (enemy == null)
                {
                    float distHitToCam = Vector3.Distance(hit.point, cam.transform.position);
                    float distEnemyToCam = Vector3.Distance(newTarget.transform.position, cam.transform.position);

                    if (distEnemyToCam > distHitToCam)
                        moveVector = Vector3.zero;
                }
            }
        }


        return moveVector;
    }
}