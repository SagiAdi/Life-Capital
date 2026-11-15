using System;
using UnityEngine;

// The win and lose conditions. They are checked at the end of every month.
// Win: the net worth reached the goal. Lose: too many months in debt, or the last month ended.
public class GameRules : MonoBehaviour
{
    [SerializeField] private EconomySettings settings;
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private Wallet wallet;
    [SerializeField] private NetWorth netWorth;
    [SerializeField] private GamePause pause;

    private int monthsInDebt;
    private bool ended;

    public event Action<GameResult> GameEnded;

    private void OnEnable()
    {
        monthCycle.MonthEnded += OnMonthEnded;
    }

    private void OnDisable()
    {
        monthCycle.MonthEnded -= OnMonthEnded;
    }

    private void OnMonthEnded(MonthReport report)
    {
        if (ended)
        {
            return;
        }

        monthsInDebt = wallet.Cash < 0 ? monthsInDebt + 1 : 0;

        if (netWorth.Current >= settings.goalNetWorth)
        {
            End(true, "You reached the goal in month " + report.Month + "!");
        }
        else if (monthsInDebt >= settings.debtMonthsToLose)
        {
            End(false, "You ended " + monthsInDebt + " months in a row in debt.");
        }
        else if (report.Month >= settings.totalMonths)
        {
            End(false, "The last month is over and the goal was not reached.");
        }
    }

    private void End(bool won, string reason)
    {
        ended = true;
        pause.SetPaused(true);

        var result = new GameResult();
        result.Won = won;
        result.Reason = reason;
        result.NetWorth = netWorth.Current;
        result.Goal = settings.goalNetWorth;
        GameEnded?.Invoke(result);
    }
}
