using UnityEngine;

// Work: overtime costs one action and pays in coins that fly out of the building.
// The player has to collect the coins before they disappear.
public class WorkBuilding : Building
{
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private CoinFountain coinFountain;
    [SerializeField] private MessageChannel messages;
    [SerializeField] private int coinsPerShift = 5;
    [SerializeField] private int coinValue = 150;

    public override string Title => "Work";

    public override string Info => "Overtime pays " + coinsPerShift + " coins of " + coinValue + ". Collect them fast!";

    public override BuildingAction[] GetActions()
    {
        return new[]
        {
            new BuildingAction("Work overtime (1 action)", WorkOvertime, true),
        };
    }

    private string WorkOvertime()
    {
        // The panel closes after this action, so the result is sent as a message instead.
        if (!monthCycle.HasActions)
        {
            messages.Raise(NoActionsLeft);
            return NoActionsLeft;
        }

        monthCycle.UseAction();
        coinFountain.Burst(coinsPerShift, coinValue);
        messages.Raise("Overtime done! Collect the coins before they disappear.");
        return "";
    }
}
