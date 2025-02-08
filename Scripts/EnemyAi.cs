using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    // Reference to cards and Gamemanager
    Cards cardRef;
    GameManager gameManagerRef;

    // Get BoardSlots
    public List<GameObject> BoardSlots = new List<GameObject>();
    public GameObject[] EnemyBoardSlots;
    public List<GameObject> SummonableBoardSlots;
    public List<GameObject> EnemyBoardCards = new List<GameObject>();

    List<GameObject> MonsterCards;

    private bool MonsterSpawned;
    private bool MonsterAttack;

    // UI 
    public TextMeshProUGUI MonsterDamagePrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Reference
        cardRef = FindObjectOfType<Cards>();
        gameManagerRef = FindObjectOfType<GameManager>();

        // MonsterDamageInfo
        MonsterDamagePrefab = gameManagerRef.MonsterDamagePrefab;

        // Get Board Slots in scene
        GameObject[] AiBoardSlots = GameObject.FindGameObjectsWithTag("Player2BoardSlots");
        foreach (GameObject boardSlot in AiBoardSlots) 
        {
            // Loop through each child of the current boardSlot (AiBoardSlot)
            Transform boardSlotTransform = boardSlot.transform; // Get the Transform component of the boardSlot
            for (int i = 0; i < boardSlotTransform.childCount; i++)
            {
                // Get the child GameObject
                GameObject child = boardSlotTransform.GetChild(i).gameObject;

                // Add the child GameObject to the BoardSlots list
                BoardSlots.Add(child);
            }
        }

        //Find BoardSlots
        EnemyBoardSlots = GameObject.FindGameObjectsWithTag("Player2BoardSlots");

        // Copy Board slots to Summonable board slot
        SummonableBoardSlots = new List<GameObject>(BoardSlots);

        //Reference monsterCards list
        MonsterCards = new List<GameObject>(gameManagerRef.MonsterCards);
    }

    // Update is called once per frame
    void Update()
    {       
        // Opponent Phases check
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentStartPhase || gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentBattlePhase || gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentPostPhase)
        {
            // If monster attack animation
            if (cardRef.attack)
            {
                MonsterAttack = true;
            }

            //Turn Actions
            TurnPhaseActions();
           

        }
        else
        {
            MonsterSpawned = false;
            MonsterAttack = false;
        }

        
    }

    void TurnPhaseActions()
    {

        // Ai Start Phase:
        // If monster Not spawned yet
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentStartPhase && !MonsterSpawned)
        {
            print("Ai thinking...");
            DelayedMonsterSpawn();

            MonsterSpawned = true;
        }

        // Ai Battle Phase
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentBattlePhase && !MonsterAttack)
        {
            Transform Card = null;
            for (int n = 0; n < EnemyBoardSlots.Length; n++)  // Iterate through each EnemyBoardSlot
            {
                Transform boardSlot = EnemyBoardSlots[n].transform;

                // Check if the current BoardSlot has children
                if (boardSlot.childCount > 1)
                {
                    for (int i = 0; i < boardSlot.childCount; i++) // Iterate through children of the BoardSlot
                    {
                        Transform potentialMonsterHolder = boardSlot.GetChild(i);

                        // Check if the current child has children (further hierarchy)
                        if (potentialMonsterHolder.childCount > 0)
                        {
                            for (int c = 0; c < potentialMonsterHolder.childCount; c++) // Iterate through the nested children
                            {
                                Transform childOfChild = potentialMonsterHolder.GetChild(c);
                                print("Checking: " + childOfChild.gameObject.name);

                                // Check if the child has the "EnemyMonsterCard" tag
                                if (childOfChild.CompareTag("EnemyMonsterCard"))
                                {
                                    Debug.Log("Found a monster card with the 'EnemyMonsterCard' tag: " + childOfChild.name);
                                    EnemyBoardCards.Add(childOfChild.gameObject);

                                    // For each monster, initiate attack
                                    foreach (GameObject EnemyMonster in EnemyBoardCards)
                                    {
                                        // Call monster attack func for each monster
                                        MonsterInfo monsterInfo = EnemyMonster.GetComponent<MonsterInfo>();
                                        monsterInfo.MonsterAttack();

                                    }

                                }
                            }
                        }
                    }
                }


                MonsterAttack = true;

                //// Assuming that Player1's BoardSlots are under "Player1BoardSlots" parent
                //GameObject[] allBoardSlots = GameObject.FindGameObjectsWithTag("Player1BoardSlots");

                //foreach (GameObject boardSlot in allBoardSlots)
                //{
                //    // Check if the board slot belongs to Player1
                //    if (boardSlot.transform.IsChildOf(GameObject.Find("Player2BoardSlots").transform))
                //    {
                //        // If it's part of Player2's board, check if it contains a "MonsterCard"
                //        foreach (Transform child in boardSlot.transform)
                //        {
                //            foreach (Transform childOfChild in child.transform)
                //            {
                //                print(child.name);
                //                // Check if the child is a "MonsterCard"
                //                if (childOfChild.CompareTag("EnemyMonsterCard"))
                //                {
                //                    // Death Special Effect
                //                    GameObject DeathFX = Instantiate(gameManagerRef.SpecialEffects[1], childOfChild.gameObject.transform.position, childOfChild.gameObject.transform.rotation);
                //                    // Call the delayed destruction
                //                    DelayedDestroy(childOfChild.gameObject);
                //                    Debug.Log("Destroy Monster");

                //                    //Destroy Effect After 2.5 s
                //                    StartCoroutine(DestroySpecialEffectAfterDelay(2.5f, DeathFX));
                //                }
                //            }
                //        }
                //    }
                //}


            }

            // Ai Post Phase
            if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentPostPhase)
            {

            }
        }
    }



        // Assuming child is a reference to the GameObject to be destroyed
        public void DestroyMonster(GameObject child)
    {
        // Destroy the GameObject immediately
        Destroy(child);
    }

    // Start the coroutine to delay destruction
    public void DelayedDestroy(GameObject child)
    {
        // Start the coroutine to destroy the monster after a delay of x seconds
        StartCoroutine(DestroyMonsterAfterDelay(child, 3f));
    }

    // Coroutine that waits for a specified time before calling DestroyMonster
    private IEnumerator DestroyMonsterAfterDelay(GameObject child, float delay)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        DestroyMonster(child);
    }


    // Spawn Monster Functions
    private IEnumerator SpawnMonsterAfterDelay(float delay)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        SpawnMonster();
    }
    public void DelayedMonsterSpawn()
    {
        // Start the coroutine to destroy the monster after a delay of 3 seconds
        StartCoroutine(SpawnMonsterAfterDelay(2f));
    }

    private void SpawnMonster()
    {
        // Get a random slot out of the board slots & get it's transform
        GameObject randSlot = SummonableBoardSlots[Random.Range(0, SummonableBoardSlots.Count)];
        Transform randslotTrans = randSlot.transform;

        if (MonsterCards.Count == 0)
        {
            Debug.LogWarning("No monsters to spawn!");
            return;
        }

        BoxCollider slotCollider = randslotTrans.GetComponent<BoxCollider>(); // get board slot box collider
        slotCollider.enabled = false;

        MeshRenderer meshRenderer = randslotTrans.GetComponentInChildren<MeshRenderer>(); // get board slot mesh renderer
        meshRenderer.enabled = false;

        // Get a random index from the Monsterlist
        int randomIndex = Random.Range(0, MonsterCards.Count);

        // Get the random Monster
        GameObject selectedObject = MonsterCards[randomIndex];

        Quaternion MonsterRotation = Quaternion.Euler(selectedObject.transform.rotation.eulerAngles.x, randslotTrans.transform.rotation.eulerAngles.y, selectedObject.transform.rotation.eulerAngles.z);
        if (selectedObject.gameObject.name == "Pheonix") // pheonix has weird rotation bugs
        {
            MonsterRotation = Quaternion.Euler(selectedObject.transform.rotation.eulerAngles.x, randslotTrans.transform.rotation.eulerAngles.y - 90f, selectedObject.transform.rotation.eulerAngles.z - 90f);
        }

        // Summon Special Effect
        GameObject SummonFX = Instantiate(gameManagerRef.SpecialEffects[0], randslotTrans.gameObject.transform.position, randslotTrans.gameObject.transform.rotation);

        // resets the transform of the board so the Monsters doesn't get affected 
        randslotTrans.gameObject.transform.localScale = new Vector3(1, 1, 1);

        // Spawn the Monster at the BoardSlot's position and sets parent 
        GameObject Monster = Instantiate(selectedObject, randslotTrans.gameObject.transform.position, MonsterRotation, randslotTrans.gameObject.transform);
        Monster.tag = "EnemyMonsterCard";

        // Spawn Damage Bar on Monster

        // Instatiate MonsterDamage Info by Monster Position
        TextMeshProUGUI MonsterDamageInfo = Instantiate(MonsterDamagePrefab);

        // Set Prefab as child of MonsterDamageHud Parent in UI
        MonsterDamageInfo.transform.SetParent((GameObject.Find("Monster Damage Hud").transform), false);

        MonsterInfo monsterInfo = Monster.GetComponent<MonsterInfo>();
        // Change Text to Monster Damage
        MonsterDamageInfo.text = monsterInfo.Damage.ToString();


        // Position the text manually above the monster
        RectTransform rectTransform = MonsterDamageInfo.GetComponent<RectTransform>();
        rectTransform.position = Monster.transform.position + new Vector3(-.2f, 1.1f, -0.15f); // Offset text above monster

        // Remove current board slot & monster
        SummonableBoardSlots.Remove(randSlot);
        MonsterCards.Remove(selectedObject);

        //Destroy Effect After 2.5 s
        StartCoroutine(DestroySpecialEffectAfterDelay(2.5f, SummonFX));
    }

    private IEnumerator DestroySpecialEffectAfterDelay(float delay, GameObject Fx)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        Destroy(Fx);
    }

}
