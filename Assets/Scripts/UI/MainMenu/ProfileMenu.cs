using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class ProfileMenu : MonoBehaviour
{
    [SerializeField] GameObject MainMenuUI;

    Button BackButton;
    Button ClearButton;

    Label PlayerName;
    Button Profile_1;
    Button Profile_2;
    Button Profile_3;
    Button Profile_4;
    int currentProfile = -1;

    VisualElement popupOverlay;
    TextField popupNameField;
    Button popupAccept;
    Button popupCancel;

    int pendingProfileIndex = -1;

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        BackButton = root.Q<Button>("BackButton");
        ClearButton = root.Q<Button>("ClearButton");

        PlayerName = root.Q<Label>("PlayerName");
        Profile_1 = root.Q<Button>("Profile_1");
        Profile_2 = root.Q<Button>("Profile_2");
        Profile_3 = root.Q<Button>("Profile_3");
        Profile_4 = root.Q<Button>("Profile_4");
        currentProfile = StatsManager.LoadLastPlayer();

        CreatePopupUI(root);

        LoadProfileButtons();

        BackButton.RegisterCallback<ClickEvent>(ev =>
        {
            MainMenuUI.SetActive(true);
            this.gameObject.SetActive(false);
        });

        ClearButton.RegisterCallback<ClickEvent>(ev =>
        {
            //StatsManager.ClearAllProfiles();
            if (pendingProfileIndex < 0)
                return;
            StatsManager.ClearProfile(pendingProfileIndex);
            pendingProfileIndex = -1;
            LoadProfileButtons(); // Refresh buttons
        });
    }

    void Update()
    {
        if (popupOverlay.style.display == DisplayStyle.Flex) // popup visible
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame)
            {
                string name = popupNameField.value.Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    CreateProfile();
                }
            }
        }
    }


    // ------------------ POPUP UI ------------------
    void CreatePopupUI(VisualElement root)
    {
        popupOverlay = new VisualElement();
        popupOverlay.style.position = Position.Absolute;
        popupOverlay.style.left = 0;
        popupOverlay.style.right = 0;
        popupOverlay.style.top = 0;
        popupOverlay.style.bottom = 0;
        popupOverlay.style.backgroundColor = new Color(0, 0, 0, 0.7f);
        popupOverlay.style.justifyContent = Justify.Center;
        popupOverlay.style.alignItems = Align.Center;
        popupOverlay.style.display = DisplayStyle.None;

        VisualElement window = new VisualElement();
        window.style.width = 600;          // más grande
        window.style.height = 300;         // más grande
        window.style.paddingTop = 30;
        window.style.paddingBottom = 30;
        window.style.paddingLeft = 30;
        window.style.paddingRight = 30;
        window.style.backgroundColor = new Color(0.12f, 0.12f, 0.2f);
        window.style.borderTopLeftRadius = 12;
        window.style.borderTopRightRadius = 12;
        window.style.borderBottomLeftRadius = 12;
        window.style.borderBottomRightRadius = 12;

        Label title = new Label("Create Profile");
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.fontSize = 28;
        title.style.color = Color.white;
        title.style.marginBottom = 15;
        window.Add(title);

        popupNameField = new TextField("Name:");
        popupNameField.style.fontSize = 22;
        popupNameField.style.marginBottom = 20;
        window.Add(popupNameField);

        popupAccept = new Button(() => CreateProfile());
        popupAccept.text = "Accept";
        popupAccept.style.fontSize = 22;
        popupAccept.style.marginBottom = 10;
        window.Add(popupAccept);

        popupCancel = new Button(() => ClosePopup());
        popupCancel.text = "Cancel";
        popupCancel.style.fontSize = 22;
        window.Add(popupCancel);

        popupOverlay.Add(window);
        root.Add(popupOverlay);
    }

    // ------------------ PROFILE BUTTONS ------------------
    void LoadProfileButtons()
    {
        string playerName = StatsManager.LoadPlayerName(currentProfile);
        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerName.text = playerName;            
        } else PlayerName.text = "Select a profile";

        SetupProfileButton(Profile_1, 0);
        SetupProfileButton(Profile_2, 1);
        SetupProfileButton(Profile_3, 2);
        SetupProfileButton(Profile_4, 3);
    }

    void SetupProfileButton(Button button, int index)
    {
        bool exists = ProfileExists(index);

        // Unregister to prevent stacking callbacks on reloads
        button.UnregisterCallback<ClickEvent, int>(OnProfileButtonClick);

        button.text = exists ? StatsManager.LoadPlayerName(index) : "Create";

        button.RegisterCallback<ClickEvent, int>(OnProfileButtonClick, index);
    }

    private void OnProfileButtonClick(ClickEvent evt, int index)
    {
        // Set the pending index so we know which profile is selected for deletion.
        pendingProfileIndex = index;

        if (ProfileExists(index))
        {
            LoadProfile(index);
        }
        else
        {
            OpenPopup();
        }
    }

    // ------------------ POPUP LOGIC ------------------
    void OpenPopup()
    {
        popupNameField.value = "";
        popupOverlay.style.display = DisplayStyle.Flex;

        popupNameField.Focus();
    }

    void ClosePopup()
    {
        popupOverlay.style.display = DisplayStyle.None;
        pendingProfileIndex = -1;
    }

    void CreateProfile()
    {
        if (pendingProfileIndex < 0)
            return;

        string name = popupNameField.value.Trim();
        if (string.IsNullOrEmpty(name))
            return;

        // guardar nombre de perfil
        StatsManager.SavePlayerName(pendingProfileIndex, name);

        LoadProfile(pendingProfileIndex);

        ClosePopup();
    }

    // ------------------ LOAD PROFILE INTO MAIN MENU ------------------
    void LoadProfile(int index)
    {
        StatsManager.SaveLastPlayer(index);

        MainMenuUI.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // ------------------ HELPERS ------------------
    bool ProfileExists(int index)
    {
        return StatsManager.ProfileNameExists(index);
    }

}
