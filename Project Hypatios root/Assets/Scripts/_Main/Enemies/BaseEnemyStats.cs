using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Spider", menuName = "Hypatios/BaseEnemyStats", order = 1)]
public class BaseEnemyStats : ScriptableObject
{
    public EnemyStats Stats;
    public Sprite enemySprite;
    public LootTable lootDrop;

}
