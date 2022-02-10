using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    public static AuthUIManager instance;

    [Header("UI GameObject")]
    public GameObject checkingForAccountUI;
    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject verifyEmailUI;
    public Text verifyEmail_T;

    private void Awake()
    {
        instance = this;

        LoginScreen();
    }

    private void ClearUI()
    {
        FirebaseManager.instance.ClearOutputs();
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        verifyEmailUI.SetActive(false);
        checkingForAccountUI.SetActive(false);
    }

    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }

    public void AWaitVerifcation(bool _emailSent, string _email, string _output)
    {
        ClearUI();
        verifyEmailUI.SetActive(true);

        if (_emailSent)
        {
            verifyEmail_T.text = $"已寄出郵件:\n請前往信箱 { _email} 驗證";
            verifyEmail_T.color = Color.green;
        }
        else
        {
            verifyEmail_T.text = $"寄出郵件失敗: {_output}\n 請前往信箱{_email}驗證";
            verifyEmail_T.color = Color.red;
        }
    }
}
