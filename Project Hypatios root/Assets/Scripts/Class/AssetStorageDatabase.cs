using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class AssetStorageDatabase : MonoBehaviour
{

    public List<Trivia> AllTrivias = new List<Trivia>();
    public List<BaseStatusEffectObject> AllStatusEffects;
    public List<WeaponItem> Weapons = new List<WeaponItem>();
    public List<BasePerk> AllBasePerks;

    [Button("Refresh Database")]
    public void RefreshDatabase()
    {
        var _trivias = Resources.FindObjectsOfTypeAll<Trivia>().ToList();
        var _statusEffects = Resources.FindObjectsOfTypeAll<BaseStatusEffectObject>().ToList();
        var _weapons = Resources.FindObjectsOfTypeAll<WeaponItem>().ToList();
        var _basePerks = Resources.FindObjectsOfTypeAll<BasePerk>().ToList();

        _trivias.RemoveAll(x => x.disableTrivia == true);
        AllTrivias = _trivias;
        AllStatusEffects = _statusEffects;
        Weapons = _weapons;
        AllBasePerks = _basePerks;
    }

    public BaseStatusEffectObject GetStatusEffect(StatusEffectCategory category)
    {
        return AllStatusEffects.Find(x => x.category == category);
    }

}
