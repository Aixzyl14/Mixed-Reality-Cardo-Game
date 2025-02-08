using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Cards : MonoBehaviour
{
    public bool hasBeenPlayed;
    public GameObject DiscardPile;

    public int handIndex;
    public List<Cards> HandCards = new List<Cards>();

    public bool DrawPhase;
    public bool SpellCard;

    public bool cardPlaced = false;

    // Monster Properties
    public List<GameObject> MonsterCards;

    private GameObject Monster;

    public bool attack;

    // Spell Properties
    public string spellName;

    // Board Elements
    public List<GameObject> BoardCards;
    public bool OnBoardSlot;

    public bool HasSpawnedMonster = false;
    public bool HasDrawnCard = false;

    bool grabactivated = true;
    bool grabactivated2 = false;

    public TextMeshProUGUI MonsterDamageInfo;

    // References
    private GameManager GameManagerRef;
    GameObject deckCardno1;

    private EnemyAi enemyAiRef;

    // UI 
    public TextMeshProUGUI MonsterDamagePrefab;

    // References to XR Grab scripts
    public List<MonoBehaviour> grabScripts;

    private class Monster_Card: Cards
    {
        // Monster Card attributes
        public int BaseAttack;
        int BaseDefence;

        bool Destroyed;
        //spawnMonsters();

    }

    private class Spell_Card : Cards
    {
        // Spell Card attributes
        string Effect;
        ParticleSystem Effectfx;
        bool Used;
    }

    private class Trap_Card : Cards
    {
        // Trap Card attributes
        string Effect;
        ParticleSystem Effectfx;
        bool Used;
    }

    private void Start()
    {
        GameManagerRef = FindObjectOfType<GameManager>();
        enemyAiRef = FindObjectOfType<EnemyAi>();

        DiscardPile = GameObject.FindGameObjectWithTag("DiscardPile"); // Reference Discard Pile

        MonsterCards = GameManagerRef.MonsterCards;

        // Assign from gamemanager ref so all cards has this monster damage prefab assigned to them
        MonsterDamagePrefab = GameManagerRef.MonsterDamagePrefab;

        // Get all monobehaviour scripts attached to this GameObject
        grabScripts = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());

        if (this.CompareTag("MonsterCard"))
        {
            Monster_Card monsterCard = this.AddComponent<Monster_Card>();
        }
        if (this.CompareTag("SpellCard"))
        {
            Spell_Card spellCard = this.AddComponent<Spell_Card>();
        }
        if (this.CompareTag("TrapCard"))
        {
            Trap_Card trapCard = this.AddComponent<Trap_Card>();
        }

    }

    private void OnMouseDown()
    {
     
            UsedCard();
        
    }

    // Play Card Func
    private void UsedCard()
    {
        if (hasBeenPlayed == false)
        {

            hasBeenPlayed = true;


            if (SpellCard)
            {
                //Invoke function move to discard pile to make card go to discard pile
                Invoke("MoveToDiscardPile", 1f);
            }
            // Set available card slot only for the card that was deactivated
            else
            {
                cardPlaced = true;
            }
            GameManagerRef.ReplaceMissingCard(); // Update available slots when a card is deactivated
        }
    }

    void MoveToDiscardPile()
    {
        // add current card to discard pile & get it out of scene
        GameManagerRef.discardPile.Add(this);
        this.transform.SetParent(DiscardPile.gameObject.transform);
        gameObject.SetActive(false); // Deactivate the card in the scene

        
        SpellCard = false;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (!GameManagerRef.TurnEnabledParam.Contains(this.gameObject)) //  Check if the list contains myself
            {
                GameManagerRef.TurnEnabledParam.Add(this.gameObject);
            }
            else
            {
                GameManagerRef.TurnEnabledParam.Remove(this.gameObject);
            }
        }


        // Deck Draw Enable Only during start phase and only 1 card
        if (GameManagerRef.deck.Contains(this) && !GameManagerRef.HandCards.Contains(this) )
        {
            FirstDeckCardEnabled();
        }

        // If monster is destroyed
        if (enemyAiRef.EnemyBoardCards.Contains(Monster))
        {
            Destroy(MonsterDamageInfo);
        }


        attack = GameManagerRef.Attacked;

        //// if Finished attack animation
        //if (attack && GameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase)
        //{

        //    // Assuming that Player1's BoardSlots are under "Player1BoardSlots" parent
        //    GameObject[] allBoardSlots = GameObject.FindGameObjectsWithTag("Player2BoardSlots");

        //    foreach (GameObject boardSlot in allBoardSlots)
        //    {
        //        // Check if the board slot belongs to Player1
        //        if (boardSlot.transform.IsChildOf(GameObject.Find("Player2BoardSlots").transform))
        //        {
        //            // If it's part of Player2's board, check if it contains a "MonsterCard"
        //            foreach (Transform child in boardSlot.transform)
        //            {
        //                foreach (Transform childOfChild in child.transform)
        //                {
        //                    print(child.name);
        //                    // Check if the child is a "MonsterCard"
        //                    if (childOfChild.CompareTag("MonsterCard"))
        //                    {
        //                        // Death Special Effect
        //                        GameObject DeathFX = Instantiate(GameManagerRef.SpecialEffects[1], childOfChild.gameObject.transform.position, childOfChild.gameObject.transform.rotation);
        //                        // Call the delayed destruction
        //                        DelayedDestroy(childOfChild.gameObject);
        //                        Debug.Log("Destroy Monster");

        //                        //Destroy Effect After 2.5 s
        //                        StartCoroutine(DestroyAfterDelay(2.5f, DeathFX));
        //                    }
        //                }
        //            }
        //        }
        //    }
            
        //    // If board slotted
        //    if (OnBoardSlot )
        //    {
        //        // no monster
        //        if (Monster == null)
        //        {
        //            Destroy(gameObject);
        //        }
        //    }

        //}

        // At End Turn of Opponent top card is changed to the next card
        if (GameManagerRef.currentTurnPhase == GameManager.TurnPhases.opponentPostPhase && HasSpawnedMonster)
        {
            //Remove top card from deck
            GameManagerRef.deck.RemoveAt(0);
            HasSpawnedMonster = false;
        }



    }

    private void FirstDeckCardEnabled()
    {
        this.gameObject.SetActive(true);
        // Free Rigid body and Collider if card = deck card
        BoxCollider cardCollider = gameObject.GetComponent<BoxCollider>();
        cardCollider.enabled = false;
        Rigidbody cardrigidbody = gameObject.GetComponent<Rigidbody>();
        cardrigidbody.constraints = RigidbodyConstraints.FreezeAll;

        if (grabactivated)
        {
            // Disable each script except this one
            foreach (MonoBehaviour grabScript in grabScripts) //To Get rid of grab scripts
            {
                // Skip disabling the current script
                if (grabScript != this)
                {
                    grabScript.enabled = false;
                }

            }
            grabactivated = false;
        }
        if (this != null)
        {
            // Get First Card of deck
            Cards firstCardref = GameManagerRef.deck[0];
            deckCardno1 = firstCardref.gameObject;
        }
        

        // If first card = this card set rigid body amd card collider to true & hasn't drawn card yet
        if (GameManagerRef.currentTurnPhase == GameManager.TurnPhases.startPhase && this.gameObject == deckCardno1 && !grabactivated2)
        {
            // change Layer type of gameobject to allow being interactable with board
            // Get the layer index of the card Layer
            int CardsLayer = LayerMask.NameToLayer("Cards");
            this.gameObject.layer = CardsLayer;

            // Changes parent
            GameManagerRef.DrawCard(this, cardCollider, cardrigidbody, grabScripts);

            
         



            grabactivated2 = true;
            HasDrawnCard = true;


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
        // Start the coroutine to destroy the monster after a delay of 3 seconds
        StartCoroutine(DestroyMonsterAfterDelay(child, .8f));
    }

    // Coroutine that waits for a specified time before calling DestroyMonster
    private IEnumerator DestroyMonsterAfterDelay(GameObject child, float delay)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        DestroyMonster(child);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only spawnable during Start phase
        if (GameManagerRef.currentTurnPhase == GameManager.TurnPhases.startPhase)
        {
            if (other.transform.parent.CompareTag("Player1BoardSlots"))
            {
                if (!GameManagerRef.SpawnedMonster && GameManagerRef.MonsterCardsList.Contains(this.gameObject)) // Check if one monster is spawned per turn and is a monster card
                {
                    // make it so One monster is spawned per trigger
                    if (!HasSpawnedMonster)
                    {


                        BoxCollider slotCollider = other.GetComponent<BoxCollider>(); // get board slot box collider
                        slotCollider.enabled = false;

                        MeshRenderer meshRenderer = other.GetComponentInChildren<MeshRenderer>(); // get board slot mesh renderer
                        meshRenderer.enabled = false;

                        boardCardSlotted(other);

                        // Disable each script except this one
                        foreach (MonoBehaviour grabScript in grabScripts) //To Get rid of grab scripts
                        {
                            // Skip disabling the current script
                            try
                            {
                                if (grabScript != this)
                                {
                                    grabScript.enabled = false;
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogError("Exception disabling script: " + e.ToString());
                            }

                        }


                        spawnMonsters(other);

                        GameManagerRef.SpawnedMonster = true;
                        HasSpawnedMonster = true;
                        UsedCard();

                        // Enable Boardslot bool on game object and add the gameobject to the list
                        OnBoardSlot = true;
                        BoardCards.Add(gameObject);


                        //MultiNetworkObjectManager multiNetworkObjectManager = new MultiNetworkObjectManager();
                        //multiNetworkObjectManager.SpawnAndSyncObjects(other.transform.position,other.transform.rotation, other);

                        print("spawn");

                        //HasSpawnedMonster = true;
                    }
                }
                if (GameManagerRef.SpellCardsList.Contains(this.gameObject))
                {
                    // Spell card special effect
                    GameObject SpellFx = Instantiate(GameManagerRef.SpecialEffects[4], this.gameObject.transform.position, this.gameObject.transform.rotation);

                    //Effect activate

                    // Call spell specifier with input of sprite name
                    Spell_Info spell_InfoRef = GameManagerRef.gameObject.GetComponent<Spell_Info>();
                    spell_InfoRef.SpellSpecifier(spellName, this);
                    //Destroy After delay
                    StartCoroutine(DestroyAfterDelay(.3f, SpellFx));
                    StartCoroutine(DestroyAfterDelay(.5f, this.gameObject));
                }
            }
        }
    }

    public void SpellName(string spellname)
    {
        // get spell name from gamemanager
        spellName = spellname;
    }


    private void boardCardSlotted(Collider other)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        // Freeze Rigidbody Components
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Transforms current card -> child of current Board slot 
        transform.SetParent(other.gameObject.transform);
        transform.localPosition = Vector3.zero;

        // Rotates the card to lie flat on board slot
        transform.localRotation = Quaternion.Euler(90, 0, 0);

        // Rotates the card to the same as parent
        transform.rotation = other.transform.rotation;

        // Scale the card to make it fit to the Board slot
        transform.localScale = new Vector3(.175f, .175f, .175f);

        OnBoardSlot = true;
    }

    private void spawnMonsters(Collider other)
    {
        // Check if there are monsters in the list
        if (MonsterCards.Count == 0)
        {
            Debug.LogWarning("No monsters to spawn!");
            return;
        }

        // Get card type info
        Sprite Currentcardsprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        String Currentcardname = Currentcardsprite.name;

        // Find the matching GameObject from the MonsterCards list
        GameObject selectedObject = null;

        foreach (GameObject monster in MonsterCards)
        {
            if (monster.name == Currentcardname) // Compare names
            {
                selectedObject = monster;
                break; // Exit the loop once a match is found
            }
        }
        // 270 rm

        //// Get a random index from the Monsterlist
        //int randomIndex = UnityEngine.Random.Range(0, MonsterCards.Count);

        //// Get the random Monster
        //GameObject selectedObject = MonsterCards[randomIndex];

        Quaternion MonsterRotation = Quaternion.Euler(selectedObject.transform.rotation.eulerAngles.x, other.transform.rotation.eulerAngles.y, selectedObject.transform.rotation.eulerAngles.z);

        if (selectedObject.gameObject.name == "Monster Pheonix") // pheonix has weird rotation
        {
            MonsterRotation = Quaternion.Euler(selectedObject.transform.rotation.eulerAngles.x, other.transform.rotation.eulerAngles.y + 90f, selectedObject.transform.rotation.eulerAngles.z);
        }
        // Summon Special Effect
        GameObject SummonFX = Instantiate(GameManagerRef.SpecialEffects[0], other.gameObject.transform.position, other.gameObject.transform.rotation);

        Vector3 MonsterSpawnPositionNOffset = other.gameObject.transform.position + new Vector3(0f, 0.1f, 0f);
 

        // resets the transform of the board so the Monsters doesn't get affected 
        other.transform.localScale = new Vector3(1, 1, 1);
        // Spawn the Monster at the BoardSlot's position and setting it as a parent
        Monster = Instantiate(selectedObject, other.transform.position, MonsterRotation, other.transform);





        // Spawn Damage Bar on Monster

        // Instatiate MonsterDamage Info by Monster Position
        MonsterDamageInfo = Instantiate(MonsterDamagePrefab);

        // Set Prefab as child of MonsterDamageHud Parent in UI
        MonsterDamageInfo.transform.SetParent((GameObject.Find("Monster Damage Hud").transform), false);

        MonsterInfo monsterInfo = Monster.GetComponent<MonsterInfo>();
        // Change Text to Monster Damage
        MonsterDamageInfo.text = monsterInfo.Damage.ToString();


        // Position the text manually above the monster
        RectTransform rectTransform = MonsterDamageInfo.GetComponent<RectTransform>();

        rectTransform.position = Monster.transform.position + new Vector3(-.2f, 1.1f, -0.15f); // Offset text above monster

        if (selectedObject.gameObject.name == "Monster WolfMonster") // Wolf Taller than the rest
        {
            rectTransform.position = Monster.transform.position + new Vector3(-.2f, 1.4f, -0.15f);
        }


        // If multiplayer spawn the monsters for other user clients
        if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            // Spawn Monster on all connected clients
            NetworkObject MonsternetworkObject = Monster.GetComponent<NetworkObject>();
            MonsternetworkObject.Spawn();
        }


        //Destroy Effect After 2.5 s
        StartCoroutine(DestroyAfterDelay(2.5f, SummonFX));

        // Remove current card from handcards when monster is spawned
        GameManagerRef.HandCards.Remove(this);

        //// Remove the selected monster from the list
        //MonsterCards.Remove(selectedObject);
    }

    private IEnumerator DestroyAfterDelay(float delay, GameObject Fx)
    {
        // Wait for the specified amount of time (3 seconds in this case)
        yield return new WaitForSeconds(delay);

        // Now destroy the child after the delay
        Destroy(Fx);
    }

}
