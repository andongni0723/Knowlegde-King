using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("UI Panel")]
    public GameObject profileUI;
    public GameObject changeProfileUI;
    //public GameObject changeEmailUI;
    //public GameObject changePasswordUI;
    //public GameObject reverifyUI;
    //public GameObject resetPasswordConfirmUI;
    public GameObject actionSuccessPanelUI;
    //public GameObject deletUserConifmUI;
    [Space(5)]

    [Header("Basic Info UI")]
    public Text userName_T;
    public Text settingUserName_T;
    public Text email_T;
    public string token;
    [Space(5)]

    [Header("Profile Picture UI")]
    public Image profilePicture;
    public Image settingProfilePicture;
    public InputField profilePictureLink_IF;
    public Text output_T;
    [Space(5)]

    //[Header("Change Email UI")]
    //public InputField changeEmail_IF;
    //[Space(5)]

    //[Header("Change Password UI")]
    //public InputField changePassword_IF;
    //public InputField changePasswordConfirm_IF;
    //[Space(5)]

    //[Header("Reverify UI")]
    //public InputField reverifyEmail_IF;
    //public InputField reverifyPassword_IF;
    //[Space(5)]

    [Header("Action Success Panel UI")]
    public Text actionSuccess_T;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        print(FirebaseManager.instance.user);
        if(FirebaseManager.instance.user != null)
        {
            LoadProfile();
        }
        
    }

    private void LoadProfile()
    {
        if (FirebaseManager.instance.user != null)
        {
            // Set Varibles
            System.Uri photoUrl = FirebaseManager.instance.user.PhotoUrl;
            string name = FirebaseManager.instance.user.DisplayName;
            string email = FirebaseManager.instance.user.Email;


            // Set UI
            StartCoroutine(LoadImage(photoUrl.ToString()));
            userName_T.text = name;
            settingUserName_T.text = name;
            email_T.text = email;
        }
    }

    private IEnumerator LoadImage(string _photoUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(_photoUrl);

        yield return request.SendWebRequest();

        if (request.error != null)
        {
            string output = "未知錯誤！ 再試一次！";

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                output = "圖片類型不支持！ 請嘗試另一張圖片";

            }

            Output(output);
        }
        else
        {
            Texture2D image = ((DownloadHandlerTexture)request.downloadHandler).texture;

            profilePicture.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
            settingProfilePicture.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        }
    }

    public void Output(string _output)
    {
        output_T.text = _output;
    }

    public void ClearUI()
    {
        output_T.text = "";
        //profileUI.SetActive(false);
        changeProfileUI.SetActive(false);
        // Change Email
        // Change Password
        // Reverify
        // Reset Password
        actionSuccessPanelUI.SetActive(false);
        // Delet User
    }

    public void ProfileUI()
    {
        ClearUI();
        profileUI.SetActive(true);
        LoadProfile();
    }

    public void ChangePfpUI()
    {
        ClearUI();
        Output("");
        changeProfileUI.SetActive(true);
        
    }

    public void ChangePfpSuccess()
    {
        ClearUI();
        LoadProfile();
        actionSuccessPanelUI.SetActive(true);
        actionSuccess_T.text = "頭像更改成功";
        
    }

    public void FunctionNotOpen()
    {
        actionSuccessPanelUI.SetActive(true);
        actionSuccess_T.text = "功能尚未開放";
    }

    public void SubmitProfileImageButton()
    {
        FirebaseManager.instance.UpdateProfilePicture(profilePictureLink_IF.text);
    }

    public void SignOutButton()
    {
        FirebaseManager.instance.SignOut();
    }
}
