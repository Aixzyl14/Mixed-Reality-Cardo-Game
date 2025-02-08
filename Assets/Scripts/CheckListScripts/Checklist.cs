using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import for TMP_InputField
using UnityEngine.UI;

public class Checklist : MonoBehaviour
{
    //Intro Stage
    public TMP_InputField inputField;
    public Button DoneButton;
    public TextMeshProUGUI IntroText;

    private string Input;

    private List<string> GoalLists = new List<string>(); //Using a List for dynamic resizing

    //Suggestions
    public Button Suggestion1;
    public Button Suggestion2;
    public Button Suggestion3;
    private string Suggestion1Text;
    private string Suggestion2Text;
    private string Suggestion3Text;

    //Goals Stage
    public TextMeshProUGUI GoalsTitle;
    public TextMeshProUGUI GoalText1;
    public TextMeshProUGUI GoalText2;
    public TextMeshProUGUI GoalText3;
    public Button CheckBox1;
    public Button CheckBox2;
    public Button CheckBox3;
    public Sprite[] TickBox;

    //Internal Variables
    public bool BoxReward;


    //Reward System
    private int NoOfRewards;
    public GameObject Star;         // Reference to the Cube GameObject
    public Material[] newMaterials;    // Drag a new Materials here in the Inspector

    private Renderer StarRenderer;  // Renderer to access the material

    private GameObject[] enemies;

    private void Start()
    {
        GoalText1.text = "";
        GoalText2.text = "";
        GoalText3.text = "";
        GoalsTitle.gameObject.SetActive(false);
        GoalText1.gameObject.SetActive(false);
        GoalText2.gameObject.SetActive(false);
        GoalText3.gameObject.SetActive(false);

        // Assign the SuggestionClicked method with different identifiers to each SuggestionButton
        Suggestion1.onClick.AddListener(() => SuggestionClicked(1));
        Suggestion2.onClick.AddListener(() => SuggestionClicked(2));
        Suggestion3.onClick.AddListener(() => SuggestionClicked(3));
        Suggestion1Text = Suggestion1.GetComponentInChildren<TextMeshProUGUI>().text;
        Suggestion2Text = Suggestion2.GetComponentInChildren<TextMeshProUGUI>().text;
        Suggestion3Text = Suggestion3.GetComponentInChildren<TextMeshProUGUI>().text;

        CheckBox1.gameObject.SetActive(false);
        CheckBox2.gameObject.SetActive(false);
        CheckBox3.gameObject.SetActive(false);

        // Assign the GoalTicked method with different identifiers to each checkbox
        CheckBox1.onClick.AddListener(() => GoalTicked(1));
        CheckBox2.onClick.AddListener(() => GoalTicked(2));
        CheckBox3.onClick.AddListener(() => GoalTicked(3));

        // Get the Renderer component from the Cube
        StarRenderer = Star.GetComponent<Renderer>();

        //Finds all gameobject with tag Enemy
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Get value from inputfield
        inputField.onEndEdit.AddListener(OnInputFieldChange);
    }

    private void Update()
    {
        if (BoxReward)
        {
            // Change the Cube's material to the new one
            StarRenderer.material = newMaterials[NoOfRewards];

            // Transforms the cube size if Goals = 3
            if (NoOfRewards >= 2)
            {
                Star.transform.localScale = new Vector3(40, 40, 30);

                //Destroys all enemies after 3 check list
                foreach (GameObject enemy in enemies)
                {
                    Destroy(enemy);
                }
            }
            // iterate the NoOfRewards
            NoOfRewards += 1;
            Debug.Log("Reward " + NoOfRewards);
            BoxReward = false;

        }
    }
    public void OnInputFieldChange(string s)
    {
        Debug.Log("Input received: " + s); // Add a debug line
        Input = s;

        // Add the string value to the list
        GoalLists.Add(Input);

        Input = "";
        inputField.text = Input;
        for (int i = 0; i < GoalLists.Count; i++)
        {
            Debug.Log(i + 1 + " " + GoalLists[i]);
        }
    }

    public void DonePressed()
    {
        Debug.Log("pressed");
        Destroy(inputField.gameObject);
        Destroy(DoneButton.gameObject);
        Destroy(IntroText.gameObject);
        ActivateGoals();
    }

    public void GoalTicked(int checkboxID)
    {
        switch (checkboxID)
        {
            case 1:
                // CheckBox1 clicked
                CheckBox1.image.sprite = TickBox[0]; // Set sprite for checkbox 1
                break;
            case 2:
                // CheckBox2 clicked
                CheckBox2.image.sprite = TickBox[1]; // Set sprite for checkbox 2
                break;
            case 3:
                // CheckBox3 clicked
                CheckBox3.image.sprite = TickBox[2]; // Set sprite for checkbox 3
                break;
            default:
                Debug.Log("Invalid checkbox ID");
                break;
        }
        BoxReward = true;
    }

    private void ActivateGoals()
    {
        GoalsTitle.gameObject.SetActive(true);

        // Check if there are enough goals in the list to avoid accessing non-existent indices
        if (GoalLists.Count > 0)
        {
            GoalText1.text = GoalLists[0];
            GoalText1.gameObject.SetActive(true);
            CheckBox1.gameObject.SetActive(true);
        }

        if (GoalLists.Count > 1)
        {
            GoalText2.text = GoalLists[1];
            GoalText2.gameObject.SetActive(true);
            CheckBox2.gameObject.SetActive(true);
        }

        if (GoalLists.Count > 2)
        {
            GoalText3.text = GoalLists[2];
            GoalText3.gameObject.SetActive(true);
            CheckBox3.gameObject.SetActive(true);
        }

    }

    public void SuggestionClicked(int suggestionID)
    {
        switch (suggestionID)
        {
            case 1:
                // Suggestion clicked
                // Add the string value to the list
                GoalLists.Add(Suggestion1Text);
                Destroy(Suggestion1.gameObject);
                Debug.Log(Suggestion1Text);
                //  GoalLists.Add(SuggestionValue1);
                break;
            case 2:
                // Suggestion clicked
                // Add the string value to the list
                GoalLists.Add(Suggestion2Text);
                Destroy(Suggestion2.gameObject);
                Debug.Log(Suggestion2Text);
                //  GoalLists.Add(SuggestionValue2);
                break;
            case 3:
                // Suggestion clicked
                // Add the string value to the list
                GoalLists.Add(Suggestion3Text);
                Destroy(Suggestion3.gameObject);
                Debug.Log(Suggestion3Text);
                //  GoalLists.Add(SuggestionValue3);
                break;
            default:
                Debug.Log("Invalid Suggestion ID");
                break;


        }
    }


}
