using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Chamber_Level6 : MonoBehaviour
{

    public enum PiringMode
    {
        NotPlay,
        PiringNotTaken,
        PiringTaken
    }

    public enum Gamemode
    {
        NotPlay,
        Ongoing,
        Finished
    }

    public enum Ingredient
    {
        Rice = 0,
        Salmon = 1,
        Tuna = 2,
        Melon = 3,
        Pickle = 4,
        Egg = 5,
        Seaweed = 6,
        Lettuce = 7,
        Beef = 8,
        Crab = 9
    }

    [System.Serializable]
    public class Order
    {
        public string name = "Sushi Salmon";
        public List<Ingredient> allRecipes = new List<Ingredient>();

        public bool HasIngredient(Ingredient ingredient)
        {
            return allRecipes.Contains(ingredient);
        }
    }

    [FoldoutGroup("Setup")] public Chamber6_Ingredient prefab_Ingredient;
    [FoldoutGroup("Setup")] public Chamber6_Customer prefab_Customer;
    [FoldoutGroup("Setup")] public AntiZartEnemy prefab_AntiZart;
    [FoldoutGroup("Setup")] public Transform parentIngredient;
    [FoldoutGroup("Setup")] public Transform spawnCustomer;
    [FoldoutGroup("Setup")] public Transform spawnAntiZart;
    [FoldoutGroup("Setup")] public Material[] materialSetup;
    [FoldoutGroup("Setup")] public Chamber6_Piring mainPiring;
    [FoldoutGroup("Setup")] public Chamber6_Piring servingPiring;
    [FoldoutGroup("Setup")] public PiringMode currentMode = PiringMode.NotPlay;
    [FoldoutGroup("Setup")] public List<Transform> seats = new List<Transform>();
    [FoldoutGroup("Setup")] public ChamberText chamberText;
    [FoldoutGroup("Audio")] public AudioSource chamberAudioAnnouncement;
    [FoldoutGroup("Audio")] public AudioSource Audio_foodDelivered;
    [FoldoutGroup("Audio")] public AudioSource Audio_newCustomer;

    public int remainingCustomers = 24;    
    public Gamemode currentGamemode = Gamemode.NotPlay;
    public UnityEvent OnCustomerLeaving;
    public UnityEvent OnChamberCompleted;
    public List<Order> order = new List<Order>();

    [ReadOnly] public List<Chamber6_Piring> piringList = new List<Chamber6_Piring>();
    [ReadOnly] public List<Chamber6_Customer> allCustomers = new List<Chamber6_Customer>();
    [ReadOnly] private AntiZartEnemy currentAntiZart;

    private static Chamber_Level6 instance;

    public static Chamber_Level6 Instance { set => instance = value; }

    private void Awake()
    {
        Instance = this;
    }

    #region Initiation

    void Start()
    {
        mainPiring.gameObject.SetActive(false);
        piringList.Add(mainPiring);
        chamberText.textMesh.text = "0";
        Initialization();
    }

    private void Initialization()
    {
        prefab_Ingredient.gameObject.SetActive(false);

        int i = 0;
        foreach (Ingredient recipe in System.Enum.GetValues(typeof(Ingredient)))
        {
            Vector3 v = prefab_Ingredient.transform.localPosition;
            v.z += i * 1;
           
            Chamber6_Ingredient newPrefab = Instantiate(prefab_Ingredient, parentIngredient);
            newPrefab.gameObject.SetActive(true);
            newPrefab.transform.localPosition = v;
            newPrefab.ingredient = recipe;
            newPrefab.textMesh.text = recipe.ToString();
            newPrefab.gameObject.name = $"Ingredient_{recipe.ToString()}";

            if (i < materialSetup.Length)
            {
                var mat = materialSetup[i];
                newPrefab.meshrenderer.material = mat;
            }

            i++;
        }
    }

    public void StartTheGame()
    {
        if (currentGamemode == Gamemode.NotPlay)
        {
            DialogueSubtitleUI.instance.QueueDialogue("Ding! Sushi Zart has opened for business!", "SYSTEM", 10f);
        }
        if (currentGamemode != Gamemode.NotPlay)
        {
            return;
        }
        currentGamemode = Gamemode.Ongoing;
    }

    #endregion

    private float _cooldownSpawnBots = 3f;

    void Update()
    {
        if (currentGamemode == Gamemode.Ongoing)
        {
            OngoingMode();
        }
    }

    # region Update

    public void OngoingMode()
    {
        chamberText.textMesh.text = remainingCustomers.ToString();

        if (remainingCustomers >= 0)
        {
            _cooldownSpawnBots -= Time.deltaTime;

            if (_cooldownSpawnBots < 0)
            {
                float chance = Random.Range(0f, 1f);
                bool spawn = false;

                if (remainingCustomers > 21)
                {
                    if (allCustomers.Count <= 0)
                    {
                        spawn = true;
                    }
                    else if (chance > 0.4f && allCustomers.Count <= 3)
                    {
                        spawn = true;
                    }
                }
                else if (remainingCustomers > 11)
                {
                    if (allCustomers.Count <= 0)
                    {
                        spawn = true;
                    }
                    else if (chance > 0.6f && allCustomers.Count <= 3)
                    {
                        spawn = true;
                    }
                    else if (chance > 0.3f && allCustomers.Count <= 6)
                    {
                        spawn = true;
                    }
                }
                else if (remainingCustomers > 0)
                {
                    if (allCustomers.Count <= 0)
                    {
                        spawn = true;
                    }
                    else if (chance > 0.6f && allCustomers.Count <= 4)
                    {
                        spawn = true;
                    }
                    else if (chance > 0.4f && allCustomers.Count <= 9)
                    {
                        spawn = true;
                    }
                }

                if (spawn)
                {
                    SpawnCustomer();
                }

                if (currentAntiZart == null && remainingCustomers < 18)
                {
                    AttemptSpawnAntiZart();
                }

                _cooldownSpawnBots = 2f + Random.Range(0f, 2f);
            }
        }

        if (currentGamemode == Gamemode.Ongoing)
        {
            if (allCustomers.Count <= 0 && remainingCustomers <= 0)
            {
                EndTheGame();
            }
        }
    }

    private void AttemptSpawnAntiZart()
    {
        float chance = Random.Range(0, 1f);
        float spawnLimitChance = 0.05f;

        if (remainingCustomers < 21)
        {
            spawnLimitChance += 0.02f;
        }
        if (remainingCustomers < 11)
        {
            spawnLimitChance += 0.05f;
        }

        if (chance < spawnLimitChance)
        {
            SpawnAntiZart();
        }
    }

    #endregion

    #region Actions

    public void SpawnAntiZart()
    {
        Vector3 spawnPos = spawnAntiZart.transform.position;
        spawnPos.x += Random.Range(-10, 10f); spawnPos.z += Random.Range(-10, 10f);
        var antiZart = Instantiate(prefab_AntiZart);
        antiZart.gameObject.SetActive(true);
        antiZart.transform.position = spawnPos;
        currentAntiZart = antiZart;
    }

    public void AmbilPiring()
    {
        currentMode = PiringMode.PiringTaken;
        mainPiring.gameObject.SetActive(true);
        mainPiring.audio_piring.Play();
    }

    public void PutOffPiring()
    {
        currentMode = PiringMode.PiringNotTaken;
        mainPiring.gameObject.SetActive(false);
        mainPiring.ingredients.Clear();
        mainPiring.Refresh();
    }

    public void ThrowIngredients()
    {
        mainPiring.ClearIngredients();
    }

    public void DeliverFood()
    {
        if (currentMode != PiringMode.PiringTaken)
        {
            DialogueSubtitleUI.instance.QueueDialogue("You need to have a plate to deliver the sushi.", "SYSTEM", 3);
            return;
        }

        var newPiring = Instantiate(servingPiring);
        string name = "";

        foreach(var ingredient in mainPiring.ingredients)
        {
            name += ingredient.ToString() + ",";
        }

        newPiring.ingredients.AddRange(mainPiring.ingredients);
        newPiring.Refresh();
        newPiring.gameObject.SetActive(true);
        newPiring.gameObject.name = $"Piring_{name}";
        piringList.Add(newPiring);
        Audio_foodDelivered.Play();
        PutOffPiring();

    }

    #endregion

    public void EndTheGame()
    {
        currentGamemode = Gamemode.Finished;

        if (chamberAudioAnnouncement != null)
        {
            chamberAudioAnnouncement.Play();
            OnChamberCompleted?.Invoke();
            DialogueSubtitleUI.instance.QueueDialogue("Attention to all facility users: Chamber completed.", "ANNOUNCER", 14f);
        }
    }

    [FoldoutGroup("Debug")] [Button("Spawn Customer")]
    public void SpawnCustomer()
    {
        var seatTarget = FindAvailableSeat();
        if (seatTarget == null) return;

        var newRobot = Instantiate(prefab_Customer);
        var order = CreateOrder();
        newRobot.gameObject.SetActive(true);
        newRobot.order = order;
        newRobot.agent.Warp(spawnCustomer.transform.position + new Vector3(Random.Range(-1,1),0, Random.Range(-1, 1)));
        newRobot.targetSeat = seatTarget;
        newRobot.transform.rotation = spawnCustomer.transform.rotation;
        newRobot.tableSeat = seats.IndexOf(seatTarget);
        newRobot.agent.speed = newRobot.agent.speed + Random.Range(-0.4f, 0.3f);
        newRobot.UpdateAI();
        allCustomers.Add(newRobot);
        Audio_newCustomer.Play();
        remainingCustomers--;
    }

    public void RefreshList()
    {
        allCustomers.RemoveAll(x => x == null);
        piringList.RemoveAll(x => x == null);

    }

    #region Utility
    public GameObject GetCustomer()
    {
        int index = Random.Range(0, allCustomers.Count);

        if (allCustomers.Count == 0)
        {
            return null;
        }

        return allCustomers[index].gameObject;
    }

    private Transform FindAvailableSeat()
    {
        foreach(var seat in seats)
        {
            if (IsSeatOccupied(seat) == false)
            {
                return seat;
            }
        }

        return null;
    }

    private bool IsSeatOccupied(Transform seat)
    {
        foreach(var customer in allCustomers)
        {
            if (customer.targetSeat == seat)
            {
                return true;
            }
        }

        return false;
    }

    private Order CreateOrder()
    {
        Order order = new Order();
        int amount = Random.Range(2, 5);

        order.allRecipes.Add(Ingredient.Rice);

        for(int x = 1; x < amount; x++)
        {
            bool isConflict = true;
            Ingredient recipe = Ingredient.Rice;
            int loop = 0;

            while (isConflict)
            {
                recipe = (Ingredient)Random.Range(1,9);
                if (!order.HasIngredient(recipe))
                    isConflict = false;
                if (order.HasIngredient(Ingredient.Tuna) && recipe == Ingredient.Salmon)
                    isConflict = true;
                if (order.HasIngredient(Ingredient.Salmon) && recipe == Ingredient.Tuna)
                    isConflict = true;

                loop++;

                if (loop > 499) break;
            }

            order.allRecipes.Add(recipe);
        }

        return order;
    }

    #endregion

}
