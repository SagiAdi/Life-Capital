using UnityEngine;

// Stock exchange: buy or sell shares. The price moves once a month, up or down.
public class ExchangeBuilding : Building
{
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private Portfolio portfolio;
    [SerializeField] private int lot = 10;

    public override string Title => "Stock Exchange";

    public override string Info =>
        portfolio.CompanyName + ": " + portfolio.Price + " a share (last month " + portfolio.LastChangePercent.ToString("+0;-0")
        + "%). You own " + portfolio.Shares + " shares worth " + portfolio.Value + ".";

    public override BuildingAction[] GetActions()
    {
        int lotPrice = lot * portfolio.Price;
        return new[]
        {
            new BuildingAction("Buy " + lot + " shares for " + lotPrice + " (1 action)", Buy),
            new BuildingAction("Sell " + lot + " shares for " + lotPrice + " (1 action)", Sell),
        };
    }

    private string Buy()
    {
        if (!monthCycle.HasActions)
        {
            return NoActionsLeft;
        }

        if (!portfolio.Buy(lot))
        {
            return "Not enough cash to buy.";
        }

        monthCycle.UseAction();
        return "Bought " + lot + " shares. The price can go up or down.";
    }

    private string Sell()
    {
        if (!monthCycle.HasActions)
        {
            return NoActionsLeft;
        }

        if (!portfolio.Sell(lot))
        {
            return "You do not own " + lot + " shares.";
        }

        monthCycle.UseAction();
        return "Sold " + lot + " shares.";
    }
}
