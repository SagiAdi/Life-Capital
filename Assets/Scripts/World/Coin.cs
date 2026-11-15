using UnityEngine;

// A coin with a physics body. It is collected when it touches something that can hold money (the player).
// A new coin cannot be collected for a short moment, so it first flies away from the building.
// A coin that nobody collects disappears after a few seconds.
[RequireComponent(typeof(Rigidbody))]
public class Coin : MonoBehaviour
{
    [SerializeField] private float lifetime = 8f;

    [SerializeField, Tooltip("Seconds after it appears before the coin can be collected.")]
    private float pickupDelay = 0.6f;

    private int value;
    private bool collected;
    private float pickupTime;

    public void SetValue(int coinValue)
    {
        value = coinValue;
    }

    private void Start()
    {
        pickupTime = Time.time + pickupDelay;
        Destroy(gameObject, lifetime);
    }

    // OnTriggerStay (not Enter): the player may already be touching the coin when the delay ends.
    private void OnTriggerStay(Collider other)
    {
        if (collected || Time.time < pickupTime || !other.TryGetComponent(out IMoneyHolder holder))
        {
            return;
        }

        collected = true;
        holder.Receive(value, "overtime pay");
        Destroy(gameObject);
    }
}
