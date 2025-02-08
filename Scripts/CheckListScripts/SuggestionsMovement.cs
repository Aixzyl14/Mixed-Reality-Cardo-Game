using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuggestionsMovement : MonoBehaviour
{
    public Button Suggestion1; // Reference to the first button
    public Button Suggestion2; // Reference to the second button
    public Button Suggestion3; // Reference to the third button
    public string[] SuggestionsTexts; // Array of strings for button texts

    public float moveSpeed = 2f; // Speed at which the buttons move towards each other

    private Vector3 suggestion1TargetPos;
    private Vector3 suggestion2TargetPos;
    private Vector3 suggestion3TargetPos;

    private bool ActivateSwitch;



    void Start()
    {
        // Store the target positions of the buttons (swapping positions)
        suggestion1TargetPos = Suggestion2.transform.position;
        suggestion2TargetPos = Suggestion3.transform.position;
        suggestion3TargetPos = Suggestion1.transform.position;
        //StartCoroutine(SwitchPos());

    }

    void Update()
    {


        if (Input.GetKey("m"))
        {
            ActivateSwitch = true;
        }
        if (Input.GetKey("escape"))
        {
            ActivateSwitch = false;
        }
        if (ActivateSwitch)
        {
            MoveButton(Suggestion1, suggestion1TargetPos);
            MoveButton(Suggestion2, suggestion2TargetPos);
            MoveButton(Suggestion3, suggestion3TargetPos);
        }
    }

    private IEnumerator SwitchPos()
    {
        while (true)
        {

            // Move buttons towards each other's target positions
            MoveButton(Suggestion1, suggestion1TargetPos);
            MoveButton(Suggestion2, suggestion2TargetPos);
            MoveButton(Suggestion3, suggestion3TargetPos);
            Debug.Log("g");
            yield return new WaitForSeconds(3f);

        }
    }

    private void MoveButton(Button button, Vector3 targetPos)
    {
        // Move the button towards the target position using Lerp
        button.transform.position = Vector3.Lerp(button.transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(Suggestion1.transform.position, suggestion1TargetPos) < 0.1f &&
            Vector3.Distance(Suggestion2.transform.position, suggestion2TargetPos) < 0.1f &&
            Vector3.Distance(Suggestion3.transform.position, suggestion3TargetPos) < 0.1f) // Use a small threshold
                                                                                   // Use a small threshold
            {
                ActivateSwitch = false;
                // Store the target positions of the buttons (swapping positions)
                suggestion1TargetPos = Suggestion2.transform.position;
                suggestion2TargetPos = Suggestion3.transform.position;
                suggestion3TargetPos = Suggestion1.transform.position;
                ActivateSwitch = true;

            // Check if the buttonTexts array is not empty
            if (SuggestionsTexts.Length > 0)
            {
                // Randomly select an index from the array
                int randomIndex = Random.Range(0, SuggestionsTexts.Length);
                string selectedText1 = SuggestionsTexts[randomIndex];
                string selectedText2 = SuggestionsTexts[randomIndex + 1];
                string selectedText3 = SuggestionsTexts[randomIndex - 1];

                // Get the TextMeshPro component from the button's child
                TextMeshProUGUI buttonText1 = Suggestion1.GetComponentInChildren<TextMeshProUGUI>();

                // Get the TextMeshPro component from the button's child
                TextMeshProUGUI buttonText2 = Suggestion2.GetComponentInChildren<TextMeshProUGUI>();

                // Get the TextMeshPro component from the button's child
                TextMeshProUGUI buttonText3 = Suggestion3.GetComponentInChildren<TextMeshProUGUI>();

                // Check if the component exists
                if (buttonText1 != null)
                {
                    // Change the text to the selected string
                    buttonText1.text = selectedText1;
                }
                if (buttonText2 != null)
                {
                    // Change the text to the selected string
                    buttonText2.text = selectedText2;
                }
                if (buttonText3 != null)
                {
                    // Change the text to the selected string
                    buttonText3.text = selectedText3;
                }

                else
                {
                    Debug.LogError("TextMeshProUGUI component not found on the button's child.");
                }
            }

        }

     
    }
}
