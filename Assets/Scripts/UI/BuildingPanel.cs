using UnityEngine;
using UnityEngine.UI;

// The panel of a building: its title, one info line, and a button for every action the building offers.
// It opens when a building raises the building channel. It works for every kind of building.
public class BuildingPanel : MonoBehaviour
{
    [SerializeField] private BuildingChannel buildingEntered;
    [SerializeField] private GamePause pause;

    private GameObject root;
    private Text titleText;
    private Text infoText;
    private Text feedbackText;
    private RectTransform list;
    private Building current;

    private void Awake()
    {
        Canvas canvas = UIBuilder.CreateCanvas("Building Panel", 10);
        Transform panel = UIBuilder.CreatePanel(
            canvas.transform, new Color(0f, 0f, 0f, 0.92f), new Vector2(1300, 800), Vector2.zero, "Building");
        root = canvas.gameObject;

        titleText = UIBuilder.CreateText(panel, "", 56, new Vector2(0, 330), new Vector2(1200, 80));
        infoText = UIBuilder.CreateText(panel, "", 32, new Vector2(0, 250), new Vector2(1200, 80));
        list = UIBuilder.CreatePanel(panel, new Color(1f, 1f, 1f, 0f), new Vector2(1200, 340), new Vector2(0, 20), "Actions");
        feedbackText = UIBuilder.CreateText(panel, "", 32, new Vector2(0, -205), new Vector2(1200, 90));
        UIBuilder.CreateButton(panel, "Leave", new Vector2(0, -320), new Vector2(260, 90), Close);
        root.SetActive(false);
    }

    private void OnEnable()
    {
        buildingEntered.Raised += Open;
    }

    private void OnDisable()
    {
        buildingEntered.Raised -= Open;
    }

    private void Open(Building building)
    {
        current = building;
        feedbackText.text = "";
        Refresh();
        pause.SetPaused(true);
        root.SetActive(true);
    }

    // Rebuilds the texts and the buttons, because prices and balances change after every action.
    private void Refresh()
    {
        titleText.text = current.Title;
        infoText.text = current.Info;

        for (int i = list.childCount - 1; i >= 0; i--)
        {
            Destroy(list.GetChild(i).gameObject);
        }

        float y = 110f;
        foreach (BuildingAction action in current.GetActions())
        {
            BuildingAction chosen = action;
            UIBuilder.CreateButton(list, action.Label, new Vector2(0, y), new Vector2(1200, 100), () => Run(chosen));
            y -= 120f;
        }
    }

    private void Run(BuildingAction action)
    {
        feedbackText.text = action.Run();
        if (action.LeavesBuilding)
        {
            Close();
        }
        else
        {
            Refresh();
        }
    }

    private void Close()
    {
        current = null;
        pause.SetPaused(false);
        root.SetActive(false);
    }
}
