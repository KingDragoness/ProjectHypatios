using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

                for (int x = 0; x < setOfNumbers.Length; x++)
                {
                    if (x > 0)
                        s += ", ";
                    
                    s += setOfNumbers[x];
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
    }

    public List<Bet> specialBetLists = new List<Bet>();
    public Interact_CasinoRoulette_Touchable touchable_Single;
    public Interact_CasinoRoulette_Touchable touchable_1to18;
    public Interact_CasinoRoulette_Touchable touchable_First12;
    public Interact_CasinoRoulette_Touchable touchable_Street;
    public Transform t_startingPointSingle;
    public Transform t_endPointZ;
    public Transform t_endPointX;
    public Transform parent_SpawnTouchables;

    public const int TOTAL_NUMBERS = 36;
    public const int TOTAL_COLUMN = 3;
    public const int TOTAL_ROWS = 12;

    [ShowInInspector] [ReadOnly] private List<Bet> allGeneratedBet = new List<Bet>();

    private void Start()
    {
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
    
    }

    public void GenerateButton()
    {
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

        }
    }


}
