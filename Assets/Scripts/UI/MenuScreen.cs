using UnityEngine;
using UnityEngine.SceneManagement;

// Title screen with the instructions and a Start button.
public class MenuScreen : MonoBehaviour
{
    [SerializeField] private string gameScene = "City";

    private void Start()
    {
        Canvas canvas = UIBuilder.CreateCanvas("Menu", 0);
        Transform t = canvas.transform;
        UIBuilder.CreatePanel(t, new Color(0.08f, 0.1f, 0.16f), new Vector2(4000, 4000), Vector2.zero, "Background");

        UIBuilder.CreateText(t, "Life Capital", 96, new Vector2(0, 380), new Vector2(1600, 140));
        UIBuilder.CreateText(
            t, "Every decision today shapes your financial future.", 40,
            new Vector2(0, 280), new Vector2(1500, 80));
        UIBuilder.CreateText(
            t,
            "You are 18, with 10,000 in cash and a first job. Reach a net worth of 45,000 in 12 months.\n" +
            "Move: arrow keys / WASD, or the on-screen stick. Face a building and press E (or the Enter button).\n" +
            "Every month you have 3 actions: overtime at Work, a course at School,\n" +
            "a deposit at the Bank, or shares at the Stock Exchange.\n" +
            "Go Home to end the month: you get your salary and pay your expenses.\n" +
            "Red bills chase you from month 2. If one touches you, you pay it.",
            32, new Vector2(0, 40), new Vector2(1700, 380));
        UIBuilder.CreateText(
            t, "This is a learning game with made-up numbers. It is not investment advice.", 28,
            new Vector2(0, -200), new Vector2(1500, 60));

        UIBuilder.CreateButton(t, "Start", new Vector2(0, -330), new Vector2(360, 110), StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }
}
