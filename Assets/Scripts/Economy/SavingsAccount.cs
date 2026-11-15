using System;
using UnityEngine;

// The savings account in the bank: safe money that earns a little interest every month.
public class SavingsAccount : MonoBehaviour
{
    [SerializeField] private Wallet wallet;

    public event Action<int> BalanceChanged;

    public int Balance { get; private set; }

    public bool Deposit(int amount)
    {
        if (!wallet.TrySpend(amount))
        {
            return false;
        }

        Balance += amount;
        BalanceChanged?.Invoke(Balance);
        return true;
    }

    public bool Withdraw(int amount)
    {
        if (amount > Balance)
        {
            return false;
        }

        Balance -= amount;
        wallet.Add(amount);
        BalanceChanged?.Invoke(Balance);
        return true;
    }

    // Adds one month of interest and returns how much was added.
    public int AddInterest(float monthlyRate)
    {
        int interest = Mathf.RoundToInt(Balance * monthlyRate);
        if (interest != 0)
        {
            Balance += interest;
            BalanceChanged?.Invoke(Balance);
        }

        return interest;
    }
}
