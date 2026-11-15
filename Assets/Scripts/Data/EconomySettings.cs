using UnityEngine;

// The numbers that tune the level: starting money, salary, expenses, and the goal.
[CreateAssetMenu(menuName = "Life Capital/Economy Settings")]
public class EconomySettings : ScriptableObject
{
    [Header("Start")]
    public int startCash = 10000;
    public int monthlySalary = 6000;
    public int monthlyExpenses = 4000;

    [Header("Goal")]
    [Tooltip("The player wins when the net worth reaches this at the end of a month.")]
    public int goalNetWorth = 45000;

    [Tooltip("The level ends after this many months.")]
    public int totalMonths = 12;

    [Tooltip("Ending this many months in a row with negative cash loses the game.")]
    public int debtMonthsToLose = 3;

    [Header("Month")]
    public int actionsPerMonth = 3;

    [Tooltip("0.015 = 1.5% interest on the savings every month.")]
    public float savingsMonthlyInterest = 0.015f;
}
