using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityStandardAssets.Utility;

public class Interact_Casino_Roulette : MonoBehaviour
{
    
    public enum BetType
    {
        SingleNumber,
        First12,
        Second12,
        Third12,
        _1to18,
        _19to36,
        Even,
        Odd,
        Red,
        Black,
        Column1,
        Column2,
        Column3,
        SetOf3,
        SetOf6,
    }
    //single number, set of 3/6 all generated on the fly except 0

    public enum ButtonSize
    {
        Single,
        _1to18,
        First12,
        Street
    }

    [System.Serializable]
    public class Bet
    {
        [ShowIf(nameof(IsUsingSetNumber))] public int[] setOfNumbers;
        public BetType betType = BetType.Second12;
        public ButtonSize buttonSize;
        public Transform spawn;

        public bool IsUsingSetNumber => betType == BetType.SingleNumber | betType == BetType.SetOf3 | betType == BetType.SetOf6;

        public string GetName()
        {
            if (betType == BetType.SingleNumber)
            {
                return setOfNumbers[0].ToString();
            }
            else if (betType == BetType.SetOf3 | betType == BetType.SetOf6)
            {
                string s = "";

                if (setOfNumbers.Length <= 3)
                {
                    for (int x = 0; x < setOfNumbers.Length; x++)
                    {
                        if (x > 0)
                            s += ", ";

                        s += setOfNumbers[x];
                    }
                }
                else
                {
                    int i1 = setOfNumbers[setOfNumbers.Length - 1];
                    s = $"{setOfNumbers[0]} - {i1}";
                }

                return s;
            }
            else if (betType == BetType.First12)
                return "1st 12";
            else if (betType == BetType.Second12)
                return "2nd 12";
            else if (betType == BetType.Third12)
                return "3rd 12";
            else if (betType == BetType._1to18)
                return "1 to 18";
            else if (betType == BetType._19to36)
                return "19 to 36";
            else if (betType == BetType.Column1)
                return "Column 1";
            else if (betType == BetType.Column2)
                return "Column 2";
            else if (betType == BetType.Column3)
                return "Column 3";

            return betType.ToString();

        }
    
        public int RewardMultiplier()
        {
            int multiplier = 1;

            if (betType == BetType.SingleNumber)
            {
                multiplier = 36;
            }
            else if (betType == BetType.SetOf3)
                multiplier = 12;
            else if (betType == BetType.SetOf6)
                multiplier = 6;
            else if (betType == BetType.Column1)
                multiplier = 2;
            else if (betType == BetType.Column2)
                multiplier = 2;
            else if (betType == BetType.Column3)
                multiplier = 2;
            else if (betType == BetType.First12)
                multiplier = 2;
            else if (betType == BetType.Second12)
                multiplier = 2;
            else if (betType == BetType.Third12)
                multiplier = 2;
            else if (betType == BetType.Odd)
                multiplier = 2;
            else if (betType == BetType.Even)
                multiplier = 2;
            else if (betType == BetType.Red)
                multiplier = 2;
            else if (betType == BetType.Black)
                multiplier = 2;
            else if (betType == BetType._1to18)
                multiplier = 2;
            else if (betType == BetType._19to36)
                multiplier = 2;


            return multiplier;

        }

        public bool IsConditionMet(int number)
        {
            List<int> listNumbers = setOfNumbers.ToList();

            if (IsUsingSetNumber)
            {
                if (listNumbers.Contains(number))
                {
                    return true;
                }
            }
            else if (betType == BetType.Odd)
            {
                if (number % 2 == 1)
                    return true;
            }
            else if (betType == BetType.Even)
            {
                if (number % 2 == 0)
                    return true;
            }
            else if (betType == BetType._1to18)
            {
                if (number >= 1 && number <= 18)
                    return true;
            }
            else if (betType == BetType._19to36)
            {
                if (number >= 19 && number <= 36)
                    return true;
            }
            else if (betType == BetType.First12)
            {
                if (number >= 1 && number <= 12)
                    return true;
            }
            else if (betType == BetType.Second12)
            {
                if (number >= 13 && number <= 24)
                    return true;
            }
            else if (betType == BetType.Third12)
            {
                if (number >= 25 && number <= 36)
                    return true;
            }
            else if (betType == BetType.Column1)
            {
                int x1 = number - 1;
                if (number == 0) return true;

                if (x1 % 3 == 0)
                    return true;           
            }
            else if (betType == BetType.Column2)
            {
                int x1 = number - 2;
                if (number == 0) return true;

                if (x1 % 3 == 0)
                    return true;
            }
            else if (betType == BetType.Column3)
            {
                int x1 = number;
                if (number == 0) return true;

                if (x1 % 3 == 0)
                    return true;
            }

            return false;
        }
    }

    public List<Bet> specialBetLists = new List<Bet>();
    public UnityEvent OnStartMinigame;

    [FoldoutGroup("References")] public BaseStatValue stat_SoulSpentGambling;
    [FoldoutGroup("References")] public Interact_CasinoRoulette_Touchable touchable_Single;
    [FoldoutGroup("References")] public Interact_CasinoRoulette_Touchable touchable_1to18;
    [FoldoutGroup("References")] public Interact_CasinoRoulette_Touchable touchable_First12;
    [FoldoutGroup("References")] public Interact_CasinoRoulette_Touchable touchable_Street;
    [FoldoutGroup("References")] public Transform t_startingPointSingle;
    [FoldoutGroup("References")] public Transform t_startPoint_3Set;
    [FoldoutGroup("References")] public Transform t_startPoint_6Set;
    [FoldoutGroup("References")] public Transform t_endPointZ;
    [FoldoutGroup("References")] public Transform t_endPointX;
    [FoldoutGroup("References")] public Transform parent_SpawnTouchables;
    [FoldoutGroup("References")] public Transform parent_SpawnChip;
    [FoldoutGroup("References")] public Interact_CasinoRoulette_WagerToken prefabChip;
    [FoldoutGroup("Reference Wheel")] public AutoMoveAndRotate wheel_RotateScript;
    [FoldoutGroup("Reference Wheel")] public AutoMoveAndRotate wheelBall_RotateScript;
    [FoldoutGroup("Reference Wheel")] public dRouletteMachine_TextGenerate wheel_GenerateText;
    [FoldoutGroup("Stat")] public float offset_YChip = 0.05f;
    [FoldoutGroup("Stat")] public float dist_YChip = 0.08f;
    [FoldoutGroup("Stat")] public float BaseSpinningTime = 6f;
    [FoldoutGroup("Stat")] public float SpinningSpeed = 360f;
    [FoldoutGroup("Stat")] public int chipSoul = 10;
    [FoldoutGroup("UIs")] public TextMesh label_totalWager;

    public const int TOTAL_NUMBERS = 36;
    public const int TOTAL_COLUMN = 3;
    public const int TOTAL_ROWS = 12;

    [ShowInInspector] [ReadOnly] private List<Bet> allGeneratedBet = new List<Bet>();
    [ShowInInspector] [ReadOnly] private List<Interact_CasinoRoulette_WagerToken> allWagerChips = new List<Interact_CasinoRoulette_WagerToken>();

    internal bool _isBetLocked = false;
    [ShowInInspector] [ReadOnly] internal int _totalSoul = 0;


    private void Start()
    {
        prefabChip.gameObject.SetActive(false);
        touchable_Single.gameObject.SetActive(false);
        touchable_1to18.gameObject.SetActive(false);
        touchable_First12.gameObject.SetActive(false);
        touchable_Street.gameObject.SetActive(false);
        GenerateSpawn();
        GenerateButton();
    }

    public void GenerateSpawn()
    {
        float totalDistX = Vector3.Distance(t_endPointX.position, t_startingPointSingle.position);
        float totalDistZ = Vector3.Distance(t_endPointZ.position, t_startingPointSingle.position);
        float distX = (totalDistX / (TOTAL_ROWS - 1f));
        float distZ = (totalDistZ / (TOTAL_COLUMN - 1f));

        allGeneratedBet.AddRange(specialBetLists);

        for (int x = 0; x < TOTAL_NUMBERS; x++)
        {
            int num = x+1;
            GameObject spawnT = new GameObject($"Bet_{num}");
            spawnT.transform.SetParent(parent_SpawnTouchables);
            Bet _betSingle = new Bet();
            int[] setNumber = new int[1];
            setNumber[0] = num;
            _betSingle.betType = BetType.SingleNumber;
            _betSingle.setOfNumbers = setNumber;
            _betSingle.spawn = spawnT.transform;

            {
                int _column = (x % TOTAL_COLUMN);
                int _row = Mathf.FloorToInt(x / TOTAL_COLUMN);
                float zPos = distZ * _column;
                float xPos = distX * _row;
                Vector3 pos = t_startingPointSingle.position;
                pos.x += xPos;
                pos.z += zPos;
                spawnT.transform.position = pos;
            }

            allGeneratedBet.Add(_betSingle);
        }

        //Generate Streets
        for (int x = 0; x < TOTAL_ROWS; x++)
        {
            int _row = x;
            int[] setNumber = new int[3];
            setNumber[0] = (_row * 3) + 1;
            setNumber[1] = (_row * 3) + 2;
            setNumber[2] = (_row * 3) + 3;

            GameObject spawnT = new GameObject($"Bet_{setNumber[0]},{setNumber[1]},{setNumber[2]}");
            spawnT.transform.SetParent(parent_SpawnTouchables);
            Bet _betSingle = new Bet();
          
            _betSingle.betType = BetType.SetOf3;
            _betSingle.setOfNumbers = setNumber;
            _betSingle.spawn = spawnT.transform;
            _betSingle.buttonSize = ButtonSize.Street;

            {
                //int _column = (x % TOTAL_COLUMN);
                //float zPos = distZ * _column;
                float xPos = distX * _row;
                Vector3 pos = t_startPoint_3Set.position;
                pos.x += xPos;
                //pos.z += zPos;
                spawnT.transform.position = pos;
            }

            allGeneratedBet.Add(_betSingle);
        }

        int rowHalf = TOTAL_ROWS / 2;

        //Generate Set of 6
        for (int x = 0; x < rowHalf; x++)
        {
            int _row = x;
            int[] setNumber = new int[6];
            setNumber[0] = (_row * 6) + 1;
            setNumber[1] = (_row * 6) + 2;
            setNumber[2] = (_row * 6) + 3;
            setNumber[3] = (_row * 6) + 4;
            setNumber[4] = (_row * 6) + 5;
            setNumber[5] = (_row * 6) + 6;

            GameObject spawnT = new GameObject($"Bet_{setNumber[0]}-{setNumber[5]}");
            spawnT.transform.SetParent(parent_SpawnTouchables);
            Bet _betSingle = new Bet();

            _betSingle.betType = BetType.SetOf6;
            _betSingle.setOfNumbers = setNumber;
            _betSingle.spawn = spawnT.transform;
            _betSingle.buttonSize = ButtonSize.Street;

            {
                //int _column = (x % TOTAL_COLUMN);
                //float zPos = distZ * _column;
                float xPos = distX * 2f * _row;
                Vector3 pos = t_startPoint_6Set.position;
                pos.x += xPos;
                //pos.z += zPos;
                spawnT.transform.position = pos;
            }

            allGeneratedBet.Add(_betSingle);
        }
    }

    public void GenerateButton()
    {
        int ID = 0;

        foreach (var bet in allGeneratedBet)
        {
            Interact_CasinoRoulette_Touchable template = null;

            {
                if (bet.buttonSize == ButtonSize.Single)
                    template = touchable_Single;
                else if (bet.buttonSize == ButtonSize._1to18)
                    template = touchable_1to18;
                else if (bet.buttonSize == ButtonSize.First12)
                    template = touchable_First12;
                else if (bet.buttonSize == ButtonSize.Street)
                    template = touchable_Street;
            }

            var prefab1 = Instantiate(template, parent_SpawnTouchables);
            prefab1.gameObject.SetActive(true);
            prefab1.transform.position = bet.spawn.transform.position;
            prefab1.touchable.interactDescription = $"{bet.GetName()} (+1 Chip)";
            prefab1.ID = ID;
            ID++;
        }
    }

    #region Chip

    public void RemoveChip(Interact_CasinoRoulette_WagerToken wagerChip)
    {
        if (_isBetLocked)
        {
            Prompt_BetLockOn();
            return;
        }

        int wagerID = wagerChip.ID;
        allWagerChips.Remove(wagerChip);
        Destroy(wagerChip.gameObject);
        allWagerChips.RemoveAll(x => x == null);

        var bet = GetBetClass(wagerID);
        var listNeedOrder = allWagerChips.FindAll(x => x.ID == wagerID);

        //reorder
        int index = 0;
        foreach(var chip in listNeedOrder)
        {
            Vector3 pos = bet.spawn.transform.position;
            float y = index * dist_YChip;
            pos.y += y + offset_YChip;
            chip.transform.position = pos;
            index++;
        }

    }

    public void AddChip(Interact_CasinoRoulette_Touchable bet)
    {
        if (_isBetLocked)
        {
            Prompt_BetLockOn();
            return;
        }

        int count = CountBet(bet.ID);
        if (count >= 20)
        {
            DeadDialogue.PromptNotifyMessage_Mod("Maximum is 20 chips per number.", 4f);
            return;
        }

        var chip1 = Instantiate(prefabChip, parent_SpawnChip);
        Vector3 pos = bet.transform.position;
        float y = count * dist_YChip;
        pos.y += y + offset_YChip;
        chip1.transform.position = pos;
        chip1.ID = bet.ID;
        chip1.gameObject.SetActive(true);

        allWagerChips.Add(chip1);
    }

    private void Prompt_BetLockOn()
    {
        DeadDialogue.PromptNotifyMessage_Mod("Bet is locked. You cannot add/remove wager anymore.", 4f);
    }


    public Bet GetBetClass(int ID)
    {
        return allGeneratedBet[ID];
    }

    public int CountBet(int ID)
    {
        int count = 0;
        count = allWagerChips.FindAll(x => x.ID == ID).Count;
        return count;
    }

    [FoldoutGroup("DEBUG")]
    [Button("Roulette Start")]
    public void TriggerRouletteStart()
    {
        LockOnBet();
    }

    public bool LockOnBet()
    {
        allWagerChips.RemoveAll(x => x == null);
        _totalSoul = allWagerChips.Count * chipSoul;

        if (_isBetLocked == true)
        {
            DeadDialogue.PromptNotifyMessage_Mod("You already put the wager. You have to wait the game to finish.", 4f);
            return false;
        }

        if (_totalSoul <= 0)
        {
            DeadDialogue.PromptNotifyMessage_Mod("No wager! Take the casino chips to add wager!", 4f);
            return false;
        }

        if (Hypatios.Game.SoulPoint < _totalSoul)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Not enough souls! {_totalSoul} souls required.", 4f);
            return false;
        }

        Hypatios.Dialogue.QueueDialogue($"Sixtusian Roulette [{_totalSoul} souls]. Spinning the ball...", "SYSTEM", 5f, shouldOverride: true);

        Hypatios.Game.Add_PlayerStat(stat_SoulSpentGambling, _totalSoul);
        _isBetLocked = true;
        OnStartMinigame?.Invoke();
        Hypatios.Game.SoulPoint -= _totalSoul;
        SpinRoulette();
        return true;

    }

    public void ResetGame()
    {
        allWagerChips.RemoveAll(x => x == null);
        foreach (var chip in allWagerChips)
        {
            Destroy(chip.gameObject);
        }
        allWagerChips.Clear();
        _totalSoul = 0;
        _isBetLocked = false;
    }

    #endregion

    #region UIs
    private float _cooldown = 0.2f;
    private float _spinningTime = 10f;
    private float _setSpinTime = 10f;

    private void Update()
    {
        _cooldown -= Time.deltaTime;
        if (_isBetLocked)
        {
            SpinningWheel();
        }

        if (_cooldown > 0f) return;
        RefreshUI();
        _cooldown = 0.2f;
    }

    private void RefreshUI()
    {
        allWagerChips.RemoveAll(x => x == null);
        _totalSoul = allWagerChips.Count * chipSoul;
        label_totalWager.text = $"{_totalSoul} Souls";
    }

    private void SpinningWheel()
    {
        _spinningTime -= Time.deltaTime;
        float percent = _spinningTime / _setSpinTime;
        percent = percent - 0.015f;
        float speed = Mathf.Lerp(0, SpinningSpeed, percent);
        Vector3 v3 = new Vector3(0f, speed, 0f);
        Vector3 v3_1 = new Vector3(0f, -speed, 0f);
        wheelBall_RotateScript.rotateDegreesPerSecond.value = v3;
        wheel_RotateScript.rotateDegreesPerSecond.value = v3_1;

        if (_spinningTime <= 0f)
        {
            RewardGame();
            ResetGame();
        }
    }

    #endregion

    public void SpinRoulette()
    {
        _setSpinTime = BaseSpinningTime + Random.Range(-1f, 1f);
        _spinningTime = _setSpinTime;

    }

    public void RewardGame()
    {
        float rot = wheelBall_RotateScript.transform.localEulerAngles.y;
        int number = wheel_GenerateText.GetCurrentNumber(rot);
        Debug.Log($"Ball entered: {number}");
        int totalSoulReward = 0;

        //evaluate reward
        foreach(var chip in allWagerChips)
        {
            var bet = GetBetClass(chip.ID);

            if (bet.IsConditionMet(number))
            {
                int reward = chipSoul * bet.RewardMultiplier();
                totalSoulReward += reward;
            }
        }

        Hypatios.Game.SoulPoint += totalSoulReward;
        Hypatios.Dialogue.QueueDialogue($"Sixtusian Roulette won: [{totalSoulReward} souls].", "SYSTEM", 5f, shouldOverride: true);

    }

    #region DEBUG

    [FoldoutGroup("DEBUG")]
    [Button("Roulette Test Wheel")]
    public void DEBUG_TestRoulette()
    {
        float rot = wheelBall_RotateScript.transform.localEulerAngles.y;
        int number = wheel_GenerateText.GetCurrentNumber(rot);

        Debug.Log(number);
    }


    #endregion
}
