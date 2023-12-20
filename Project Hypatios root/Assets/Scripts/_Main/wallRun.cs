using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class wallRun : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rb;
    [SerializeField] Transform body;
    public WeaponManager weapon;
    public AudioSource audio_Wallrun;

    public CharacterScript character;

    public float maxWallDistance = 1f;
    public float stickToWallForce = 10f;
    public float minHeight = 1f;
    float wallRunGravity;
    public float wallRunJumpForce;
    public bool isWallRunning = false;
    public float baseWallRunGravity = -1f;

    bool wallLeft = false;
    bool wallRight = false;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    //Camera
    public CinemachineVirtualCamera vCam;
    public Camera FPScam;
    [SerializeField] float aimFov = 45;
    [SerializeField] float aimFPSFov = 50;
    [SerializeField] float fov;
    [SerializeField] float wallRunFov;
    [SerializeField] float wallRunFovTime;
    [SerializeField] float camTilt;
    [SerializeField] float camTiltTime;

    private float originFPSCam_FOV = 60f;



    int wallLayer = 10;

    public float tilt { get; private set; }

    private void Start()
    {
        //originFPSCam_FOV = FPScam.fieldOfView;
        wallRunGravity = baseWallRunGravity;
    }

    public void SetFOV(int _fov)
    {
        originFPSCam_FOV = _fov;
        fov = _fov;
    }

    bool canWallRun()
    {
        return !character.isGrounded;
    }

    void checkWall()
    {
        if(Physics.Raycast(transform.position, -body.right, out leftWallHit, maxWallDistance))
        {
            if (leftWallHit.transform.gameObject.layer == wallLayer)
            {
                wallLeft = true;
            }
            else
            {
                wallLeft = false;
            }
        }
        else
        {
            wallLeft = false;
        }
        if(Physics.Raycast(transform.position, body.right, out rightWallHit, maxWallDistance))
        {
            if (rightWallHit.transform.gameObject.layer == wallLayer)
            {
                wallRight = true;
            }
            else
            {
                wallRight = false;
            }
        }
        else
        {
            wallRight = false;
        }
    }


    private bool isScoping = false;

    void Update()
    {
        if (weapon == null)
        {
            return;
        }    

        anim = weapon.anim;

        checkWall();
        isScoping = false;

        GunScript Gun = Hypatios.Player.Weapon.currentGunHeld;

        if (Gun != null)
        {
            if (Gun.isScoping)
            {
                if (Hypatios.Input.Fire2.IsPressed() && Gun.canScope)
                {
                    isScoping = true;

                }
            }
        }

        if (isScoping)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, aimFov, 10f * Time.deltaTime);
            FPScam.fieldOfView = Mathf.Lerp(FPScam.fieldOfView, aimFPSFov, 10f * Time.deltaTime);
        }

        if (canWallRun())
        {
            if (wallLeft)
            {
                startWallRun();
            }
            else if (wallRight)
            {
                startWallRun();
            }
            else
            {
                stopWallRun();
            }
        }
        else
        {
            stopWallRun();
        }


    }

    void startWallRun()
    {
        if (isWallRunning == false)
        {
            if (audio_Wallrun.isPlaying == false) audio_Wallrun?.Play();
        }
        isWallRunning = true;
        rb.useGravity = false;

        if (isScoping == false) vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        wallRunGravity += Time.deltaTime * 3f;

        Vector3 stickWallDir = new Vector3();


        if (wallLeft)
        {
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
            stickWallDir = transform.up + -leftWallHit.normal;
        }
        else if (wallRight)
        {
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
            stickWallDir = transform.up + -rightWallHit.normal;
        }
        else
        {
            tilt = 0;
        }


        rb.AddForce(stickWallDir * stickToWallForce, ForceMode.Force);



        if (Hypatios.Input.Jump.triggered)
        {
            anim.SetTrigger("jumping");
            float power = Mathf.Clamp(rb.velocity.magnitude * 0.067f, 1f, 4f);

            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;

                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * power * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * power * 100, ForceMode.Force);
            }
        }
    }

    void stopWallRun()
    {
        if (isWallRunning)
        {
            audio_Wallrun?.Stop();
        }
        isWallRunning = false;
        rb.useGravity = true;
        wallRunGravity = baseWallRunGravity;

        if (isScoping == false) vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, fov, wallRunFovTime * Time.deltaTime);
        if (isScoping == false) FPScam.fieldOfView = Mathf.Lerp(FPScam.fieldOfView, FPSCamera_FOV(), wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }

    public float FPSCamera_FOV()
    {
        float fov = originFPSCam_FOV;
        float min_FOV = 60f;
        float delta_fov = originFPSCam_FOV - min_FOV;

        fov = min_FOV + (delta_fov * 3f / 4f);

        return fov;
    }
}
