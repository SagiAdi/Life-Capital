using UnityEngine;

// Moves the player with a CharacterController: a kinematic controller.
// The player moves exactly as the input says and slides along walls. Forces never push it.
// The city is seen from above, so the movement is on the X and Y axes and there is no gravity.
[RequireComponent(typeof(CharacterController), typeof(PlayerInputReader))]
public class PlayerWalker : MonoBehaviour
{
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private GamePause pause;

    private CharacterController controller;
    private PlayerInputReader input;

    // The last direction the player moved in. The building sensor looks this way.
    public Vector2 Facing { get; private set; } = Vector2.up;

    // The current movement, for the walk animation.
    public Vector2 Velocity { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        Vector2 direction = pause.IsPaused ? Vector2.zero : input.Move;
        if (direction.sqrMagnitude > 0.01f)
        {
            Facing = direction.normalized;
        }

        Velocity = direction * speed;
        controller.Move(new Vector3(Velocity.x, Velocity.y, 0f) * Time.deltaTime);
    }
}
