using System;
using UnityEngine;

// The player's job: the monthly salary, which grows with courses.
public class Career : MonoBehaviour
{
    [SerializeField] private EconomySettings settings;

    public event Action<int> SalaryChanged;

    public int Salary { get; private set; }

    private void Awake()
    {
        Salary = settings.monthlySalary;
    }

    public void Raise(int amount)
    {
        Salary += amount;
        SalaryChanged?.Invoke(Salary);
    }
}
