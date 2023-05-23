using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class GrapplingHookWeapon : GunScript
{

    public float forceMult = 10f;
    public GameObject anchorPrefab;
    public Wire wire;
    [FoldoutGroup("Audios")] public AudioSource audio_Loop;
    [FoldoutGroup("Audios")] public AudioSource audio_CancelHook;

    [ReadOnly] public DynamicObjectPivot currentAnchor;
    private static GameObject containerAnchor;
    private bool isRolling = false;

    public override void Start()
    {
        base.Start();

        if (containerAnchor == null)
        {
            containerAnchor = new GameObject();
            containerAnchor.gameObject.name = "Anchors";
        }
    }

    private void OnDisable()
    {
        CancelHook();
        wire.gameObject.SetActive(false);
    }

    public override void Update()
    {
        base.Update();
        AnchorCheck();
        if (currentAnchor == null)
        {
            wire.gameObject.SetActive(false);
        }
        else
        {
            wire.gameObject.SetActive(true);
        }

        if (isRolling)
        {
            if (audio_Loop.isPlaying == false) audio_Loop.Play();
        }
        else
        {
            if (audio_Loop.isPlaying) audio_Loop.Stop();
        }
    }

    private void AnchorCheck()
    {
        if (currentAnchor == null) return;
        if (currentAnchor.target == null)
        {
            Destroy(currentAnchor.transform.parent.gameObject);
            currentAnchor = null;
            CancelHook();
        }

    }

    public override void FireInput()
    {
        isRolling = false;

        if (Hypatios.Input.Fire2.IsPressed() && !isReloading && currentAnchor != null)
        {
            isRolling = true;
            Grappling();
        }

        if (Hypatios.Input.Fire1.WasReleasedThisFrame() && curAmmo > 0 && !isReloading)
        {
            if (currentAnchor == null)
            {
                if (Time.time >= nextAttackTime)
                {
                    anim.SetTrigger("shooting");
                    FireWeapon();
                    nextAttackTime = Time.time + 1f / bulletPerSecond;
                }
            }
            else
            {
                CancelHook();
            }
        }

      
    }

    public void GrapplingReloadStart()
    {
        CancelHook();
    }

    public void CancelHook()
    {
        if (currentAnchor != null) audio_CancelHook.Play();
        currentAnchor = null;
        
    }

    private void Grappling()
    {
        Vector3 dir = currentAnchor.transform.position - Hypatios.Player.transform.position;
        dir.Normalize();
        Hypatios.Player.rb.AddForce(dir * forceMult * Time.deltaTime, ForceMode.Force);
    }

    public override void FireWeapon()
    {
        float spreadX = 0; Random.Range(-spread, spread);
        float spreadY = 0; Random.Range(-spread, spread);
        Vector3 raycastDir = new Vector3(cam.transform.forward.x + spreadX, cam.transform.forward.y + spreadY, cam.transform.forward.z);
        RaycastHit hit;
        bool isHit = false;
        if (audioFire != null) audioFire.Play();
        gunRecoil.RecoilFire();
        curAmmo--;

        if (Physics.Raycast(cam.transform.position, raycastDir, out hit, 199f, Hypatios.Player.Weapon.defaultLayerMask, QueryTriggerInteraction.Ignore))
        {
            currentHit = hit;
            isHit = true;
        }
        else
        {
            currentHit.point = (raycastDir * 100f) + cam.transform.position;
        }

        if (isHit)
        {
            CreateAnchor(hit.point, hit.normal, hit.collider.transform);
        }
    }

    private void CreateAnchor(Vector3 pos, Vector3 rot, Transform _t)
    {

        var prefab1 = Instantiate(anchorPrefab, pos, Quaternion.Euler(rot));
        var anchorPivot = prefab1.GetComponent<DynamicObjectPivot>();
        currentAnchor = anchorPivot;
        currentAnchor.transform.SetParent(containerAnchor.transform);
        anchorPivot.target = _t;
        wire.target = currentAnchor.transform;
    }
}
