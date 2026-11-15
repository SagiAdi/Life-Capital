using System;
using UnityEngine;

// The player's cash.
public class Wallet : MonoBehaviour
{
    [SerializeField] private EconomySettings settings;

    public event Action<int> CashChanged;

    public int Cash { get; private set; }

    private void Awake()
    {
        Cash = settings.startCash;
    }

    public void Add(int amount)
    {
        Cash += amount;
        CashChanged?.Invoke(Cash);
    }

    // A payment the player chooses to make. It fails when there is not enough cash.
    public bool TrySpend(int amount)
    {
        if (amount > Cash)
        {
            return false;
        }

        Cash -= amount;
        CashChanged?.Invoke(Cash);
        return true;
    }

    // A payment that cannot be refused (living expenses, bills). The cash can go below zero: that is debt.
    public void Charge(int amount)
    {
        Cash -= amount;
        CashChanged?.Invoke(Cash);
    }
}
