using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ChameleonMobius : Enemy
{

    public float health = 1103;
    public Transform target;
    public float rotateSpeed = 10;
    public float moveSpeed = 100;
    public float hoverSpeed = 50;
    public UnityEvent OnKilledEvent;
    public GameObject aliveChameleon;
    public GameObject corpseChameleon;
    public AudioSource audio_WalkingBot;
    public AudioSource audio_DieBot;

    [Space]
    [Title("Weapon System")]
    public MissileChameleon missilePrefab;
    public Transform outWeaponMissile;
    public AudioSource audio_FireMissile;
    [Button("Fire missile")]
    public void FireMissile()
    {
        GameObject prefabMissile = Instantiate(missilePrefab.gameObject, outWeaponMissile.position, Quaternion.identity);
        prefabMissile.gameObject.SetActive(true);

    }
    public float missileCooldownCycle = 2f;

    [Space]

    public Animator animator;

    private float _missileCooldownTimer = 2f;
    private Rigidbody rb;
    private bool hasDied = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        target = FindObjectOfType<characterScript>().transform;

    }

    public override void Attacked(float damage, float repulsionForce = 1)
    {
        health -= damage;
        base.Attacked(damage, repulsionForce);
        DamageOutputterUI.instance.DisplayText(damage);

        if (health < 0 && !hasDied)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            Destroy(gameObject, 10f);
            OnKilledEvent?.Invoke();
            audio_DieBot.Play();
            aliveChameleon.gameObject.SetActive(false);
            corpseChameleon.gameObject.SetActive(true);
            Destroy(corpseChameleon, 10f);
            corpseChameleon.transform.SetParent(null);
            hasDied = true;
        }

    }


    private void Update()
    {
        if (hasDied) return;

        var q = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, rotateSpeed * Time.deltaTime);

        ProcessMovement();
        ProcessAnimation();
        ProcessWeapon();
    }

    private void ProcessWeapon()
    {
        _missileCooldownTimer -= Time.deltaTime;

        if (_missileCooldownTimer < 0)
        {
            audio_FireMissile.Play();
            StartCoroutine(FireRepeatMissile(Random.Range(1, 3)));
            _missileCooldownTimer = missileCooldownCycle;
        }
    }

    IEnumerator FireRepeatMissile(int amount = 1)
    {
        for (int c = 0; c < amount; c++)
        {
            FireMissile();
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void ProcessMovement()
    {
        Vector3 relativePos = transform.InverseTransformPoint(target.position);

        if (relativePos.z > 0)
        {
            rb.AddForce(transform.forward * moveSpeed * rb.mass * Time.deltaTime);
        }

        rb.AddForce(Vector3.up * hoverSpeed * rb.mass * 0.1f * Time.deltaTime);

    }

    private void ProcessAnimation()
    {
        float moveSpeedParamAnim = Mathf.Clamp(rb.velocity.magnitude * 0.25f, 0, 1f);
        animator.SetFloat("MoveSpeed", moveSpeedParamAnim);

        float clampVolume = 0.8f;

        if (moveSpeedParamAnim > 0.1f)
        {
            audio_WalkingBot.pitch = Mathf.Clamp(moveSpeedParamAnim * 2f, 0.1f, clampVolume);

        }
        else
        {
            //audio_WalkingBot.volume = 0f;
        }

        audio_WalkingBot.volume = Mathf.Clamp(moveSpeedParamAnim * 4f, 0f, clampVolume);

    }
}
