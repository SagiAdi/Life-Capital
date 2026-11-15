using UnityEngine;

// One company on the stock exchange (data only). The price changes once a month, at random inside the range.
[CreateAssetMenu(menuName = "Life Capital/Stock")]
public class StockData : ScriptableObject
{
    public string companyName = "GreenTech";
    public int startPrice = 100;

    [Tooltip("-0.12 = the price can fall by up to 12% in a month.")]
    public float minMonthlyChange = -0.12f;

    [Tooltip("0.20 = the price can rise by up to 20% in a month.")]
    public float maxMonthlyChange = 0.20f;

    public int minPrice = 5;
}
