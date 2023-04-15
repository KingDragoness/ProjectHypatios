using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum CategoryParticleEffect
{
    Uncategorized,
    BulletSparks,
    BulletSparksEnemy,
    BulletImpact,
    BulletDustTracer,
    UseableBulletDustTracer,
    ExplosionHarmless = 20,
    ExplosionPlayer,
    ExplosionAll,
    ExplosionSeaver,
    FireEffect = 30,
    PoisonEffect,
    ParalyzeEffect,
    RockDestructible = 40,
    ExplosiveDust
}

public class DynamicObjectPool : MonoBehaviour
{
   
    [System.Serializable]
    public class PoolContainer
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField, Min(1)] private int _startCount;
        [SerializeField, Range(1,1000)] private int _hardCount;
        [SerializeField] private CategoryParticleEffect _category;

        private List<GameObject> _allPooledObjects = new List<GameObject>();

        public static PoolContainer CreatePoolContainer(GameObject prefab, int startCount, int hardCount)
        {
            var newPool = new PoolContainer();
            newPool._prefab = prefab;
            newPool._startCount = startCount;
            newPool._hardCount = hardCount;
            newPool.Populate();
            return newPool;
        }

        public void ManualPopulate()
        {
            Populate();
        }

        private void Populate()
        {
            for (int x = 0; x < _startCount; x++)
            {
                var instance = Object.Instantiate(_prefab);
                instance.gameObject.SetActive(false);
                _allPooledObjects.Add(instance);
            }
        }

        private void AddNewInstance()
        {
            if (IsHardLimitReached()) return;
            var instance = Object.Instantiate(_prefab);
            instance.gameObject.SetActive(false);
            _allPooledObjects.Add(instance);
        }

        public void ParentAllPooledObjects(Transform parent)
        {
            foreach(var go in _allPooledObjects)
            {
                go.transform.SetParent(parent);
            }
        }

        public bool IsHardLimitReached()
        {
            if (_allPooledObjects.Count > _hardCount)
                return true;

            return false;
        }

        public GameObject Reuse(bool includeActive = false)
        {
            _allPooledObjects.RemoveAll(x => x == null);
            var instance = _allPooledObjects.Find(x => x.activeSelf == false);

            {
                if (instance == null && IsHardLimitReached() == false) AddNewInstance(); instance = _allPooledObjects.Find(x => x.activeSelf == false);
                if (instance == null && includeActive && IsHardLimitReached() == true) { instance = _allPooledObjects[Random.Range(0, _allPooledObjects.Count)]; if (instance != null) instance.gameObject.SetActive(false); } //second pass
            }

            if (instance == null) {     
            }

            if (instance == null) {  }
                //Debug.LogError($"Spawning {Prefab.gameObject.name} Reached Limit! ({_allPooledObjects.Count}/{_hardCount})");
            else
                instance.gameObject.SetActive(true);
            return instance;
        }

        public GameObject Prefab { get => _prefab; }
        public CategoryParticleEffect Category { get => _category;}
    }

    [SerializeField] private List<PoolContainer> _pools = new List<PoolContainer>();

    private void Awake()
    {
        foreach(var pool in _pools)
        {
            pool.ManualPopulate();
            pool.ParentAllPooledObjects(this.transform);
        }
    }

    /// <summary>
    /// Can pool object on demand. IMPORTANT: Override position must be done under this function or else ObjectPool events might not work.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="startingAmount"></param>
    /// <param name="hardLimit"></param>
    /// <param name="IncludeActive"></param>
    /// <param name="spawnPos"></param>
    /// <returns></returns>
    public GameObject SummonObject(GameObject prefab, int startingAmount = 30, int hardLimit = 1000, bool IncludeActive = false, Vector3 spawnPos = new Vector3())
    {
        if (prefab == null)
        {
            Debug.LogError("Cannot spawn an empty object!");
            return null;
        }

        if (IsPoolAlreadyRegistered(prefab) == false)
        {
            var pool = PoolContainer.CreatePoolContainer(prefab, startingAmount, hardLimit);
            pool.ParentAllPooledObjects(this.transform);
            _pools.Add(pool);
        }

        var targetPool = _pools.Find(x => x.Prefab == prefab);
        var instance = targetPool.Reuse(IncludeActive);
        if (instance)
        {
            instance.transform.SetParent(this.transform);
            var objPool = instance.GetComponent<ObjectPool>();
            if (objPool != null)
            {
                if (spawnPos.x != 0) objPool.transform.position = spawnPos;
                objPool.OnReuseObject(); 
            }
        }
        return instance;
    }

    /// <summary>
    /// Summon pre-determined particle.
    /// </summary>
    /// <param name="particle"></param>
    /// <param name="IncludeActive"></param>
    /// <param name="_pos"></param>
    /// <param name="_rot"></param>
    /// <returns></returns>
    public GameObject SummonParticle(CategoryParticleEffect particle, bool IncludeActive = false, Vector3 _pos = new Vector3(), Quaternion _rot = new Quaternion())
    {
        var targetPool = _pools.Find(x => x.Category == particle);
        var instance = targetPool.Reuse(IncludeActive);
        if (instance)
        {
            instance.transform.SetParent(this.transform);
            instance.transform.position = _pos;
            instance.transform.rotation = _rot;
            var objPool = instance.GetComponent<ObjectPool>();
            if (objPool != null) { objPool.OnReuseObject(); }
        }
        return instance;
    }



    private bool IsPoolAlreadyRegistered(GameObject prefab)
    {
        foreach(var poolContainer in _pools)
        {
            if (poolContainer.Prefab == prefab)
            {
                return true;
            }
        }

        return false;
    }

}
