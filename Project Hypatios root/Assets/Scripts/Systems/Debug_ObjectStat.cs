using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class Debug_ObjectStat : MonoBehaviour
{

    public Transform crosshairGizmos;
    public Transform boundingBox;
    public GameObject walkableGizmo;
    public GUISkin skin;
    public GUISkin skin1;

    [FoldoutGroup("Utility")] public bool LockEnemy = false;

    [FoldoutGroup("Layout")] public float offsetX = 20f;
    [FoldoutGroup("Layout")] public float perLineYSize = 20f;
    [FoldoutGroup("Layout")] public Vector3 EnemyWindowSize = new Vector3(300, 100);
    [FoldoutGroup("Layout")] public Vector3 EnemyWindowOffset = new Vector3(30, 20);

    private bool isCrosshairHitSomething = false;

    [ShowInInspector] [ReadOnly] private EnemyScript currentEnemy;
    [ShowInInspector] [ReadOnly] private InteractableObject currentInteract;


    private void Start()
    {

    }

    private void Update()
    {
        UpdateGizmos();
        if (currentEnemy != null) GizmoEnemy(); else NoGizmoEnemy();
    }

    private void NoGizmoEnemy()
    {
        boundingBox.gameObject.SetActive(false);
    }

    private void GizmoEnemy()
    {
        boundingBox.gameObject.SetActive(true);
        boundingBox.transform.position = currentEnemy.transform.position + Vector3.Scale(currentEnemy.BoundingBox.center, currentEnemy.transform.localScale);
        boundingBox.transform.rotation = currentEnemy.transform.rotation;
        boundingBox.transform.localScale = Vector3.Scale(currentEnemy.BoundingBox.extents, currentEnemy.transform.localScale);
    }

    private void OnGUI()
    {
        if (isCrosshairHitSomething)
        {
            Color c = Color.yellow; c.a = 0.5f;
            GUI.color = c;
            Vector3 v1 = crosshairGizmos.transform.position;
            v1.x = Mathf.Round(v1.x * 10) / 10;
            v1.y = Mathf.Round(v1.y * 10) / 10;
            v1.z = Mathf.Round(v1.z * 10) / 10;

            GUI.Label(new Rect((Screen.width / 2) + offsetX, Screen.height / 2, 500, 20), $"{v1}", skin.label);
            if (currentEnemy != null)
            {
                Color color1 = Color.white; color1.a = 0.8f;
                GUI.color = color1;
                string s1 = $"{currentEnemy.EnemyName} <{Hypatios.Enemy.CheckMyIndex(currentEnemy)}>";
                string s2 = $"";

                {
                    s2 += $"({currentEnemy.Stats.MainAlliance}, {currentEnemy.Stats.UnitType})";
                    s2 += $"\n HP: ({ Mathf.Round(currentEnemy.Stats.CurrentHitpoint)}/{ currentEnemy.Stats.MaxHitpoint.Value})";
                    s2 += $"\n IQ: ({currentEnemy.Stats.Intelligence.Value})";
                }

                GUI.Box(new Rect((Screen.width) - EnemyWindowOffset.x, (Screen.height / 2) - EnemyWindowOffset.y, EnemyWindowSize.x, perLineYSize), s1
                    , skin1.box);

                GUI.Box(new Rect((Screen.width) - EnemyWindowOffset.x, (Screen.height / 2) - EnemyWindowOffset.y + perLineYSize + 1, EnemyWindowSize.x, EnemyWindowSize.y), s2
                 , skin.box);
            }

            if (currentInteract != null)
            {
                Color color1 = Color.white; color1.a = 0.8f;
                GUI.color = color1;
                string s1 = $"{currentInteract.gameObject.name}";
                string s2 = $"";

                {
                    //s2 += $"({currentEnemy.Stats.MainAlliance}, {currentEnemy.Stats.UnitType})";
                    //s2 += $"\n HP: ({ Mathf.Round(currentEnemy.Stats.CurrentHitpoint)}/{ currentEnemy.Stats.MaxHitpoint.Value})";
                    //s2 += $"\n IQ: ({currentEnemy.Stats.Intelligence.Value})";
                }

                GUI.Box(new Rect((Screen.width) - EnemyWindowOffset.x, (Screen.height / 2) - EnemyWindowOffset.y, EnemyWindowSize.x, perLineYSize), s1
                    , skin1.box);

                GUI.Box(new Rect((Screen.width) - EnemyWindowOffset.x, (Screen.height / 2) - EnemyWindowOffset.y + perLineYSize + 1, EnemyWindowSize.x, EnemyWindowSize.y), s2
                 , skin.box);
            }
        }
    }

    private void UpdateGizmos()
    {
        var cam = Hypatios.MainCamera;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            var go = hit.collider.gameObject;
            var enemyGO = go.GetComponentInParent<EnemyScript>();
            var interactObj = go.GetComponentInParent<InteractableObject>();

            crosshairGizmos.transform.position = hit.point;
            crosshairGizmos.transform.rotation = Quaternion.LookRotation(hit.normal);
            isCrosshairHitSomething = true;

            var walkable = CheckPointWalkable(crosshairGizmos.transform.position + new Vector3(0,0.1f,0f));

            if (walkable)
            {
                walkableGizmo.gameObject.SetActive(true);
            }
            else walkableGizmo.gameObject.SetActive(false);

            if (enemyGO != null) { if (!LockEnemy) currentEnemy = enemyGO; } 
            else if (!LockEnemy) currentEnemy = null;

            if (interactObj != null) { currentInteract = interactObj; }
            else currentInteract = null;

        }
        else
        {
            crosshairGizmos.transform.position = new Vector3(-999, -999, -999);
            isCrosshairHitSomething = false;
        }
    }

    bool CheckPointWalkable(Vector3 pos)
    {
        Vector3 randomPoint = pos;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 0.4f, NavMesh.AllAreas))
        {
            return true;
        }

        return false;
    }


    #region Enemy Commands

    public void Enemy_ChangeAllianceToPlayer()
    {
        if (currentEnemy == null)
        {
            ConsoleCommand.Instance.SendConsoleMessage("No enemy detected! Target enemy and then 'wstat lockenemy' to get enemy.");
            return;
        }

        currentEnemy.Hack();
    }

    public void Enemy_WarpToGizmoCrosshair()
    {
        if (currentEnemy == null)
        {
            ConsoleCommand.Instance.SendConsoleMessage("No enemy detected! Target enemy and then 'wstat lockenemy' to get enemy.");
            return;
        }

        currentEnemy.Warp(crosshairGizmos.transform.position);
    }

    public void Enemy_ResetStat()
    {
        if (currentEnemy == null)
        {
            ConsoleCommand.Instance.SendConsoleMessage("No enemy detected! Target enemy and then 'wstat lockenemy' to get enemy.");
            return;
        }

        currentEnemy.Revert();

    }

    #endregion

}
