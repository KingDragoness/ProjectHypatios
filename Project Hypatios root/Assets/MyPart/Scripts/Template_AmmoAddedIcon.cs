using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Template_AmmoAddedIcon : MonoBehaviour
{

    public WeaponItem.Category category;
    public Text labelAmmoCount;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TriggerAnim()
    {
        anim.SetTrigger("Show");
    }

    public void SetAmmoText(string s)
    {
        labelAmmoCount.text = s;
    }

}
