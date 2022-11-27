using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class Hypatios : MonoBehaviour
{

    public enum GameDifficulty
    {
        Peaceful = 0, //AI deals absolutely no damage or not shooting
        Casual = 1, //dumbed down enemy AI, optimized for console player
        Normal = 10, //for casual first-person
        Hard = 20, //for those who has experience in FPS (default difficulty)
        Brutal = 30, //for FPS expert, enemy can deal very high damage & chance of crits
    }

    #region Systems
    [SerializeField] private GameDifficulty _gameDifficulty = GameDifficulty.Hard;
    public static GameDifficulty Difficulty { get => Instance._gameDifficulty; }

    #endregion

    [SerializeField]
    private FPSMainScript _fpsMainScript;

    [SerializeField]
    private CharacterScript _characterScript;

    [SerializeField]
    private MainUI _ui;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private DynamicObjectPool _dynamicObjectPool;

    [SerializeField]
    private EnemyContainer _enemyContainer;

    [SerializeField]
    private Debug_ObjectStat _debugObjectStat;

    public static FPSMainScript Game { get => Instance._fpsMainScript; }
    public static CharacterScript Player { get => Instance._characterScript; }
    public static MainUI UI { get => Instance._ui; }
    public static Camera MainCamera { get => Instance._mainCamera; }
    public static DynamicObjectPool ObjectPool { get => Instance._dynamicObjectPool; }
    public static EnemyContainer Enemy { get => Instance._enemyContainer; }
    public static Debug_ObjectStat DebugObjectStat { get => Instance._debugObjectStat; }

    public static Hypatios Instance;

    private void Awake()
    {
        Instance = this;
        //FindObjectOfType
    }

}
