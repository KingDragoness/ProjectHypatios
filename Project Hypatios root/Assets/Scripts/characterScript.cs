using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class characterScript : MonoBehaviour
{

    dashParticleManager dashManager;
    public Rigidbody rb;
    float playerHeight = 2f;
    wallRun WallRun;

    [Header("Special FPS Mode")]
    public bool isLimitedIntroMode = false;
    public bool normalMode = true;
    public bool disableInput = false;
    public bool tutorialMode = false;

    [Space]
    [Header("Movement States")]
    public float crouchSpeed = 3f;
    public float collider_heightDefault = 0.9f;
    public float collider_heightCrouching = 0.3f;
    public float camera_heightDefault = 0.9f;
    public float camera_heightCrouching = 0.3f;
    public GameObject cameraHolder;

    [Space]
    //Moving
    public float speedMultiplier = 10f;
    public float runSpeed = 12f;
    public float moveSpeed;
    public Vector3 dir;
    float xMovement, yMovement;
    public float groundDrag = 6f;

    //Jumping
    public float gravity = -18.3f;
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask player;
    public float jumpHeight;   
    public float jumpDrag = 1f;
    public float jumpSpeedMultiplier = 0.4f;
    public float fallSpeed = 2f;
    bool inAir = false;
    public bool isGrounded;
    public bool isCrouching = false;

    //Slope & Stair Detection
    RaycastHit slopeHit;
    Vector3 slopeDirection;

    //Dashing
    Vector3 dashDirection;
    public float dashForce = 5f;
    public float dashDuration = .5f;
    public float timeSinceLastDash;
    public float dashCooldown = 3f;

    //Audio
    soundManagerScript soundManager;
    bool runningAudioPlaying = false;

    //Pick Up Items
    public GameObject[] itemOnField;
    public float distanceToPickUp = 2f;
    public GameObject closestItem;
    public WeaponManager weaponSystem;

    //Animation
    [ShowInInspector] [ReadOnly] private Animator anim;
    [ShowInInspector] [ReadOnly] private BaseWeaponScript weaponScript;
    public Animator Anim { get => anim; }

    //Scope
    float scopingSpeed = 6f;

    //Health
    public health heal;
    private CapsuleCollider cc;
    

    public bool isCheatMode;

    void Start()
    {   
        moveSpeed = runSpeed;
        WallRun = GetComponent<wallRun>();
        rb = GetComponent<Rigidbody>();
        dashManager = GetComponent<dashParticleManager>();
        rb.freezeRotation = true;
        timeSinceLastDash = dashCooldown;
        soundManager = FindObjectOfType<soundManagerScript>();
        cc = GetComponent<CapsuleCollider>();

        var currentWeapon = weaponSystem.currentWeaponHeld;

        if (currentWeapon != null)
            anim = currentWeapon.anim;
    }

    private void Update()
    {

        if (Time.timeScale <= 0)
        {
            return;
        }

        if (!heal.isDead)
        {
            if (weaponSystem != null)
            {
                anim = weaponSystem.anim;

                var gun = weaponSystem.currentGunHeld;

                if (gun != null)
                {
                    if (gun.isScoping && !gun.isReloading && gun.canScope)
                        moveSpeed = scopingSpeed;
                    else
                        moveSpeed = runSpeed;
                }
            }

            Friction();
            HandleCrouchingState();

            if (isLimitedIntroMode == false && !disableInput)
                Jumping();

            timeSinceLastDash += Time.deltaTime;
            float timeAfterDash = Mathf.Clamp(timeSinceLastDash / dashCooldown, 0f, dashCooldown);
            MainGameHUDScript.Instance.dashSlider.value = timeAfterDash;

            slopeDirection = Vector3.ProjectOnPlane(dir, slopeHit.normal);


            if (Anim != null)
            {
                if (!isGrounded && !WallRun.isWallRunning)
                {
                    inAir = true;
                    Anim.SetBool("inAir", true);
                }
                else if (WallRun.isWallRunning)
                {
                    inAir = false;
                    Anim.SetBool("inAir", false);

                    FPSMainScript.instance.RuntimeTutorialHelp("Wallrunning", "Simply hold W while steering the player forward to prevent from falling. Player can jump then dash to reach hard-to-reach platform.", "FirstWallRun");
                }
                if (inAir && isGrounded)
                {
                    inAir = false;
                    soundManager.Play("falling");
                    Anim.SetBool("inAir", false);
                }
            }
        }
        else
        {
            if (soundManager != null) soundManager.Pause("falling");
            if (soundManager != null) soundManager.Pause("running");
            moveSpeed = 0f;
        }
    }

    bool testDashReady = false;


    void FixedUpdate()
    {
        Moving();

        if (isLimitedIntroMode)
        {
            return;
        }

        if (timeSinceLastDash > dashCooldown)
        {
            MainGameHUDScript.Instance.dashOk.gameObject.SetActive(true);

            if (testDashReady == false)
            {
                MainGameHUDScript.Instance.PlayDash();
                testDashReady = true;
            }

        }
        else
        {
            MainGameHUDScript.Instance.dashOk.gameObject.SetActive(false);
            testDashReady = false;
        }

        if (isCheatMode == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) && timeSinceLastDash > dashCooldown)
            {
                FPSMainScript.instance.RuntimeTutorialHelp("Dashing", "While holding LEFT SHIFT, you can dash by hold WASD keys to dash either left, right, back or forward.", "FirstDash");
                StartCoroutine(Dash());
                timeSinceLastDash = 0;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                StartCoroutine(Dash());
                timeSinceLastDash = 0;
            }
        }
    }

    public void HandleCrouchingState()
    {
        if (isLimitedIntroMode)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
        }

        if (isCrouching)
        {
            Vector3 v3CamHolder = cameraHolder.transform.localPosition;
            v3CamHolder.y = camera_heightCrouching;
            cameraHolder.transform.localPosition = v3CamHolder;
            cc.height = collider_heightCrouching;

        }
        else
        {
            Vector3 v3CamHolder = cameraHolder.transform.localPosition;
            v3CamHolder.y = camera_heightDefault;
            cameraHolder.transform.localPosition = v3CamHolder;
            cc.height = collider_heightDefault;

        }
    }

    void Friction()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = jumpDrag;
        }
    }

    void Moving()
    {
        if (!disableInput)
        {
            xMovement = Input.GetAxisRaw("Horizontal");
            yMovement = Input.GetAxisRaw("Vertical");
        }

        dir = transform.right * xMovement + transform.forward * yMovement;

        float speed = moveSpeed;

        if (isCrouching)
        {
            speed = crouchSpeed;
        }

        if (isGrounded && !onSlope())
        {
            rb.AddForce(dir.normalized * speed * speedMultiplier);
        }
        else if (!isGrounded)
        {
            rb.AddForce(dir.normalized * speed * speedMultiplier * jumpSpeedMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && onSlope())
        {
            rb.AddForce(slopeDirection.normalized * speed * speedMultiplier, ForceMode.Acceleration);
        }

        if (isLimitedIntroMode == false)
        {
            if (isGrounded && dir.magnitude > 0f)
            {
                if (!runningAudioPlaying)
                {
                    if (isCrouching == false) soundManager.Play("running");
                    runningAudioPlaying = true;

                }

                if (isCrouching == true) soundManager.Pause("running");
            }
            else
            {
                soundManager.Pause("running");
                runningAudioPlaying = false;
            }
        }

        if (weaponSystem != null)
        {
            var gun = weaponSystem.currentGunHeld;

            if (gun != null)
            {
                if (dir.magnitude > 0f || WallRun.isWallRunning)
                {
                    Anim.SetBool("isRunning", true);
                    FPSMainScript.instance.RuntimeTutorialHelp("Moving the Player", "Use your mouse to move your camera. WASD to move the player while SPACE to jump. LEFT CTRL to crouch.", "FirstMove");
                }
                else
                {
                    Anim.SetBool("isRunning", false);
                }

                if (isCrouching)
                {
                    Anim.SetBool("isRunning", false);
                }
            }
        }
    }

    void Jumping()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, player);

        if (!isCheatMode)
        {
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
            }
            else if (!isGrounded && !WallRun.isWallRunning)
            {
                rb.AddForce(-transform.up * fallSpeed, ForceMode.Acceleration);
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                soundManager.Play("jumping");
                Anim.SetTrigger("jumping");
            }
        }
        
    }

    bool onSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight/2 + .5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public IEnumerator Dash()
    {
        if (xMovement == 0 && yMovement == 0)
        {
            dashDirection = transform.forward;
        }
        else if (yMovement > 0)
        {
            dashDirection = transform.forward;
        }
        else if (yMovement < 0)
        {
            dashDirection = -transform.forward;   
        }
        else if (yMovement == 0)
        {
            if (xMovement > 0)
            {
                dashDirection = transform.right;
            }
            else if (xMovement < 0)
            {
                dashDirection = -transform.right;
            }
        }
        rb.useGravity = false;
        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        soundManager.Play("dash");
        dashManager.manageDash();
        yield return new WaitForSeconds(dashDuration);

        rb.velocity = dir * moveSpeed;
        rb.useGravity = true;
    }
}
