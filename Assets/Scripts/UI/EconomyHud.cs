using UnityEngine;
using UnityEngine.UI;

// Top of the screen: month, actions, cash, savings, shares, net worth and the goal.
// It only listens to the economy events. The game never calls it.
public class EconomyHud : MonoBehaviour
{
    [SerializeField] private EconomySettings settings;
    [SerializeField] private Wallet wallet;
    [SerializeField] private SavingsAccount savings;
    [SerializeField] private Portfolio portfolio;
    [SerializeField] private Career career;
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private NetWorth netWorth;

    private Text turnText;
    private Text moneyText;
    private Text goalText;

    private void Awake()
    {
        Canvas canvas = UIBuilder.CreateCanvas("Economy HUD", 0);
        Transform t = canvas.transform;
        UIBuilder.CreatePanel(t, new Color(0f, 0f, 0f, 0.55f), new Vector2(4000, 150), new Vector2(0, 462), "Backdrop");
        turnText = UIBuilder.CreateText(t, "", 40, new Vector2(0, 505), new Vector2(1800, 50));
        moneyText = UIBuilder.CreateText(t, "", 34, new Vector2(0, 458), new Vector2(1800, 44));
        goalText = UIBuilder.CreateText(t, "", 34, new Vector2(0, 416), new Vector2(1800, 44));
    }

    private void OnEnable()
    {
        netWorth.Changed += OnNumberChanged;
        career.SalaryChanged += OnNumberChanged;
        monthCycle.ActionsChanged += OnNumberChanged;
        monthCycle.MonthStarted += OnNumberChanged;
    }

    private void OnDisable()
    {
        netWorth.Changed -= OnNumberChanged;
        career.SalaryChanged -= OnNumberChanged;
        monthCycle.ActionsChanged -= OnNumberChanged;
        monthCycle.MonthStarted -= OnNumberChanged;
    }

    private void Start()
    {
        // Start, not Awake: the economy components set their first values in their own Awake.
        Refresh();
    }

    private void OnNumberChanged(int newValue)
    {
        Refresh();
    }

    private void Refresh()
    {
        turnText.text = "Month " + monthCycle.Month + " of " + settings.totalMonths
                        + "     Actions left: " + monthCycle.ActionsLeft
                        + "     Salary: " + career.Salary + "   Expenses: " + settings.monthlyExpenses;
        moneyText.text = "Cash: " + wallet.Cash + "     Savings: " + savings.Balance
                         + "     Shares: " + portfolio.Shares + " (worth " + portfolio.Value + ")";
        goalText.text = "Net worth: " + netWorth.Current + " / goal " + settings.goalNetWorth;
    }
}
