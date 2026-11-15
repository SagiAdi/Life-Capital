using UnityEngine;

// Home: the place where the player ends the month.
public class HomeBuilding : Building
{
    [SerializeField] private MonthCycle monthCycle;

    public override string Title => "Home";

    public override string Info => "Month " + monthCycle.Month + ". Actions left: " + monthCycle.ActionsLeft + ".";

    public override BuildingAction[] GetActions()
    {
        return new[]
        {
            new BuildingAction("End the month: get your salary, pay your expenses", EndMonth),
        };
    }

    private string EndMonth()
    {
        MonthReport report = monthCycle.EndMonth();
        return "Month " + report.Month + " ended: salary +" + report.Salary + ", expenses -" + report.Expenses
               + ", interest +" + report.Interest + ", share price " + report.StockChangePercent.ToString("+0;-0") + "%.";
    }
}
