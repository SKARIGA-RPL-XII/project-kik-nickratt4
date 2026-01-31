using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    [Header("LOGIN UI")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("REGISTER UI")]
    public TMP_InputField regUsername;
    public TMP_InputField regEmail;
    public TMP_InputField regPassword;

    [Header("NOTIFICATION UI")]
    public GameObject notifBox;
    public TMP_Text infoText;
    public Image notifBg;

    [Header("PANELS")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("BUTTON PINDAH PANEL")]
    public GameObject btnToRegister;
    public GameObject btnToLogin;

    [Header("CONFIG")]
    [SerializeField]
    private string baseUrl = "http://127.0.0.1/Dice_of_doom_DB/";

    void Start()
    {
        notifBox.SetActive(false);

        if (PlayerPrefs.HasKey("player_id"))
        {
            loginPanel.SetActive(false);
            registerPanel.SetActive(false);
            btnToRegister.SetActive(false);
            btnToLogin.SetActive(false);
        }
        else
        {
            SwitchToLogin();
        }
    }

    // login
    public void OnLoginClick()
    {
        if (string.IsNullOrEmpty(loginEmail.text) ||
            string.IsNullOrEmpty(loginPassword.text))
        {
            ShowNotif("Email dan password wajib diisi", false);
            return;
        }

        StartCoroutine(Login());
    }
    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", loginEmail.text.Trim());
        form.AddField("password", loginPassword.text.Trim());

        UnityWebRequest req = UnityWebRequest.Post(baseUrl + "login.php", form);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            LoginResponse res =
                JsonUtility.FromJson<LoginResponse>(req.downloadHandler.text);

            if (res.status == "success")
            {
                PlayerPrefs.SetInt("player_id", res.player_id);
                PlayerPrefs.SetString("username", res.username);
                PlayerPrefs.Save();

                ShowNotif("Login berhasil", true);

                loginPanel.SetActive(false);
                registerPanel.SetActive(false);
                btnToRegister.SetActive(false);
                btnToLogin.SetActive(false);
            }
            else
            {
                ShowNotif("Email atau password salah", false);
            }
        }
        else
        {
            ShowNotif("Koneksi ke server gagal", false);
        }
    }

    // regis
    public void OnRegisterClick()
    {
        if (string.IsNullOrEmpty(regUsername.text) ||
            string.IsNullOrEmpty(regEmail.text) ||
            string.IsNullOrEmpty(regPassword.text))
        {
            ShowNotif("Semua field wajib diisi", false);
            return;
        }

        StartCoroutine(Register());
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", regUsername.text.Trim());
        form.AddField("email", regEmail.text.Trim());
        form.AddField("password", regPassword.text.Trim());

        UnityWebRequest req = UnityWebRequest.Post(baseUrl + "register.php", form);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            SimpleResponse res =
                JsonUtility.FromJson<SimpleResponse>(req.downloadHandler.text);

            if (res.status == "success")
            {
                ShowNotif("Register berhasil, silakan login", true);
                SwitchToLogin();
            }
            else
            {
                ShowNotif("Username atau email sudah digunakan", false);
            }
        }
        else
        {
            ShowNotif("Koneksi ke server gagal", false);
        }
    }

    void ShowNotif(string message, bool success)
    {
        notifBox.SetActive(true);
        infoText.text = message;

        notifBg.color = success
            ? new Color(0.2f, 0.6f, 0.2f, 0.9f)
            : new Color(0.6f, 0.2f, 0.2f, 0.9f);

        StopAllCoroutines();
        StartCoroutine(HideNotif());
    }

    IEnumerator HideNotif()
    {
        yield return new WaitForSeconds(2.5f);
        notifBox.SetActive(false);
    }

    public void SwitchToRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        btnToRegister.SetActive(false);
        btnToLogin.SetActive(true);
    }

    public void SwitchToLogin()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        btnToLogin.SetActive(false);
        btnToRegister.SetActive(true);
    }

    public void Logout()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("SampleScene");
    }
}

[System.Serializable]
public class LoginResponse
{
    public string status;
    public int player_id;
    public string username;

}

[System.Serializable]
public class SimpleResponse
{
    public string status;
}
