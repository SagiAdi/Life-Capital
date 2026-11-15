using System.Collections.Generic;
using UnityEngine;

// Sends new bills after the player at the start of every month, and removes the bills
// that the player managed to avoid during the last month.
public class BillSpawner : MonoBehaviour
{
    [SerializeField] private MonthCycle monthCycle;
    [SerializeField] private GamePause pause;
    [SerializeField] private Transform target;
    [SerializeField] private Bill billPrefab;

    [SerializeField, Tooltip("The kinds of bill to choose from. Add a new asset here to add a new kind.")]
    private BillType[] types;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField, Tooltip("The first month is calm, so the player can learn the city.")]
    private int firstMonthWithBills = 2;

    [SerializeField] private int billsPerMonth = 2;
    [SerializeField] private float minDistanceFromTarget = 3f;

    private readonly List<Bill> alive = new List<Bill>();

    private void OnEnable()
    {
        monthCycle.MonthStarted += OnMonthStarted;
    }

    private void OnDisable()
    {
        monthCycle.MonthStarted -= OnMonthStarted;
    }

    private void OnMonthStarted(int month)
    {
        foreach (Bill bill in alive)
        {
            if (bill != null)
            {
                Destroy(bill.gameObject);
            }
        }

        alive.Clear();

        if (month < firstMonthWithBills || types.Length == 0 || spawnPoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < billsPerMonth; i++)
        {
            Bill bill = Instantiate(billPrefab, PickSpawnPosition(), Quaternion.identity, transform);
            bill.Launch(types[Random.Range(0, types.Length)], target, pause);
            alive.Add(bill);
        }
    }

    // A random spawn point that is not too close to the player.
    private Vector3 PickSpawnPosition()
    {
        Vector3 position = spawnPoints[0].position;
        for (int attempt = 0; attempt < 10; attempt++)
        {
            position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            if (Vector3.Distance(position, target.position) >= minDistanceFromTarget)
            {
                break;
            }
        }

        Vector2 offset = Random.insideUnitCircle * 0.4f;
        return position + new Vector3(offset.x, offset.y, 0f);
    }
}
