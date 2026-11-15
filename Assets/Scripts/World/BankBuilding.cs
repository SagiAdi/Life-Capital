using UnityEngine;

// Bank: move cash into the savings account (safe, small interest) or take it back out.
public class BankBuilding : Building
{
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private SavingsAccount savings;
    [SerializeField] private EconomySettings settings;
    [SerializeField] private int step = 1000;

    public override string Title => "Bank";

    public override string Info =>
        "Savings: " + savings.Balance + ". Interest: " + (settings.savingsMonthlyInterest * 100f).ToString("0.#") + "% every month.";

    public override BuildingAction[] GetActions()
    {
        return new[]
        {
            new BuildingAction("Deposit " + step + " into savings (1 action)", Deposit),
            new BuildingAction("Withdraw " + step + " from savings (free)", Withdraw),
        };
    }

    private string Deposit()
    {
        if (!monthCycle.HasActions)
        {
            return NoActionsLeft;
        }

        if (!savings.Deposit(step))
        {
            return "Not enough cash to deposit.";
        }

        monthCycle.UseAction();
        return "Deposited " + step + ". It is safe and earns interest.";
    }

    private string Withdraw()
    {
        return savings.Withdraw(step) ? "Withdrew " + step + "." : "Not enough savings to withdraw.";
    }
}
