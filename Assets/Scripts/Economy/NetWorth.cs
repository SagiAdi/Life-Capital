using System;
using UnityEngine;

// The player's net worth: cash + savings + the value of the shares.
// It listens to the three parts and raises one event when any of them changes.
public class NetWorth : MonoBehaviour
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private SavingsAccount savings;
    [SerializeField] private Portfolio portfolio;

    public event Action<int> Changed;

    public int Current => wallet.Cash + savings.Balance + portfolio.Value;

    private void OnEnable()
    {
        wallet.CashChanged += OnAmountChanged;
        savings.BalanceChanged += OnAmountChanged;
        portfolio.Changed += OnPortfolioChanged;
    }

    private void OnDisable()
    {
        wallet.CashChanged -= OnAmountChanged;
        savings.BalanceChanged -= OnAmountChanged;
        portfolio.Changed -= OnPortfolioChanged;
    }

    private void OnAmountChanged(int newAmount)
    {
        Changed?.Invoke(Current);
    }

    private void OnPortfolioChanged()
    {
        Changed?.Invoke(Current);
    }
}
