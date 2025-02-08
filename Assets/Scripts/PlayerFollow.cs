using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerFollow : MonoBehaviour
{
    // Player Information
    private Transform PlayerTransform;
    private Quaternion currentRotation;
    public Vector3 PlayerRotation;

    // Card Follow Data
    public List<GameObject> BoardSlotsRef;
    public GameObject[] CardRef;
    private BoxCollider boxCollider;
    private List<Transform> CardTransforms = new List<Transform>(); // Create a list of 5 index with null value
    public float proximityThreshold = 1.05f; // 100 cm in meters
    private int previousCardCount;

    private bool DistanceCheckComplete;
    public bool CardHeld;

    private enum CardShown {CardInactive, CardActive };
    CardShown currentCardShown;

    

    // Start is called before the first frame update
    void Start()
    {
        // Get Reference from parent i.e. XRController Transform
        PlayerTransform = transform.parent.GetComponent<Transform>();

        GameObject[] ParentBoardSlot = GameObject.FindGameObjectsWithTag("Player1BoardSlots");


        // Loop through each found BoardSlot
        for (int i=0; i<ParentBoardSlot.Length; i++)
        {
            // Iterate over all child GameObjects of each ParentBoardSlot[i]
            foreach (Transform child in ParentBoardSlot[i].transform)
            {
                // Add the child GameObject to BoardSlotsRef list
                BoardSlotsRef.Add(child.gameObject);
            }
        }
        
      
        CardRef = GameObject.FindGameObjectsWithTag("HandCards");


       
    }



    // Update is called once per frame
    void Update()
    {

        // Continuously update the PlayerRotation from the Transform's rotation
        //if (PlayerTransform != null)
        //{
        //    currentRotation = PlayerTransform.rotation;
        //    PlayerRotation = currentRotation.eulerAngles;

        //    transform.rotation = currentRotation;
        //}


        // Check if the number of cards has changed
        if (CardRef.Length != previousCardCount)
        {
            //UpdateCardTransform();
            previousCardCount = CardRef.Length;
        }

        ComparePlayerNCardDistance();



        CheckCardHeld();
        
        // If Oculus Right Controller Button A being held is inputted
        if ((OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) && (currentCardShown == CardShown.CardInactive))
        {
            foreach (var Card in CardRef)
            {
                Transform cardTransform = Card.transform;
                if (cardTransform != null)
                {
                    cardTransform.gameObject.SetActive(false);
                    print(cardTransform.gameObject + " is off");
                }

            }
            currentCardShown = CardShown.CardActive;
        }

        else if ((OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) && (currentCardShown == CardShown.CardActive))
        {
            foreach (var Card in CardRef)
            {
                Transform cardTransform = Card.transform;
                if (cardTransform != null)
                {
                    cardTransform.gameObject.SetActive(true);
                    print(cardTransform.gameObject + " is on");
                }
                currentCardShown = CardShown.CardInactive;
            }
        }

       
    }

    private void CheckCardHeld()
    {
        if (CardHeld)
        {
            for (int n = 0; n < BoardSlotsRef.Count; n++)
            {
                boxCollider = BoardSlotsRef[n].GetComponent<BoxCollider>();
                boxCollider.isTrigger = true;
            }
        }
        else if (!CardHeld)
        {
            for (int n = 0; n < BoardSlotsRef.Count; n++)
            {
                boxCollider = BoardSlotsRef[n].GetComponent<BoxCollider>();
                boxCollider.isTrigger = false;
            }
        }
    }

    private void ComparePlayerNCardDistance()
    {
        float proximityThresholdSquared = proximityThreshold * proximityThreshold; //Proximity distance for grab hold
        // Check distance between player and each card
        foreach (var card in CardRef)
        {
            Transform cardTransform = card.gameObject.transform;
            if (cardTransform != null)
            {
                float distanceSquared = (PlayerTransform.position - cardTransform.position).sqrMagnitude;


                if (CardHeld) // Disable Card Held if not grabbed
                {
                    if (!(distanceSquared <= proximityThresholdSquared))
                    {
                        CardHeld = true;
                    }
                }
                if (distanceSquared <= proximityThresholdSquared) // Enable Card Held if grabbed
                {
                    Debug.Log($"Player is within 20cm of card: {cardTransform.name}");
                    CardHeld = true;
                }
                //if (CardHeld) // Disable Card Held if not grabbed
                //    {
                //        if (!(distanceSquared <= proximityThresholdSquared))
                //        {
                //            DistanceCheckComplete = true;
                //        }
                //    }


                //else
                //{
                //    CardHeld = false;
                //}
            }
        }
    }

    //private void UpdateCardTransform()
    //{
    //    foreach (var card in CardRef)
    //    {
    //        if (card != null)
    //        {
    //            CardTransforms.Add(card.transform);
    //        }

    //    }

    //}

}

