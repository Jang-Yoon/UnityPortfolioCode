using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputFieldLoginId;
    [SerializeField] private TMP_InputField inputFieldLoginPassword;
    [SerializeField] private TMP_InputField inputFieldSignUpId;
    [SerializeField] private TMP_InputField inputFieldSignUpPassword;
    [SerializeField] private TMP_InputField inputFieldCheckSignUpPassword;
    [SerializeField] private GameObject cautionSignUpId;
    [SerializeField] private GameObject cautionSignUpPassword;
    [SerializeField] private GameObject cautionCheckSignUpPassword;
    [SerializeField] private Button buttonLogin;
    [SerializeField] private Button buttonSignUp;

    private void Start()
    {
        buttonSignUp.onClick.AddListener(CheckSignUp);
        buttonLogin.onClick.AddListener(CheckLogin);
    }

    private void CheckLogin()
    {
        BackendLogin.Instance.CustomLogin(inputFieldLoginId.text, inputFieldLoginPassword.text);
        UIManager.Instance.ClearInputField(inputFieldLoginId, inputFieldLoginPassword);
    }
    private void CheckSignUp()
    {
        if (inputFieldSignUpId.text == "" || inputFieldSignUpId.text.Contains(" "))
        {
            cautionSignUpId.SetActive(true);
            return;
        }
        else
        {
            cautionSignUpId.SetActive(false);
        }
        if(inputFieldSignUpPassword.text == "" || inputFieldSignUpPassword.text.Contains(" "))
        {
            cautionSignUpPassword.SetActive(true);
            return;
        }
        else
        {
            cautionSignUpPassword.SetActive(false);
        }
        if (inputFieldSignUpPassword.text != inputFieldCheckSignUpPassword.text)
        {
            cautionCheckSignUpPassword.SetActive(true);
            return;
        }
        else
        {
            cautionCheckSignUpPassword.SetActive(false);
            BackendLogin.Instance.CustomSignUp(inputFieldSignUpId.text, inputFieldSignUpPassword.text);
            UIManager.Instance.ClearInputField(inputFieldSignUpId, inputFieldSignUpPassword);
        }
    }
}
