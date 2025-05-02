using UnityEngine;

public class GlobalKeyCodeManager : MonoBehaviour
{
    public static GlobalKeyCodeManager Instance;

    [Header("Gameplay Key Bindings")]
    [Tooltip("Key used to fire weapons (default is left mouse button)")]
    public KeyCode fireKey = KeyCode.Mouse0;

    [Tooltip("Key used to activate powerups (default is Alpha1)")]
    public KeyCode powerupKey = KeyCode.Alpha1;

    [Tooltip("Key used for moving up (default is W)")]
    public KeyCode moveUpKey = KeyCode.W;

    [Tooltip("Key used for moving down (default is S)")]
    public KeyCode moveDownKey = KeyCode.S;

    [Tooltip("Key used for moving left (default is A)")]
    public KeyCode moveLeftKey = KeyCode.A;

    [Tooltip("Key used for moving right (default is D)")]
    public KeyCode moveRightKey = KeyCode.D;

    [Tooltip("Key used for running (default is Left Shift)")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Tooltip("Key used to drop mines (default is Space)")]
    public KeyCode dropMineKey = KeyCode.Space;

    [Tooltip("Key used to Drag enemies and boxes (default is RMB)")]
    public KeyCode dragKey = KeyCode.Mouse1;

    [Tooltip("Key used to enter build / combat mode(default is B)")]
    public KeyCode modeSwitch = KeyCode.B;

    [Tooltip("Key used to rotate walls(default is R)")]
    public KeyCode rotateBuilding = KeyCode.R;

    [Tooltip("Key used to place walls (default is E)")]
    public KeyCode placeBuilding = KeyCode.E;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Uncomment if you want this object to persist between scenes:
            // DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Save & Load

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FireKey", (int)fireKey);
        PlayerPrefs.SetInt("PowerupKey", (int)powerupKey);
        PlayerPrefs.SetInt("MoveUpKey", (int)moveUpKey);
        PlayerPrefs.SetInt("MoveDownKey", (int)moveDownKey);
        PlayerPrefs.SetInt("MoveLeftKey", (int)moveLeftKey);
        PlayerPrefs.SetInt("MoveRightKey", (int)moveRightKey);
        PlayerPrefs.SetInt("SprintKey", (int)sprintKey);
        PlayerPrefs.SetInt("DropMineKey", (int)dropMineKey);
        PlayerPrefs.SetInt("DragKey", (int)dragKey);
        PlayerPrefs.SetInt("modeKey", (int)modeSwitch);
        PlayerPrefs.SetInt("rotateKey", (int)rotateBuilding);
        PlayerPrefs.SetInt("placeKey", (int)placeBuilding);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.HasKey("FireKey"))
            fireKey = (KeyCode)PlayerPrefs.GetInt("FireKey");
        if (PlayerPrefs.HasKey("PowerupKey"))
            powerupKey = (KeyCode)PlayerPrefs.GetInt("PowerupKey");
        if (PlayerPrefs.HasKey("MoveUpKey"))
            moveUpKey = (KeyCode)PlayerPrefs.GetInt("MoveUpKey");
        if (PlayerPrefs.HasKey("MoveDownKey"))
            moveDownKey = (KeyCode)PlayerPrefs.GetInt("MoveDownKey");
        if (PlayerPrefs.HasKey("MoveLeftKey"))
            moveLeftKey = (KeyCode)PlayerPrefs.GetInt("MoveLeftKey");
        if (PlayerPrefs.HasKey("MoveRightKey"))
            moveRightKey = (KeyCode)PlayerPrefs.GetInt("MoveRightKey");
        if (PlayerPrefs.HasKey("SprintKey"))
            sprintKey = (KeyCode)PlayerPrefs.GetInt("SprintKey");
        if (PlayerPrefs.HasKey("DropMineKey"))
            dropMineKey = (KeyCode)PlayerPrefs.GetInt("DropMineKey");
        if (PlayerPrefs.HasKey("DragKey"))
            dragKey = (KeyCode)PlayerPrefs.GetInt("DragKey");
        if (PlayerPrefs.HasKey("modeKey"))
            modeSwitch = (KeyCode)PlayerPrefs.GetInt("modeKey");
        if (PlayerPrefs.HasKey("rotateKey"))
            rotateBuilding = (KeyCode)PlayerPrefs.GetInt("rotateKey");
        if (PlayerPrefs.HasKey("placeKey"))
            placeBuilding = (KeyCode)PlayerPrefs.GetInt("placeKey");
    }

    #endregion
}
