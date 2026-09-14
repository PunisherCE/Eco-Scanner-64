using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private ScrollView StatsView;
    Button StartGame;
    Button Profiles;
    Button Configuration;
    Button Credits;
    Label PlayerName;

    [SerializeField] GameObject ProfileUI;
    [SerializeField] GameObject ConfigurationUI;
    [SerializeField] GameObject CreditsUI;

    int currentProfile = 0;



    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        StartGame = root.Q<Button>("StartGame");
        Profiles = root.Q<Button>("Profiles");
        Configuration = root.Q<Button>("Configuration");
        Credits = root.Q<Button>("Credits");

        PlayerName = root.Q<Label>("PlayerName");
        StatsView = root.Q<ScrollView>("StatsView");

        // Load last used profile
        currentProfile = StatsManager.LoadLastPlayer();

        LoadStatsIntoView();

        StartGame.RegisterCallback<ClickEvent>(ev =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SceneMain");
        });

        Profiles.RegisterCallback<ClickEvent>(ev =>
        {
            ProfileUI.SetActive(true);
            this.gameObject.SetActive(false);
        });
        // Configuration.RegisterCallback<ClickEvent>(ev =>
        // {
        //     ConfigurationUI.SetActive(true);
        //     this.gameObject.SetActive(false);
        // });

        // Credits.RegisterCallback<ClickEvent>(ev =>
        // {
        //     CreditsUI.SetActive(true);
        //     this.gameObject.SetActive(false);
        // });
    }


    // ---------------------------------------------------------
    // Loads stats from PlayerPrefs JSON and populates StatsView
    // ---------------------------------------------------------
    void LoadStatsIntoView()
    {
        StatsView.Clear();

        // nombre del jugador
        string playerName = StatsManager.LoadPlayerName(currentProfile);
        PlayerName.text = playerName;

        // A profile is valid if it has a name. It doesn't need stats to start the game.
        if (string.IsNullOrEmpty(playerName))
        {
            StartGame.SetEnabled(false);
            return;
        }

        StatCollection stats = StatsManager.LoadStats(currentProfile);
        StartGame.SetEnabled(true);

        foreach (var entry in stats.stats)
        {
            // no mostrar la entrada "PlayerName" en el ScrollView
            if (entry.name == "PlayerName")
                continue;

            AddStat(entry.name, entry.score);
        }
    }



    // ---------------------------------------------------------
    // Creates a blueish stat entry and adds it to StatsView
    // ---------------------------------------------------------
    void AddStat(string statName, int statValue)
    {
        VisualElement entry = new VisualElement();
        entry.style.flexDirection = FlexDirection.Row;
        entry.style.justifyContent = Justify.SpaceBetween;
        entry.style.alignItems = Align.Center;
        entry.style.paddingLeft = 8;
        entry.style.paddingRight = 8;
        entry.style.paddingTop = 8;
        entry.style.paddingBottom = 8;
        entry.style.marginBottom = 6;

        entry.style.backgroundColor = new Color(0.15f, 0.25f, 0.45f, 0.85f);

        entry.style.borderBottomWidth = 1;
        entry.style.borderTopWidth = 1;
        entry.style.borderLeftWidth = 1;
        entry.style.borderRightWidth = 1;

        Color borderColor = new Color(0.1f, 0.1f, 0.2f);
        entry.style.borderBottomColor = borderColor;
        entry.style.borderTopColor = borderColor;
        entry.style.borderLeftColor = borderColor;
        entry.style.borderRightColor = borderColor;

        entry.style.borderTopLeftRadius = 4;
        entry.style.borderTopRightRadius = 4;
        entry.style.borderBottomLeftRadius = 4;
        entry.style.borderBottomRightRadius = 4;

        Label nameLabel = new Label(statName);
        nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        nameLabel.style.color = Color.white;
        nameLabel.style.fontSize = 20;

        Label valueLabel = new Label(statValue.ToString() + "%");
        valueLabel.style.color = Color.white;
        valueLabel.style.fontSize = 20;

        entry.Add(nameLabel);
        entry.Add(valueLabel);

        StatsView.Add(entry);
    }


}
