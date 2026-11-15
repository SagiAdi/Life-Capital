using System;
using UnityEngine;

// The turn structure of the game: every turn is one month with a limited number of actions.
// Ending the month pays the salary, charges the expenses, and updates the savings and the share price.
public class MonthCycle : MonoBehaviour
{
    [SerializeField] private EconomySettings settings;
    [SerializeField] private Wallet wallet;
    [SerializeField] private Career career;
    [SerializeField] private SavingsAccount savings;
    [SerializeField] private Portfolio portfolio;

    public event Action<int> ActionsChanged;
    public event Action<MonthReport> MonthEnded;
    public event Action<int> MonthStarted;

    public int Month { get; private set; }

    public int ActionsLeft { get; private set; }

    public bool HasActions => ActionsLeft > 0;

    private void Awake()
    {
        Month = 1;
        ActionsLeft = settings.actionsPerMonth;
    }

    public void UseAction()
    {
        ActionsLeft = Mathf.Max(0, ActionsLeft - 1);
        ActionsChanged?.Invoke(ActionsLeft);
    }

    public MonthReport EndMonth()
    {
        var report = new MonthReport();
        report.Month = Month;
        report.Salary = career.Salary;
        report.Expenses = settings.monthlyExpenses;

        wallet.Add(report.Salary);
        wallet.Charge(report.Expenses);
        report.Interest = savings.AddInterest(settings.savingsMonthlyInterest);
        report.StockChangePercent = portfolio.UpdatePrice();
        MonthEnded?.Invoke(report);

        if (Month < settings.totalMonths)
        {
            Month++;
            ActionsLeft = settings.actionsPerMonth;
            MonthStarted?.Invoke(Month);
            ActionsChanged?.Invoke(ActionsLeft);
        }

        return report;
    }
}
