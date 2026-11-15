using UnityEngine;

// The player's shares of one company, and the current price of a share.
public class Portfolio : MonoBehaviour
{
    [SerializeField] private StockData stock;
    [SerializeField] private Wallet wallet;

    // Raised when the number of shares or the price changes.
    public event System.Action Changed;

    public string CompanyName => stock.companyName;

    public int Shares { get; private set; }

    public int Price { get; private set; }

    public int LastChangePercent { get; private set; }

    public int Value => Shares * Price;

    private void Awake()
    {
        Price = stock.startPrice;
    }

    public bool Buy(int count)
    {
        if (!wallet.TrySpend(count * Price))
        {
            return false;
        }

        Shares += count;
        Changed?.Invoke();
        return true;
    }

    public bool Sell(int count)
    {
        if (count > Shares)
        {
            return false;
        }

        Shares -= count;
        wallet.Add(count * Price);
        Changed?.Invoke();
        return true;
    }

    // Moves the price by a random monthly change and returns the change in percent.
    public int UpdatePrice()
    {
        float change = Random.Range(stock.minMonthlyChange, stock.maxMonthlyChange);
        int newPrice = Mathf.Max(stock.minPrice, Mathf.RoundToInt(Price * (1f + change)));
        LastChangePercent = Mathf.RoundToInt((newPrice - Price) * 100f / Price);
        Price = newPrice;
        Changed?.Invoke();
        return LastChangePercent;
    }
}
