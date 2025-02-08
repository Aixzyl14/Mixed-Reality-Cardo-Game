using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Oculus.Interaction;
using Oculus.Interaction.Surfaces;
using Oculus.Interaction.Input;
using Oculus.Platform;

public class InputManager : MonoBehaviour
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

    //Locomotion
    public OVRInput.Controller controller;
    public FromOVRControllerDataSource rightcontroller;
    public FromOVRControllerDataSource leftcontroller;
    public float speed = 1.5f;

    // Detecting where the controller is hovering at
    private Vector3 rightControllerPosition;  // Reference to the right controller position
    private Quaternion rightControllerRotation;  // Reference to the right controller rotation
    public LayerMask raycastLayerMask;  // Layer Mask to specify what the raycast should hit
    public GameObject hitMonster;  // The object currently being hovered by the controller
    private RaycastHit hitInfo;   // Stores info of the object hit by the ray
    public float maxRayDistance = 20f; // Max Distance for the ray

    // Handle Vr Camera rotation
    public Transform VrCamera;
    public Transform PlayerRig;
    private float rotationSpeed = 100.0f;

    public RayInteractor righthandRayInteractor; // The ray Interactor setup on the righthand

    // UI
    public Image attackIcon; // Attack icon Hover
    public Image TriggerButtonIcon; //Quest Trigger button Icon

    //Public Variables
    public bool AttackTriggered = false;
    private bool MonsterHovered = false;

    //References
    GameManager gameManagerRef;


    private void Awake()
    {
        //Disable Attack Icon at Awake
        attackIcon.enabled = false;
        TriggerButtonIcon.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Reference to GameManager
        gameManagerRef = FindObjectOfType<GameManager>();

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

    }

    // Update is called once per frame
    void Update()
    {
        // Enable Locomotion
        Locomotion();



        //if ((OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.RTouch)) && (currentStage == Stages.Stage1))
        //{
        //    Board1.transform.position = Stage2BoardPosition;
        //    Board1.transform.localScale = Stage2BoardScale;
        //    print("Stage 2");
        //    Board2Active = true;


        //    currentStage = Stages.Stage2;  // Change stage to Stage 2

        //}
        //else if ((OVRInput.GetDown(OVRInput.RawButton.B, OVRInput.Controller.RTouch)) && (currentStage == Stages.Stage2))
        //{
        //    print("Stage 1");
        //    Board2.SetActive(false);
        //    Board1.transform.position = BoardOriginPosition;
        //    Board1.transform.localScale = BoardOriginScale;
        //    currentStage = Stages.Stage1;  // Change stage to Stage 1
        //}
        if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase)
        {
            if (!AttackTriggered)
            {
                CheckForHover();
                //Hover();
            }
            if (MonsterHovered)
            {

                if ((OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch)))
                {
                    MonsterInfo monsterInfo = hitMonster.GetComponent<MonsterInfo>();
                    monsterInfo.MonsterAttack();
                    print("bob");
                    AttackTriggered = true;
                    print("Attack");
                    MonsterHovered = false;
                }
            }
        }

        if (gameManagerRef.currentTurnPhase != GameManager.TurnPhases.battlePhase)
        {
            MonsterHovered = false;
            AttackTriggered = false;
            //Disable Icons
            attackIcon.enabled = false;
            TriggerButtonIcon.enabled = false;
        }


        //else
        //{
        //    StartCoroutine(ResetAttackTriggered());
        //    print("no input found");
        //}
        if (Board2Active)
        {
            Board2.SetActive(true);
        }
    }

    private void Locomotion()
    {
        //Locommotion
        // Get the thumbstick input
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

        // Get the forward and right directions relative to the VR camera
        Vector3 forward = VrCamera.forward;
        Vector3 right = VrCamera.right;

        // Flatten the forward and right vectors on the horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction based on input and camera direction
        Vector3 movementDirection = forward * primaryAxis.y + right * primaryAxis.x;

        // Move the player
        transform.position += movementDirection * speed * Time.deltaTime;

        // Camera rotation using the left controller
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        if (secondaryAxis.x != 0) // Only rotate if there's input on the X-axis
        {
            float rotationAmount = secondaryAxis.x * rotationSpeed * Time.deltaTime;
            PlayerRig.Rotate(0, rotationAmount, 0); // Rotate the player rig around the Y-axis
        }
    }

    IEnumerator ResetAttackTriggered()
    {
        yield return new WaitForSeconds(4f); // wait for 4 seconds
        AttackTriggered = false;
    }

    void CheckForHover()
    {
        // Get reference to the right controller properties
        rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 rightControllerDirection = rightControllerRotation * Vector3.forward;
        // Create a ray from the controller position and in the direction the controller is pointing
        Ray ray = new Ray(rightControllerPosition, rightControllerDirection);

        // Perform the raycast to detect any objects that the ray intersects
        if (Physics.Raycast(ray, out hitInfo, maxRayDistance, raycastLayerMask))
        {
            hitMonster = hitInfo.collider.gameObject;

            // Check if the object hit by the ray is interactable
            if (hitMonster.CompareTag("MonsterCard"))
            {
                // Handle Interaction with the object
                Debug.Log("Hovering over Monster: " + hitMonster.name);

                if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase)
                {
                    // Change attack Icon Position and Rotation + offset
                    Vector3 newAttackIconPos = new Vector3(hitMonster.transform.position.x, hitMonster.transform.position.y + 1.1f, hitMonster.transform.position.z - 0.2f); // Offset, To make it Above the monster
                    if (hitMonster.name == "Monster WolfMonster")
                    {
                        newAttackIconPos = new Vector3(hitMonster.transform.position.x, hitMonster.transform.position.y + 1.5f, hitMonster.transform.position.z - 0.2f); // Wolf taller than the rest
                    }

                    attackIcon.transform.position = newAttackIconPos;
                    attackIcon.transform.rotation = new Quaternion(attackIcon.transform.rotation.x, rightControllerRotation.y, attackIcon.transform.rotation.z, attackIcon.transform.rotation.w);
                    //Enable Attack Icon
                    attackIcon.enabled = true;

                    // Change TriggerButton Icon Position and Rotation + offset
                    Vector3 newTriggerButtonPos = new Vector3(hitMonster.transform.position.x + .4f, hitMonster.transform.position.y + .6f, hitMonster.transform.position.z - 0.34f); // Offset, To make it Above the monster
                    TriggerButtonIcon.transform.position = newTriggerButtonPos;
                    TriggerButtonIcon.transform.rotation = new Quaternion(attackIcon.transform.rotation.x, rightControllerRotation.y, attackIcon.transform.rotation.z, attackIcon.transform.rotation.w);
                    //Enable TriggerButton Icon
                    TriggerButtonIcon.enabled = true;

                    MonsterHovered = true;

                  

                        //StartCoroutine(delayIconDeactivate());
                    }
            }
            //else 
            //{
            //    StartCoroutine(delayMonsterHoveredDeactivate()); // Delayed deactivate of monster Hovered
            //}
        }

    }

    //void Hover()
    //{
    //    ControllerDataAsset controllerDataAsset = rightcontroller.GetComponent<ControllerDataAsset>();
    //    // Get the pointer pose (tip of the controller)
    //    Vector3 RpointerPosition = controllerDataAsset.PointerPose.position;
    //    Quaternion RpointerRotation = controllerDataAsset.PointerPose.rotation;

    //    // Create a ray from the pointer position and forward direction
    //    Ray ray = new Ray(RpointerPosition, RpointerRotation * Vector3.forward);

    //    // Perform the raycast
    //    if (Physics.Raycast(ray, out RaycastHit hitInfo, maxRayDistance, raycastLayerMask))
    //    {
    //        // Log the object hit by the ray
    //        Debug.Log($"Pointer hit: {hitInfo.collider.gameObject.name} at {hitInfo.point}");
    //    }
    //}

    //void CheckForHover()
    //{
        //if (righthandRayInteractor != null && righthandRayInteractor.CollisionInfo.HasValue)
        //{
        //    // Extract hit information from RayInteractor
        //    SurfaceHit hit = righthandRayInteractor.CollisionInfo.Value;
        //    Vector3 hitPoint = hit.Point;
        //    Vector3 hitNormal = hit.Normal;

        //    Debug.Log($"Surface hit at position {hitPoint} with normal {hitNormal}");

        //    // Check if the hit object implements ISurface
        //    ISurface surface = righthandRayInteractor.transform.GetComponent<ISurface>();
        //    if (surface != null)
        //    {
        //        hitObject = surface.Transform.gameObject;

        //        if (hitObject.CompareTag("MonsterCard"))
        //        {
        //            Debug.Log($"Hovering over MonsterCard: {hitObject.name}");

        //            // Check game phase for battle logic
        //            if (gameManagerRef.currentTurnPhase == GameManager.TurnPhases.battlePhase)
        //            {
        //                // Update Attack Icon position and rotation
        //                Vector3 newAttackIconPos = hitObject.transform.position + new Vector3(0, 1.1f, -0.2f); // Offset above the monster
        //                attackIcon.transform.position = newAttackIconPos;
        //                attackIcon.transform.rotation = Quaternion.Euler(0, rightControllerRotation.eulerAngles.y, 0);
        //                attackIcon.enabled = true;

        //                // Update TriggerButton Icon position and rotation
        //                Vector3 newTriggerButtonPos = hitObject.transform.position + new Vector3(0.4f, 0.6f, -0.34f); // Offset
        //                TriggerButtonIcon.transform.position = newTriggerButtonPos;
        //                TriggerButtonIcon.transform.rotation = Quaternion.Euler(0, rightControllerRotation.eulerAngles.y, 0);
        //                TriggerButtonIcon.enabled = true;

        //                MonsterHovered = true;
        //            }
        //        }
        //        else
        //        {
        //            // Reset state if not a MonsterCard
        //            ResetHoverState();
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogWarning("The RayInteractor's hit object does not implement ISurface.");
        //        ResetHoverState();
        //    }
        //}
        //else
        //{
        //    // Reset hover state if no valid CollisionInfo
        //    ResetHoverState();
        //}
    //}

    void ResetHoverState()
    {
        MonsterHovered = false;
        attackIcon.enabled = false;
        TriggerButtonIcon.enabled = false;
        Debug.Log("Hover state reset.");
    }


    IEnumerator delayIconDeactivate()
    {
        yield return new WaitForSeconds(2f); // wait for 1 and a half second
        attackIcon.enabled = false;
        TriggerButtonIcon.enabled = false;

     
    }
}

