using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputManagerOVR : MonoBehaviour
{

    GameObject Board1;
    GameObject Board2;

    // Different Stage type
    private enum Stages { Stage1, Stage2 }
    private Stages currentStage;

    public Vector3 Stage2BoardScale = new Vector3(0.107280001f, 0.107280001f, 0.107280001f);
    private Vector3 BoardOriginScale;
    // Vector3(1.94291854,0.559027791,5.89519453)
    private Vector3 Stage2BoardPosition = new Vector3(0.774999976f, 0.344f, 8.923f);
    private Vector3 BoardOriginPosition;

    private bool Board2Active;

    // Detecting where the controller is hovering at
    private Vector3 rightControllerPosition;  // Reference to the right controller position
    private Quaternion rightControllerRotation;  // Reference to the right controller rotation
    public LayerMask hoverLayer;  // Layer Mask to specify what the raycast should hit
    public GameObject hitObject;  // The object currently being hovered by the controller
    private RaycastHit hitInfo;   // Stores info of the object hit by the ray
    public float maxRayDistance = 20f; // Max Distance for the ray

    // UI
    public Image attackIcon; // Attack icon Hover

    //Public Variables
    public bool AttackTriggered = false;
    private bool MonsterHovered = false;

    //References
    GameManager gameManagerRef;

    public Transform rightController;

    private void Awake()
    {
        //Disable Attack Icon at Awake
        attackIcon.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // References
        gameManagerRef = GetComponent<GameManager>();

        // Get reference to the right controller properties
        rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);


        // Ref Board 2
        Board2 = GameObject.FindGameObjectWithTag("Ground2");
        Board2.gameObject.SetActive(false); // On Awake Board 2 is set Inactive

        // Ref Board 1
        Board1 = GameObject.FindGameObjectWithTag("Ground1");
        BoardOriginPosition = Board1.transform.position;
        BoardOriginScale = Board1.transform.localScale;

        currentStage = Stages.Stage1;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForHover();



        if ((OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.RTouch)) && (currentStage == Stages.Stage1))
        {
            Board1.transform.position = Stage2BoardPosition;
            Board1.transform.localScale = Stage2BoardScale;
            print("Stage 2");
            Board2Active = true;


            currentStage = Stages.Stage2;  // Change stage to Stage 2

        }
        else if ((OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.RTouch)) && (currentStage == Stages.Stage2))
        {
            print("Stage 1");
            Board2.SetActive(false);
            Board1.transform.position = BoardOriginPosition;
            Board1.transform.localScale = BoardOriginScale;
            currentStage = Stages.Stage1;  // Change stage to Stage 1
        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch) && MonsterHovered)
        {
            AttackTriggered = true;
        }

        else
        {
            StartCoroutine(ResetAttackTriggered());
            print("no input found");
        }
        if (Board2Active)
        {
            Board2.SetActive(true);
        }
    }

    IEnumerator ResetAttackTriggered()
    {
        yield return new WaitForSeconds(4f); // wait for 4 seconds
        AttackTriggered = false;
    }

    void CheckForHover()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(rightController.position, rightController.forward); // Use the actual controller's transform
        if (Physics.Raycast(ray, out hitInfo, maxRayDistance, hoverLayer))
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            if (hitObject.CompareTag("MonsterCard"))
            {
                Debug.Log("Hovering over Monster: " + hitObject.name);
                // Additional logic to handle hover effect
            }
        }

    }

    IEnumerator delayMonsterHoveredDeactivate()
    {
        yield return new WaitForSeconds(2f); // wait for 1 and a half second
        MonsterHovered = false;
    }
}

