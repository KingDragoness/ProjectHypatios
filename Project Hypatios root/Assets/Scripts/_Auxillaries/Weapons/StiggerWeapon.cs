using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StiggerWeapon : GunScript
{

    public float ChargeTime = 4f;
    public float PitchTarget = 4f;
    public AudioSource audio_ChargeLoop;
    public AudioSource audio_ChargedFire;
    public TextMesh Label_textMesh;
    public GameObject spark_Charged;
    public GameObject spark_NormalBullet;

    private float _timeCharge = 0f;
    private float _originalDamage = 0f;

    public override void OnEnable()
    {
        base.OnEnable();

        _originalDamage = damage;
        bulletSparks = spark_NormalBullet;
    }

    public override void Update()
    {
        if (Time.timeScale <= 0f) return;
        base.Update();

        if (isScoping && !isReloading)
        {
            ChargePower();
        }
        else
        {
            _timeCharge = 0f;
            if (audio_ChargeLoop.isPlaying) audio_ChargeLoop.Stop();
            damage = _originalDamage;
            bulletSparks = spark_NormalBullet;
        }

        float percent = (_timeCharge / ChargeTime) * 100f;
        if (percent > 100)
            percent = 100;
        Label_textMesh.text = $"{Mathf.RoundToInt(percent)}%";

    }

    private void ChargePower()
    {
        _timeCharge += Time.deltaTime;

        if (_timeCharge > ChargeTime)
        {
            damage = _originalDamage * 4f;
            bulletSparks = spark_Charged;
        }
        else
        {
            damage = _originalDamage;
            bulletSparks = spark_NormalBullet;
        }

        if (audio_ChargeLoop.isPlaying == false) audio_ChargeLoop.Play();
        float percent = (_timeCharge / ChargeTime) * 1f;
        if (percent > 1)
            percent = 1;
        audio_ChargeLoop.pitch = percent * PitchTarget;

    }

    public override void FireWeapon()
    {
        base.FireWeapon();

        if (_timeCharge >= ChargeTime) audio_ChargedFire?.Play();
        _timeCharge /= 2f;

    }

    public override void OnReloadCompleted()
    {
        base.OnReloadCompleted();

        _timeCharge = 0f;
        damage = _originalDamage;
    }

}
