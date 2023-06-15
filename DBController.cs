using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;

public class DBController : MonoBehaviour {
    public string userId;
    public string username;
    public bool logged = false;
    public bool stop = false;
    bool unlockedLevel1;
    bool unlockedLevel2;
    bool unlockedLevel3;
    public GameObject black;
    public GameObject messagePanel;
    public GameObject messageText;

    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private static DBController instance;

    void Awake () {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GetData () {
        db = FirebaseFirestore.DefaultInstance;
        if (SceneManager.GetActiveScene().name == "MainMenu") {
            black = GameObject.FindGameObjectWithTag("BlackScreen");
            messagePanel = GameObject.FindGameObjectWithTag("MessagePanel");
            messageText = GameObject.FindGameObjectWithTag("MessageText");
            black.SetActive(false);
            messagePanel.SetActive(false);
        }
    }

    private void Update () {
        if (!stop && SceneManager.GetActiveScene().name == "MainMenu") {
            LoadMap();
        }
    }

    public void SignUp (string email, string password, string username) {
        auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                //messagePanel.SetActive(true);
                //messageText.GetComponent<TextMeshProUGUI>().text = "";
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);                
                return;
            }

            FirebaseUser user = task.Result;
            this.userId = user.UserId;
            this.username = username;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            CreateUserInfo(username);
            logged = true;
            LoadData(userId);
        });

        ErrorMsg("Error: Correo ya en uso.");
    }

    public void LogIn (string email, string password) {
        auth = FirebaseAuth.DefaultInstance;
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);                
                return;
            }

            FirebaseUser user = task.Result;
            userId = user.UserId;
            logged = true;
            LoadData(userId);
        });

        ErrorMsg("Error: Usuario o contraseña incorrectos.");
    }

    public void LoadData (string userId) {
        DocumentReference userRef = db.Collection("users").Document(userId);
        userRef.GetSnapshotAsync().ContinueWith(snapshotTask => {
            if (snapshotTask.IsFaulted) {
                Debug.LogError("Failed to get user document: " + snapshotTask.Exception);
                return;
            }

            DocumentSnapshot snapshot = snapshotTask.Result;
            if (snapshot.Exists) {
                var userData = snapshot.ToDictionary();
                if (userData.TryGetValue("username", out object usernameValue)) {
                    this.username = usernameValue.ToString();

                    if (userData.TryGetValue("level1", out object unlockedLevel1)) {
                        this.unlockedLevel1 = (bool)unlockedLevel1;
                    }

                    if (userData.TryGetValue("level2", out object unlockedLevel2)) {
                        this.unlockedLevel2 = (bool)unlockedLevel2;
                    }

                    if (userData.TryGetValue("level3", out object unlockedLevel3)) {
                        this.unlockedLevel3 = (bool)unlockedLevel3;
                    }

                    logged = true;
                } else {
                    Debug.LogWarning("Username field not found in the document.");
                }
            } else {
                Debug.LogWarning("User document does not exist.");
            }
        });
    }

    public async void LoadMap () {
        if (logged) {
            stop = true;
            black.SetActive(true);
            await Task.Delay(1000);
            SceneManager.LoadScene("Map");
        }
    }

    public async void ErrorMsg (string error) {
        await Task.Delay(2500);
        if (!logged) {
            messageText.GetComponent<TextMeshProUGUI>().text = error;
            messagePanel.SetActive(true);
        }
    }

    public void SaveData (int level) {
        DocumentReference userRef = db.Collection("users").Document(userId);

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "level" + level, true }
        };

        userRef.UpdateAsync(updates)
            .ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("Actualización del documento del usuario cancelada.");
                } else if (task.IsFaulted) {
                    Debug.LogError("Error al actualizar el documento del usuario: " + task.Exception);
                } else {
                    Debug.Log("Documento del usuario actualizado correctamente.");
                }
            });
    }

    public void CreateUserInfo (string username) {
        if (auth.CurrentUser == null) {
            Debug.LogWarning("No user is currently signed in.");
            return;
        }

        string userId = auth.CurrentUser.UserId;
        DocumentReference userRef = db.Collection("users").Document(userId);

        Dictionary<string, object> user = new Dictionary<string, object>
    {
        { "username", username },
        { "level1", true },
        { "level2", false },
        { "level3", false }
    };
        userRef.SetAsync(user)
        .ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("Creación del documento del usuario cancelada.");
            } else if (task.IsFaulted) {
                Debug.LogError("Error al crear el documento del usuario: " 
                    + task.Exception);
            } else {
                Debug.Log("Documento del usuario creado correctamente.");
            }
        });
    }

    public bool Level1 {
        get { return unlockedLevel1; }
        set { this.unlockedLevel1 = value; }
    }

    public bool Level2 {
        get { return unlockedLevel2; }
        set { this.unlockedLevel2 = value; }
    }

    public bool Level3 {
        get { return unlockedLevel3; }
        set { this.unlockedLevel3 = value; }
    }
}
