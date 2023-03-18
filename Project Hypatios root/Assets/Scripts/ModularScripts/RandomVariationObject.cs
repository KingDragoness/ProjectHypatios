using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class RandomVariationObject : MonoBehaviour
{

    //OnFailed event always executed
    //then success execute later

    [System.Serializable]
    public class ObjectUnit
    {
        [FoldoutGroup("Events")] public UnityEvent OnSuccess;
        public GameObject objectActive;

        public void Fail()
        {
            if (objectActive != null) objectActive.gameObject.SetActive(false);
        }

        public void Success()
        {
            if (objectActive != null) objectActive.gameObject.SetActive(true);
            OnSuccess?.Invoke();
        }
    }

    public List<ObjectUnit> allObjectUnits = new List<ObjectUnit>();
    public UnityEvent OnFailed;

    private void Start()
    {
        OnFailed?.Invoke();
        foreach (var t in allObjectUnits) t.Fail();
        ObjectUnit objUnit = allObjectUnits[Random.Range(0, allObjectUnits.Count-1)];
        objUnit.Success();
    }


}
