using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// The win / lose screen. It appears when the game rules announce that the game ended.
public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameRules gameRules;

    private GameObject root;
    private Text titleText;
    private Text bodyText;

    private void Awake()
    {
        Canvas canvas = UIBuilder.CreateCanvas("End Screen", 20);
        Transform t = canvas.transform;
        root = canvas.gameObject;

        UIBuilder.CreatePanel(t, new Color(0.08f, 0.1f, 0.16f, 0.97f), new Vector2(4000, 4000), Vector2.zero, "Background");
        titleText = UIBuilder.CreateText(t, "", 90, new Vector2(0, 250), new Vector2(1600, 140));
        bodyText = UIBuilder.CreateText(t, "", 40, new Vector2(0, 0), new Vector2(1500, 300));
        UIBuilder.CreateButton(t, "Play again", new Vector2(0, -330), new Vector2(360, 110), PlayAgain);
        root.SetActive(false);
    }

    private void OnEnable()
    {
        gameRules.GameEnded += Show;
    }

    private void OnDisable()
    {
        gameRules.GameEnded -= Show;
    }

    private void Show(GameResult result)
    {
        titleText.text = result.Won ? "You reached your goal!" : "Game over";
        bodyText.text = result.Reason + "\nNet worth: " + result.NetWorth + " (goal " + result.Goal + ").";
        root.SetActive(true);
    }

    private void PlayAgain()
    {
        // Reloading the scene resets everything: the game keeps no state outside the scene.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
