using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

public static class StringExtensions
{
    public static string Capitalize(this string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input[0].ToString().ToUpper() + input.Substring(1);
        }
    }


    public static string ToSentence(this string Input)
    {
        return new string(Input.SelectMany((c, i) => i > 0 && char.IsUpper(c) ? new[] { ' ', c } : new[] { c }).ToArray());
    }

    public static int CountLines(this string str)
    {
        int count = 0;

        count = str.Count(c => c.Equals('\n')) + 1;


        return count;
    }

    public static string GetString(this LocalizedString _locString, string defaultText)
    {
        bool isInvalid = false;
    
        if (_locString == null)
        {
            isInvalid = true;
        }
        else if (string.IsNullOrEmpty(_locString.TableReference.TableCollectionName) | _locString.TableEntryReference.KeyId == 0)
        {
            isInvalid = true;
        }

        if (isInvalid == false)
        {
            var sd = LocalizationSettings.StringDatabase;
            var table = sd.GetTable(_locString.TableReference.TableCollectionName);

            if (table == null)
                isInvalid = true;
            else
            {
                var entry = table.GetEntry(_locString.TableEntryReference.KeyId);

                if (entry == null)
                {
                    isInvalid = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(entry.GetLocalizedString()))
                    {
                        isInvalid = true;
                    }
                }
            }
        }

        if (isInvalid)
            return defaultText;
        else
            return _locString.GetLocalizedString();
    }
}
public static class IsopatiosUtility
{

    public static object GetValue(this MemberInfo memberInfo, object forObject)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(forObject);
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(forObject);
            default:
                throw new NotImplementedException();
        }
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    /// <summary>
    /// returns -1 when to the left, 1 to the right, and 0 for forward/backward
    /// </summary>
    /// <param name="fwd"></param>
    /// <param name="targetDir"></param>
    /// <param name="up"></param>
    /// <returns></returns>
    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0.0f)
        {
            return 1.0f;
        }
        else if (dir < 0.0f)
        {
            return -1.0f;
        }
        else
        {
            return 0.0f;
        }
    }

    public static int RandomSign()
    {
        return UnityEngine.Random.value < 0.5f ? 1 : -1;
    }

    public static bool CheckNavMeshWalkable(Vector3 center, float range, out Vector3 result, int tries = 20)
    {
        for (int i = 0; i < tries; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    public static void EnableGameobject(this GameObject go, bool active)
    {
        if (go.activeSelf != active) go.SetActive(active);
    }


    public static bool IsAgentCanReachLocation(this NavMeshAgent agent, Vector3 pos)
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        if (agent == null) return false;

        if (agent.CalculatePath(pos, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            return true;

        }
        else
        {
            return false;
        }

    }

    public static bool GetPath(this NavMeshPath path, Vector3 fromPos, Vector3 toPos, int passableMask)
    {
        path.ClearCorners();

        if (NavMesh.CalculatePath(fromPos, toPos, passableMask, path) == false)
            return false;

        return true;
    }

    public static float GetPathLength(this NavMeshPath path)
    {
        if (path.corners.Length < 2)
            return 9999;

        Vector3 previousCorner = path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        while (i < path.corners.Length)
        {
            Vector3 currentCorner = path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            Debug.DrawLine(previousCorner, currentCorner, Color.red);
            previousCorner = currentCorner;
            i++;
        }

        return lengthSoFar;
    }

    public static int Choose(int[] probs)
    {

        int total = 0;

        foreach (int elem in probs)
        {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                           screenPosition.y / Screen.height,
                                           0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }

    public static T Clone<T>(this T source)
    {
        var serialized = JsonConvert.SerializeObject(source, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        return JsonConvert.DeserializeObject<T>(serialized);
    }

    public static T NextOf<T>(this IList<T> list, T item)
    {
        var indexOf = list.IndexOf(item);
        return list[indexOf == list.Count - 1 ? 0 : indexOf + 1];
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(x => Guid.NewGuid());
    }
    public static bool IsParentOf(this GameObject origin, GameObject parent)
    {
        Transform t = origin.transform;
        if (t == parent.transform) return true;

        while (t.parent != null)
        {
            t = t.parent;

            if (t == parent.transform) return true;
        }


        return false;
    }

    public static IEnumerable<T> GetValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static bool Contains(this LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    public static T GetComponentInParent<T>(this GameObject origin)
    {
        Transform t = origin.transform;

        while (t.parent != null)
        {
            t = t.parent;

            T component = t.gameObject.GetComponent<T>();

            if (component != null) return component;
        }


        return default(T);
    }

    public static Transform[] GetAllParents(this GameObject origin)
    {
        List<Transform> parents = new List<Transform>();
        Transform t = origin.transform;

        while (t.parent != null)
        {
            t = t.parent;

            parents.Add(t);
        }


        return parents.ToArray();
    }


    public static T GetComponentThenParent<T>(this GameObject origin)
    {
        T component = origin.GetComponent<T>();

        if (component != null) return component;
        else component = origin.GetComponentInParent<T>();

        if (component != null) return component;

        return default(T);
    }



    public static T GetComponentThenChild<T>(this GameObject origin)
    {
        T component = origin.GetComponent<T>();

        if (component != null) return component;
        else component = origin.GetComponentInChildren<T>();

        if (component != null) return component;

        return default(T);
    }


    public static List<GameObject> AllChilds(this GameObject root)
    {
        List<GameObject> result = new List<GameObject>();
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(result, VARIABLE.gameObject);
            }
        }
        return result;
    }

    private static void Searcher(List<GameObject> list, GameObject root)
    {
        list.Add(root);
        if (root.transform.childCount > 0)
        {
            foreach (Transform VARIABLE in root.transform)
            {
                Searcher(list, VARIABLE.gameObject);
            }
        }
    }

    public static void DisableObjectTimer(this GameObject gameObject, float time = 5f)
    {
        var deactivator = gameObject.GetComponent<TimedObjectDeactivator>();
        if (deactivator == null) deactivator = gameObject.AddComponent<TimedObjectDeactivator>();
        deactivator.allowRestart = true;
        deactivator.Timer = time;
    }

public static bool IsIndexExist<T>(this List<T> list, int index)
    {

        if (index < 0)
        {
            return false;
        }
        else if (list.Count > index)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsInsideOcclusionBox(this Transform box, Vector3 targetPosition)
    {
        Vector3 localPos = box.InverseTransformPoint(targetPosition);

        if (Mathf.Abs(localPos.x) < (box.localScale.x / 2) && Mathf.Abs(localPos.y) < (box.localScale.y / 2) && Mathf.Abs(localPos.z) < (box.localScale.z / 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }




}
