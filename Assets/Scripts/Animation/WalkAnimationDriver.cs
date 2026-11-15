using UnityEngine;

// Tells the sprite animator how the player is moving, so the walk cycle matches the movement.
[RequireComponent(typeof(PlayerWalker), typeof(SpriteAnimator))]
public class WalkAnimationDriver : MonoBehaviour
{
    private PlayerWalker walker;
    private SpriteAnimator animator;

    private void Awake()
    {
        walker = GetComponent<PlayerWalker>();
        animator = GetComponent<SpriteAnimator>();
    }

    private void Update()
    {
        animator.Drive(walker.Velocity);
    }
}
