﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
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
}
public static class IsopatiosUtility
{
    public static T Clone<T>(this T source)
    {
        var serialized = JsonConvert.SerializeObject(source);
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
        deactivator.timer = time;
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
