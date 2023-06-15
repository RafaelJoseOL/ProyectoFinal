using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour {

    public GameObject signUpPanel;
    public GameObject logInPanel;
    public GameObject optionsPanel;

    TMP_InputField mailField;
    TMP_InputField userNameField;
    TMP_InputField passField;
    TMP_InputField passField2;

    DBController dbController;

    private void Start () {
        dbController = GameObject.FindGameObjectWithTag("DBController").GetComponent<DBController>();
        dbController.GetData();
    }

    public void SignUp () {
        signUpPanel.gameObject.SetActive(true);
        mailField = GameObject.FindGameObjectWithTag("MailIF").GetComponent<TMP_InputField>();
        userNameField = GameObject.FindGameObjectWithTag("UserNameIF").GetComponent<TMP_InputField>();
        passField = GameObject.FindGameObjectWithTag("PassIF").GetComponent<TMP_InputField>();
        passField2 = GameObject.FindGameObjectWithTag("PassIF2").GetComponent<TMP_InputField>();
        mailField.text = "";
        userNameField.text = "";
        passField.text = "";
        passField2.text = "";
    }

    public void MessagePanel (bool open) {
        if (open) {
            dbController.messagePanel.SetActive(true);
        } else {
            dbController.messagePanel.SetActive(false);
        }
    }

    public void SignUpConfirm () {
        string email = mailField.text;
        string username = userNameField.text;
        string password = passField.text;
        string passwordConf = passField2.text;

        string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        bool isMatch = Regex.IsMatch(email, pattern);
        if (!isMatch) {
            dbController.messageText.GetComponent<TextMeshProUGUI>().text = "Introduce un correo válido.";
            MessagePanel(true);
        } else {
            if (username == "") {
                dbController.messageText.GetComponent<TextMeshProUGUI>().text = "Introduce un nombre de usuario.";
                MessagePanel(true);
            } else {
                if (password == "") {
                    dbController.messageText.GetComponent<TextMeshProUGUI>().text = "Introduce una contraseña.";
                    MessagePanel(true);
                } else {
                    if (password.Length < 6) {
                        dbController.messageText.GetComponent<TextMeshProUGUI>().text = "La contraseña debe tener al menos 6 caracteres.";
                        MessagePanel(true);
                    } else {
                        if (password != passwordConf) {
                            dbController.messageText.GetComponent<TextMeshProUGUI>().text = "Las contraseñas no coinciden.";
                            MessagePanel(true);
                        } else {
                            dbController.SignUp(email, password, username);
                        }
                    }
                }
            }
        }
    }

    public void LogIn () {
        logInPanel.gameObject.SetActive(true);
        mailField = GameObject.FindGameObjectWithTag("MailIF").GetComponent<TMP_InputField>();
        passField = GameObject.FindGameObjectWithTag("PassIF").GetComponent<TMP_InputField>();
        mailField.text = "";
        passField.text = "";
    }

    public void LogInConfirm () {
        string email = mailField.text;
        string password = passField.text;
        dbController.LogIn(email, password);
    }

    public void ClosePanel (GameObject panelToClose) {
        panelToClose.SetActive(false);
    }

    public void Options () {
        Debug.Log("Opciones");
    }

    public void Exit () {
        Application.Quit();
    }

    public void Testing () {
        dbController.LogIn("raff@gmail.com", "password");
    }
}
