using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using Firebase;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    [Header("Firebase")]
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference databaseReference;
    [Space(5f)]

    [Header("Login UI")]
    public InputField loginEmail_IF;
    public InputField loginPassword_IF;
    public Text loginOutput_T;
    [Space(5f)]

    [Header("Register UI")]
    public InputField registerUserName_IF;
    public InputField registerEmail_IF;
    public InputField registerPassword_IF;
    public InputField registerConfirmPassword_IF;
    public Text registerOutput_T;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        
    }

    private void Start()
    {
        StartCoroutine(CheckAndFixDependencing());
    }

    private IEnumerator CheckAndFixDependencing()
    {
        var checkAndFixDependencingTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependencingTask.IsCompleted);

        var dependencyResult = checkAndFixDependencingTask.Result;

        if (dependencyResult == DependencyStatus.Available)
        {
            InitiallzeFirebase();
        }
        else
        {
            Debug.LogError($"無法解析所有 Firebase 依賴項: {dependencyResult}");
        }
    }

    private void InitiallzeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(CheckAutoLogin());
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();

        if (user != null)
        {
            var reloadUserTask = user.ReloadAsync();

            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);

            AutoLogin();
        }
        else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }

    public void AutoLogin()
    {
        if (user != null)
        {
            if (user.IsEmailVerified)
            {
                GameManager.instance.ChangeScene(1);
            }
            else
            {
                StartCoroutine(SentVerificationEmail());
            }
        }
        else
        {
            AuthUIManager.instance.LoginScreen();
        }
    }   

    private void AuthStateChanged(object sender,System.EventArgs eventArgs)
    {
        if(auth.CurrentUser != user)
        {
            bool signIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signIn && user != null)
            {
                Debug.Log("Signed Out");
            }

            user = auth.CurrentUser;

            if (signIn)
            {
                Debug.Log($"Signed In: {user.DisplayName}");
            }
        }
    }

    public void ClearOutputs()
    {
        loginOutput_T.text = "";
        registerOutput_T.text = "";
    }

    public void LoginButton()
    {
        StartCoroutine(LoginLogic(loginEmail_IF.text, loginPassword_IF.text));
    }

    public void RegisterButton()
    {
        StartCoroutine(RegisterLogic(registerUserName_IF.text, registerEmail_IF.text, registerPassword_IF.text, registerConfirmPassword_IF.text));
    }

    private IEnumerator LoginLogic(string _email,string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        var loginTask = auth.SignInAndRetrieveDataWithCredentialAsync(credential);

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "未知錯誤，請重試";

            switch (error)
            {
                case AuthError.MissingEmail:
                    output = "請輸入您的電子郵件";
                    break;
                case AuthError.MissingPassword:
                    output = "請輸入您的密碼";
                    break;
                case AuthError.InvalidEmail:
                    output = "請輸入合理的電子郵件";
                    break;
                case AuthError.WrongPassword:
                    output = "密碼錯誤";
                    break;
                case AuthError.UserNotFound:
                    output = "未找到用戶";
                    break;
            }

            loginOutput_T.text = output;
        }
        else
        {
            if (user.IsEmailVerified)
            {
                yield return new WaitForSeconds(1);
                GameManager.instance.ChangeScene(1);
            }
            else
            {
                StartCoroutine(SentVerificationEmail());
            }
        }
    }

    public IEnumerator RegisterLogic(string _userName, string _email, string _password, string _confirmPassword)
    {
        if (_userName == "")
        {
            registerOutput_T.text = "請填入一個用戶名";
        }
        else if(_password != _confirmPassword)
        {
            registerOutput_T.text = "密碼不匹配，請重試";
        }
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "未知錯誤，請重試";

                switch (error)
                {
                    case AuthError.InvalidEmail:
                        output = "請輸入合理的電子郵件";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "電子郵件已被使用";
                        break;
                    case AuthError.WeakPassword:
                        output = "密碼強度低，請重試";
                        break;
                    case AuthError.MissingEmail:
                        output = "請輸入您的電子郵件";
                        break;
                    case AuthError.MissingPassword:
                        output = "請輸入您的密碼";
                        break;
                }

                registerOutput_T.text = output;
            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = _userName,

                    //Give Profite Default Photos
                    PhotoUrl = new System.Uri("https://pbs.twimg.com/media/EFKdt0bWsAIfcj9.jpg"),
                };

                var defaultItUserTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultItUserTask.IsCompleted);

                if (registerTask.Exception != null)
                {
                    user.DeleteAsync();

                    FirebaseException firebaseException = (FirebaseException)defaultItUserTask.Exception.GetBaseException();
                    AuthError error = (AuthError)firebaseException.ErrorCode;
                    string output = "未知錯誤，請重試";

                    switch (error)
                    {
                        case AuthError.Cancelled:
                            output = "上傳用戶已取消";
                            break;
                        case AuthError.SessionExpired:
                            output = "會話已過期，請重試";
                            break;                       
                    }

                    registerOutput_T.text = output;
                }
                else
                {
                    Debug.Log($"Firebase User Created Successfully: {user.DisplayName} ({user.UserId})");
                    StartCoroutine(SentVerificationEmail());
                }
            }
        }
    }

    private IEnumerator SentVerificationEmail()
    {
        if(user != null)
        {
            var emailTask = user.SendEmailVerificationAsync();

            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "未知錯誤，請重試";

                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "驗證被取消";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "請輸入合理的電子郵件";
                        break;
                    case AuthError.TooManyRequests:
                        output = "請求過多";
                        break;
                }

                AuthUIManager.instance.AWaitVerifcation(false, user.Email, output);
            }
            else
            {
                AuthUIManager.instance.AWaitVerifcation(true, user.Email, null);
                Debug.Log("Email Sent Successfully");
            }
        }
    }

    public void UpdateProfilePicture(string _newPfpURL)
    {
        StartCoroutine(UpdateProfilePictureLogic(_newPfpURL));
    }

    private IEnumerator UpdateProfilePictureLogic(string _newPfpURL)
    {
        if(user != null)
        {
            UserProfile profile = new UserProfile();

            try
            {
                UserProfile _profile = new UserProfile
                {
                    PhotoUrl = new System.Uri(_newPfpURL),
                };

                profile = _profile;
            }
            catch
            {
                LobbyManager.instance.Output("獲取圖片時出錯，請確保您的鏈接有效");
                yield break;
            }

            var pfpTask = user.UpdateUserProfileAsync(profile);

            yield return new WaitUntil(predicate: () => pfpTask.IsCompleted);

            if (pfpTask.Exception != null)
            {
                Debug.LogError($"Updating Profile Picture was unsuccessful: {pfpTask.Exception}");
            }
            else
            {    
                LobbyManager.instance.ChangePfpSuccess();
                Debug.Log("Profile Image Updated Successfully");
            }
        }
    }

    public void SignOut()
    {
        auth.SignOut();
        GameManager.instance.ChangeScene(0);
    }

    ///// DATA /////

    private IEnumerator UpdateUserNameDatabase(string _userName)
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).Child("userName").SetValueAsync(_userName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogError($"Update User Name data have some problem: {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateLevel(string _level)
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).Child("level").SetValueAsync(_level);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError($"Update Xp data have some problem: {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateXp(string _xp)
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).Child("xp").SetValueAsync(_xp);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError($"Update Xp data have some problem: {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateCoin(string _coin)
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).Child("coin").SetValueAsync(_coin);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError($"Update Coin data have some problem: {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateDiamond(string _diamond)
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).Child("diamond").SetValueAsync(_diamond);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogError($"Update Diamond data have some problem: {DBTask.Exception}");
        }
    }

    public IEnumerator LoadUserData()
    {
        var DBTask = databaseReference.Child("Users").Child(user.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if(DBTask.Exception != null)
        {
            Debug.LogError($"Load User Data have some problem: {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            DatabaseManager.instance.DisplayDataOnUI(0, 0, 1, 0);
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            int coin = int.Parse(snapshot.Child("coin").Value.ToString());
            int diamond = int.Parse(snapshot.Child("diamond").Value.ToString());
            int level = int.Parse(snapshot.Child("level").Value.ToString());
            int xp = int.Parse(snapshot.Child("xp").Value.ToString());

            DatabaseManager.instance.DisplayDataOnUI(coin, diamond,level, xp);
        }
    }
}
