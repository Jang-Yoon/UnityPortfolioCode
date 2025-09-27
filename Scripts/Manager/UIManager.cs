using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField,Header("로그인 실패시 띄울 텍스트")] private TextMeshProUGUI tmpLoginCaution;
    [SerializeField,Header("로그인 백그라운드")] private GameObject LoginBackground;
    [SerializeField,Header("회원가입 백그라운드")] private GameObject SignUpBackground;
    [SerializeField,Header("회원가입 성공 백그라운드")] private GameObject SignUpSuccessBackground;
    private bool isSetActiveSignUp;
    private bool isSetActiveLogin;

    private void Awake()
    {
        Instance = this;
    }

    public void QuitGame()
    {
        GameManager.Instance.GameExit();
    }

    public void FailLogin()
    {
        tmpLoginCaution.gameObject.SetActive(true);
    }

    public void OpenAndCloseLogin()
    {
        isSetActiveLogin = !isSetActiveLogin;
        LoginBackground.SetActive(isSetActiveLogin);
        tmpLoginCaution.gameObject.SetActive(false);
    }

    public void OpenAndCloseSignUp()
    {
        isSetActiveSignUp = !isSetActiveSignUp;
        SignUpBackground.SetActive(isSetActiveSignUp);
        tmpLoginCaution.gameObject.SetActive(false);
    }

    public void SuccessSignUp()
    {
        SignUpSuccessBackground.SetActive(true);
        tmpLoginCaution.gameObject.SetActive(false);
    }

    public void SignUpAccept()
    {
        SignUpSuccessBackground.SetActive(false);
        isSetActiveSignUp = false;
        SignUpBackground.SetActive(isSetActiveSignUp);
        tmpLoginCaution.gameObject.SetActive(false);
    }
    public void ClearInputField(TMP_InputField id, TMP_InputField pw)
    {
        id.text = "";
        pw.text = "";
    }
}
