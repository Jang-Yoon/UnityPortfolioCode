using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConvenienceManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] inputFieldsLogin;
    [SerializeField] private TMP_InputField[] inputFieldSignup;
    [SerializeField] private Button buttonStart;
    private bool isFocused;


    private void Update()
    {
        if (inputFieldsLogin[0].isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            inputFieldsLogin[1].ActivateInputField();
        }
        else if (inputFieldsLogin[1].text != ""  && Input.GetKeyDown(KeyCode.Return))
        {
            buttonStart.onClick.Invoke();
        }

        if(inputFieldSignup[0].isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            inputFieldSignup[1].ActivateInputField();
            isFocused = true;
        }
        else if(isFocused && Input.GetKeyDown(KeyCode.Tab))
        {
            isFocused = false;
            inputFieldSignup[2].ActivateInputField();
        }
    }
}
