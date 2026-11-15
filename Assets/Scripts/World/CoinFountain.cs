using UnityEngine;

// Throws coins out of a point. Every coin is a Rigidbody that gets a push (AddForce) and a spin (AddTorque).
public class CoinFountain : MonoBehaviour
{
    [SerializeField] private Coin coinPrefab;

    [SerializeField, Tooltip("Speed of a coin right after the push.")]
    private float launchSpeed = 8f;

    [SerializeField] private float spinSpeed = 5f;

    [SerializeField, Tooltip("Coins fly out inside this angle, in degrees, around the 'down' direction of this object.")]
    private float spreadAngle = 120f;

    public void Burst(int count, int valueEach)
    {
        for (int i = 0; i < count; i++)
        {
            Coin coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            coin.SetValue(valueEach);

            float angle = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
            Vector3 direction = Quaternion.Euler(0f, 0f, angle) * -transform.up;
            float speed = launchSpeed * Random.Range(0.6f, 1f);

            // VelocityChange: the push sets the speed directly, whatever the mass of the coin is.
            Rigidbody body = coin.GetComponent<Rigidbody>();
            body.AddForce(direction * speed, ForceMode.VelocityChange);
            body.AddTorque(Vector3.forward * spinSpeed * Random.Range(-1f, 1f), ForceMode.VelocityChange);
        }
    }
}
