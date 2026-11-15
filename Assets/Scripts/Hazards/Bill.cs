using UnityEngine;

// An unexpected expense that chases the player. It is a Rigidbody that is pushed towards
// the player every physics step (AddForce). When it touches the player, the player pays it.
[RequireComponent(typeof(Rigidbody), typeof(SpriteRenderer))]
public class Bill : MonoBehaviour
{
    private Rigidbody body;
    private BillType type;
    private Transform target;
    private GamePause pause;
    private bool paid;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    // Called once by the spawner, right after the bill is created.
    public void Launch(BillType billType, Transform chaseTarget, GamePause gamePause)
    {
        type = billType;
        target = chaseTarget;
        pause = gamePause;

        transform.localScale = new Vector3(type.size, type.size, 1f);
        GetComponent<SpriteRenderer>().color = type.color;
        body.AddTorque(Vector3.forward * type.spinTorque, ForceMode.Impulse);
    }

    private void FixedUpdate()
    {
        if (type == null || target == null)
        {
            return;
        }

        if (pause.IsPaused)
        {
            body.linearVelocity = Vector3.zero;
            return;
        }

        // The body's damping works against this force, so the bill settles at a steady speed.
        Vector3 toTarget = target.position - transform.position;
        toTarget.z = 0f;
        body.AddForce(toTarget.normalized * type.chaseForce);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (paid || type == null || !other.TryGetComponent(out IMoneyHolder victim))
        {
            return;
        }

        paid = true;
        victim.Pay(type.amount, type.displayName);
        Destroy(gameObject);
    }
}
