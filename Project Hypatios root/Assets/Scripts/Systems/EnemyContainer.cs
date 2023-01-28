using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;


public class EnemyContainer : MonoBehaviour
{

    [ReadOnly] [ShowInInspector] private List<EnemyScript> AllEnemies = new List<EnemyScript>();
    public LayerMask baseDetectionLayer;
    public System.Action<EnemyScript> OnEnemyDied;

    public void RegisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Add(enemy);
        AllEnemies.RemoveAll(x => x == null);
    }

    public void DeregisterEnemy(EnemyScript enemy)
    {
        AllEnemies.Remove(enemy);
        AllEnemies.RemoveAll(x => x == null);
    }

    public int CountMyEnemies(Alliance alliance)
    {
        return AllEnemies.FindAll(x => x.Stats.MainAlliance != alliance).Count;
    }

    [FoldoutGroup("Debug")] [ShowInInspector] [ReadOnly] private List<EnemyScript> tempList_NearestEnemy = new List<EnemyScript>();

    [FoldoutGroup("Debug")] [Button("testOrderDistance")]
    public void ListOrderTest(Transform enemyOrigin)
    {
        tempList_NearestEnemy.Clear();
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(enemyOrigin.transform.position, x.transform.position)).ToList();
    }

    /// <summary>
    /// Find any enemies/player to attack.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="myPos">Finder's current world position.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    public Entity FindEnemyEntity(Alliance alliance, Vector3 myPos = new Vector3(), float chanceSelectAlly = 0.3f)
    {
        tempList_NearestEnemy.Clear();
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x => Vector3.Distance(myPos, x.transform.position)).ToList();

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }

    /// <summary>
    /// Checking enemies by screen's distance.
    /// </summary>
    /// <param name="alliance">Finder's alliance.</param>
    /// <param name="camTransform">Camera's transform.</param>
    /// <param name="chanceSelectAlly">Recommended value 0.1-1</param>
    /// <returns></returns>
    public Entity FindEnemyEntityFromScreen(Alliance alliance, Camera cam, float chanceSelectAlly = 0.3f)
    {
        tempList_NearestEnemy.Clear();
        foreach (var enemy1 in AllEnemies) tempList_NearestEnemy.Add(enemy1);
        tempList_NearestEnemy = tempList_NearestEnemy.OrderBy(x =>
        {
            Vector3 screenPos = cam.WorldToScreenPoint(x.transform.position);
            float dist = Vector3.Distance(new Vector3(Screen.width/2f, Screen.height/2f, screenPos.z), screenPos);
            return Mathf.RoundToInt(dist*10)/10;
        })
        .ThenBy(l => 
        {
            return Vector3.Distance(cam.transform.position, l.transform.position);
        })
        .ToList();

        return FindEnemyEntity(alliance, chanceSelectAlly);
    }


    private Entity FindEnemyEntity(Alliance alliance, float chanceSelectAlly = 0.3f)
    {

        if (alliance != Alliance.Player)
        {
            float chance = Random.Range(-0f, 1f);

            if (chance < chanceSelectAlly && CountMyEnemies(alliance) > 0)
            {
                var enemy = tempList_NearestEnemy.Find(x => x.Stats.MainAlliance != alliance && x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);

                return enemy;
            }
            else
            {
                return Hypatios.Player;
            }
        }

        else if (alliance == Alliance.Rogue)
        {
            var enemy = tempList_NearestEnemy.Find(x => x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);
            return enemy;
        }
        else
        {
            var enemy = tempList_NearestEnemy.Find(x => x.Stats.MainAlliance != alliance && x.gameObject.activeInHierarchy && x.Stats.UnitType != UnitType.Projectile);
            return enemy;

        }
    }

    public int CheckMyIndex(EnemyScript enemyScript)
    {
        return AllEnemies.IndexOf(enemyScript);
    }

    /// <summary>
    /// Check if the targeted transform is a part of an enemy.
    /// </summary>
    /// <param name="target">Gameobject that has EnemyScript.cs or leg of an enemy + not the same alliance, will return true.</param>
    /// <param name="myAlliance"></param>
    /// <returns></returns>
    public bool CheckTransformIsAnEnemy(Transform target, Alliance myAlliance)
    {
        var enemy = target.GetComponent<EnemyScript>();
        if (enemy == null) enemy = target.GetComponentInParent<EnemyScript>();

        if (enemy != null)
        {
            if (enemy.Stats.MainAlliance != myAlliance)
                return true;
        }

        return false;
    }


}
