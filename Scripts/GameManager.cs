using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Oculus.Interaction;
using Oculus.Avatar2;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    // Deck Properties
    public List<Cards> deck = new List<Cards>();
    public List<Cards> discardPile = new List<Cards>();
    public List<Cards> HandCards = new List<Cards>();
    // Contains the different card sprites
    public List<Sprite> DeckCardsSprite = new List<Sprite>();

    private bool cardPlaced = false;
    private int prevHandCardAmount = 0;

    public Transform[] cardSlots;
    public bool[] availableCardSlots;

    public TextMeshProUGUI deckSizeText;
    public TextMeshProUGUI discardPileText;

    public List<GameObject> MonsterCards = new List<GameObject>();

    // list of Type of card
    public List<GameObject> MonsterCardsList = new List<GameObject>();
    public List<GameObject> SpellCardsList = new List<GameObject>();
    public List<string> MonsterCardsNames = new List<string>();
    public List<string> SpellCardsNames = new List<string>();

    // Cards Script Ref:
    private Cards cardRef;
    private GameObject deckCardno1;
    Cards firstCardref;
    public List<GameObject> PlayerBoardCards = new List<GameObject>();

    // Board Reference
    private GameObject[] BoardSlots;
    public bool Attacked;
    private List<Transform> PreviousboardSlot = new List<Transform>();

    public bool DrawnCard = false;
    public bool SpawnedMonster = false;

    // References
    EnemyAi enemyAiRef;

    // Ui Properties
    public TextMeshProUGUI MonsterDamagePrefab;


    // Special Effects
    public GameObject[] SpecialEffects;

    // Turn Phase
    // Player Phase
    public enum TurnPhases { startPhase, postPhase, battlePhase, opponentStartPhase, opponentBattlePhase, opponentPostPhase } // Different Phases in a player turn & Opponent Turn
    public TurnPhases currentTurnPhase; // Current Phase within a Turn
    private int currentTurnPhaseIndex; // Current Phase index

    [SerializeField]
    private Sprite[] currentTurnImage; // Current Turn Icons
    public Image currentTurnIcon;

    //Misc Variables
    int x = 0;
    string prevString;


    bool SummonMonster;
    bool SetSpellsNTrap;
    bool ActivateTrap;
    bool ActivateSpell;
    bool Fusion;
    bool Draw;
    bool Discard;
    bool SetMonsterMode;
    bool MonsterAttack;
    bool HandSpell;

    public List<GameObject> TurnEnabledParam = new List<GameObject>();

    // Enemy Phase


    void Start()
    {
        // Get First Card of deck
        firstCardref = deck[0];
        deckCardno1 = firstCardref.gameObject;

        // References
        cardRef = FindAnyObjectByType<Cards>();
        enemyAiRef = FindAnyObjectByType<EnemyAi>();

        // Make Current Turn Phase at the start of turn = OpponentPhase & Current Turn Phase Index = 0
        currentTurnPhase = TurnPhases.opponentPostPhase;
        currentTurnPhaseIndex = 0;

        // Add the Turn Enabled Parameters into the List
        //TurnEnabledParam.AddRange(new List<bool> { SummonMonster, SetSpellsNTrap, ActivateTrap, ActivateSpell, Fusion, Draw, Discard, SetMonsterMode, MonsterAttack, HandSpell });

        //Find BoardSlots
        BoardSlots = GameObject.FindGameObjectsWithTag("Player1BoardSlots");

        // Go through each hand card
        for (int i = 0; i < HandCards.Count; i++)
        {


            print("hand =  " + HandCards[i].gameObject);
            CardTypePicker(i);
        }
    }

    private void CardTypePicker(int i = 0)
    {
        // Choose Card type sprite
        int randomIndex = Random.Range(0, DeckCardsSprite.Count);

        // randomly picks out of deck
        Sprite Currentcardsprite = DeckCardsSprite[randomIndex];

        // Get reference of the hand cards sprite renderer and replace it with the chosen card type
        SpriteRenderer CurrentcardspriteHolder = HandCards[i].gameObject.GetComponent<SpriteRenderer>();
        CurrentcardspriteHolder.sprite = Currentcardsprite;

        //Assign monster card or spell card
        if (MonsterCardsNames.Contains(CurrentcardspriteHolder.sprite.name))
        {
            // if sprite = monster, add card to monster cards list
            MonsterCardsList.Add(CurrentcardspriteHolder.gameObject);
        }
        if (SpellCardsNames.Contains(CurrentcardspriteHolder.sprite.name))
        {
            // if sprite = spell, add card to spell cards list
            SpellCardsList.Add(CurrentcardspriteHolder.gameObject);
            
            Cards cards = CurrentcardspriteHolder.gameObject.GetComponent<Cards>();
            cards.SpellName(CurrentcardspriteHolder.sprite.name);


        }


        // Avoid more than 2 of same cards is spawned
        DeckCardsSprite.Remove(Currentcardsprite);
    }

    public void ReplaceMissingCard()
    {

        // Remove all null cards from HandCards
        HandCards.RemoveAll(card => card == null);

        if (HandCards.Count < 5)
        {
            // correct amount of hand cards
            for (int n = 0; n < HandCards.Count; n++)
            {
                GameObject Gameslot = GameObject.Find("GameSlot(" + (n + 1) + ")");
                if (Gameslot != null)
                {
                    print(Gameslot.name);
                HandCards[n].transform.SetParent(Gameslot.transform);
                //Reset transform
                ResetCardTransform(HandCards[n].transform);
                }
                else
                {
                    Debug.LogError($"GameSlot({n + 1}) not found!");
                }
            }
        }

    }

    //Draw Card function
    // Has optional boxcollider, rigidbody for the card
    public void DrawCard(Cards firstCard, BoxCollider cardCollider = null, Rigidbody cardrigidbody = null, List<MonoBehaviour> grabScripts = null)
    {
        // Only accessible during draw phase
        if (currentTurnPhase == TurnPhases.startPhase)
        {
            print("draw");
            if (deck.Count >= 1)
            {
                // Add the card to handcards
                HandCards.AddFirst(firstCard);
                // Pick card type
                CardTypePicker();

                // Change parent to make it under Hand card gameobject
                firstCard.transform.SetParent(cardSlots[0], true);
                //Reset transform
                ResetCardTransform(firstCard.transform);



                // Enables movement of card and able to be grabbbed
                cardCollider.enabled = true;
                cardrigidbody.constraints = RigidbodyConstraints.FreezePositionY; // occasionally card keeps floating bug, temp fix

                // Re-enable Grab script
                foreach (MonoBehaviour grabScript in grabScripts)
                {

                    grabScript.enabled = true;

                }

                //// First, check for available slots and handle the new card
                //for (int i = 0; i < HandCards.Count; i++)
                //{
                //    // Replace the inactive card at index 'i' with the new randCard
                //    HandCards[i] = firstCard;

                //    firstCard.gameObject.SetActive(true);
                //    firstCard.handIndex = i;

                //    // Set the parent to the left-most card slot (index 0)
                //    firstCard.transform.SetParent(cardSlots[i], false);
                //    firstCard.transform.localPosition = Vector3.zero;

                //    availableCardSlots[i] = false;  // Mark the slot as used




                //    return;

                //}

                //deck.Remove(firstCard);  // Remove the card from the deck

                // If no inactive card is found, shift cards to make space for the new card
                if (HandCards.Count < 5)  // Check if there is space to add the card
                {
                    // Shift cards to the right by 1 starting from the last card
                    for (int j = HandCards.Count - 1; j >= 0; j--)
                    {
                        if (j + 1 < 5) // Ensure we're not exceeding the cardSlots size
                        {
                            //left for later *to fix

                            // Move the card's transform to the next available slot (shift right)
                            //HandCards[j].transform.SetParent(cardSlots[j + 1], false);
                            //HandCards[j].transform.localPosition = Vector3.zero;

                            // Update the HandCards list to reflect the shift
                            HandCards[j + 1] = HandCards[j];

                        }
                    }

                    // Add the new card to the first available index (index 0)
                    HandCards[0] = firstCard;

                    firstCard.gameObject.SetActive(true);
                    firstCard.handIndex = 0;  // Set hand index = 0

                    // Set the parent to the card slot in hand (index 0)
                    firstCard.transform.SetParent(cardSlots[0], false);
                    firstCard.transform.localPosition = Vector3.zero;

                    availableCardSlots[0] = false;  // Mark slot 0 as used
                    deck.Remove(firstCard);  // Remove the card from the deck

                }
            }
        }
    }




    private void Update()
    {
        // updates the deck size & discard pile per frame
        deckSizeText.text = deck.Count.ToString();
        discardPileText.text = discardPile.Count.ToString();


        // If current Turn phase index = 0 make it opponent phase
        if (currentTurnPhaseIndex == 0)
        {
            currentTurnPhase = TurnPhases.opponentPostPhase;

        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            NextPhase();
        }

        if (currentTurnPhase == TurnPhases.startPhase)
        {
            if (prevHandCardAmount == 0)
            {
                prevHandCardAmount = HandCards.Count;
            }
            // Check if hand card amount has changed
            if (prevHandCardAmount != HandCards.Count)
            {
                ReplaceMissingCard();
                prevHandCardAmount = HandCards.Count;
            }

        }


        //if (TurnEnabledParam.Contains(SummonMonster) && Input.GetKeyDown(KeyCode.C))
        //{
        //    print("Monster Summoned");
        //}

        // Update Drawn Card
        DrawnCard = firstCardref.HasDrawnCard;


        // Reset card position
        ResetCardPosition();

      
     



        //// Update Spawn Monster
        //SpawnedMonster = cardRef.HasSpawnedMonster;

        if (SummonMonster && Input.GetKeyDown(KeyCode.C))
        {
            print("Monster Summoned");
        }

        if (Input.GetKeyDown(KeyCode.B) && (currentTurnPhase == TurnPhases.battlePhase))
        {
            if (PlayerBoardCards.Count > 0)
            {
                print("Attacked");
                Attacked = true;
            }
            else
            {
                print("Unable to Attack");
            }
        }
    }

    /// <summary>
    /// Check the Playerboard for monsters and if so add them to PlayerBoard List
    /// </summary>
    private void AddToPlayerBoard()
    {
        for (int n = 0; n < BoardSlots.Length; n++)  // Iterate through each PlayerBoardSlot
        {
            Transform boardSlot = BoardSlots[n].transform;
            
                // Check if the current BoardSlot has children
                if (boardSlot.childCount > 0)
                {
                    ////Check if boardslot child count has changed, if not return
                    //if (PreviousboardSlot[n].childCount != boardSlot.childCount || PreviousboardSlot.Count == 0)
                    //{
                    //PreviousboardSlot.Add(boardSlot);

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

                                // Check if the child has the "MonsterCard" tag
                                if (childOfChild.CompareTag("MonsterCard"))
                                {
                                    Debug.Log("Found a monster card with the 'MonsterCard' tag: " + childOfChild.name);
                                    PlayerBoardCards.Add(childOfChild.gameObject);
                                }
                            }
                        }
                    }
                //}
                //    else
                //    {
                //    // board slot N has not changed
                //    }
            }
        }
    }

    /// <summary>
    /// Checks if card has been dropped then resets the position and rotation to origin hand slot
    /// </summary>

    private void ResetCardPosition()
    {
        // Check all the Handspots
        for (int i = 0; i < HandCards.Count; i++)
        {
            Grabbable grabbable = HandCards[i].GetComponent<Grabbable>();
            if (grabbable.isDropped)
            {
                StartCoroutine(ResetCardsAfterDelay(i, 3f));
                print("Dropped");
            }

        }
    }
    private IEnumerator ResetCardsAfterDelay( int i, float delay)
    {
        // Wait for the specified amount of time (0.75 second in this case)
        yield return new WaitForSeconds(delay);
        ResetCardTransform(HandCards[i].gameObject.transform);
        //HandCards[i].gameObject.transform.localPosition = Vector3.zero;
        //HandCards[i].gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void NextPhase()
    {
        currentTurnPhaseIndex += 1;

        PhaseManagement(currentTurnPhaseIndex);
    }

    private void PhaseManagement(int currentTurnPhaseIndexNum)
    {
        //Make Everything false
        SummonMonster = false;
        SetSpellsNTrap = false;
        ActivateTrap = false;
        ActivateSpell = false;
        Fusion = false;
        Draw = false;
        Discard = false;
        SetMonsterMode = false;
        MonsterAttack = false;
        HandSpell = false;


        // Every start of Turn, Set Current to 1
        if (currentTurnPhaseIndexNum is 0)
        {
            currentTurnPhaseIndexNum = 1;
        }



        switch (currentTurnPhaseIndexNum)
        {
            // Start Phase
            // Enable Summon Monster, Set Spells & Trap, Activate Spells & Trap, Fusion, Draw Card, Discard, Set Monster Mode
            case 1:
                print("Start Phase");

                currentTurnIcon.sprite = currentTurnImage[1]; // Start Phase Icon

                SpawnedMonster = false; // allow spawn monster
                SummonMonster = true;
                SetSpellsNTrap = true;
                ActivateTrap = true;
                ActivateSpell = true;
                Fusion = true;
                Draw = true;
                Discard = true;
                SetMonsterMode = true;
                currentTurnPhase = TurnPhases.startPhase;

                ////Check for player board slots and add its to BoardCards list
                //AddToPlayerBoard();

                break;

            // Battle Phase
            // Enable MonsterAttack, Set Monster Mode, Activate Spells & Trap, Fusion
            case 2:
                print("Battle Phase");

                currentTurnIcon.sprite = currentTurnImage[2]; // Battle Phase Icon

                MonsterAttack = true;
                SetMonsterMode = true;
                ActivateSpell = true;
                ActivateTrap = true;
                Fusion = true;
                currentTurnPhase = TurnPhases.battlePhase;

                //Check for player board slots and add its to BoardCards list
                AddToPlayerBoard();

                break;

            // Post Phase
            // Enable Set Mosnter Mode, Set Spells & Trap, Discard, Fusion
            case 3:
                print("Post Phase");

                currentTurnIcon.sprite = currentTurnImage[3]; // Post Phase Icon

                Attacked = false;
                SummonMonster = true;
                SetMonsterMode = true;
                SetSpellsNTrap = true;
                Fusion = true;
                Discard = true;
                currentTurnPhase = TurnPhases.postPhase;

                ////Check for player board slots and add its to BoardCards list
                //AddToPlayerBoard();

                break;

            case 4:
                print("Opponent Start Phase");

                currentTurnIcon.sprite = currentTurnImage[0]; // Opponent Phase Icon

                currentTurnPhase = TurnPhases.opponentStartPhase;
                SummonMonster = true;
                break;

            case 5:
                print("Opponent Battle Phase");

                currentTurnIcon.sprite = currentTurnImage[2]; // Opponent Battle Icon

                HandSpell = true;
                currentTurnPhase = TurnPhases.opponentBattlePhase;
                break;
            case 6:
                print("Opponent Post Phase");

                Attacked = false;
                currentTurnIcon.sprite = currentTurnImage[3]; // Opponent Battle Icon

                currentTurnPhaseIndex = 0;
                currentTurnPhase = TurnPhases.opponentPostPhase;
                break;

            default: //something went wrong with phase management
                print("Error Phase Unknown");
                break;
        }


        //if (currentTurnPhaseIndexNum > 5) // Once it is over than 3 after the switch statemments reset current turn phase index
        //{
            

        //    // After Final stage set all bools to false
        //    ResetPhaseParam();
        //}

    

    }

    public void TutorialButtonPressed()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void MultiplayerButtonPressed()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    private void ResetPhaseParam()
    {
        // At start of every stage set all bools to false
        for (int n = 0; n < TurnEnabledParam.Count; n++)
        {
            //TurnEnabledParam[n] = false;
        }
    }


    public static void ResetCardTransform(Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = new Vector3(0.03780007f, 0.03780007f, 0.03780007f);
    }






}


