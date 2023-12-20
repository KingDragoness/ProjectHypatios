using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber_Freemode : MonoBehaviour
{


    private HypatiosSave _cachedPlayerMainSave;
    public HypatiosSave SaveFile
    {
        get
        {
            return _cachedPlayerMainSave;
        }
    }


    void Start()
    {
        _cachedPlayerMainSave = MainMenuTitleScript.GetHypatiosSave();

        RefreshPerkFromSave();
    }

    private void RefreshPerkFromSave()
    {
        HypatiosSave.PerkDataSave perkDataSave = _cachedPlayerMainSave.AllPerkDatas;
        perkDataSave = _cachedPlayerMainSave.AllPerkDatas.Clone();
        perkDataSave.Temp_StatusEffect.Clear();
        perkDataSave.Temp_CustomPerk.Clear();

        Hypatios.Player.PerkData = perkDataSave;
    }

    public void RefreshPlayerPerk()
    {
        Hypatios.Player.ReloadStatEffects();
        Hypatios.Player.Health.targetHealth = Hypatios.Player.Health.maxHealth.Value;
        Hypatios.Player.Health.curHealth = Hypatios.Player.Health.maxHealth.Value;
    }

}
