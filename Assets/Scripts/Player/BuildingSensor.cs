using System;
using UnityEngine;

// The player's "eyes": finds the building in front of the player.
// A small sphere is cast forward (Physics.SphereCast, a thick ray), and the first solid thing it hits is checked.
// The cast starts at the center of the CharacterController: a SphereCast ignores anything
// the sphere already touches at the start, so the sphere must start inside the player's own body.
[RequireComponent(typeof(PlayerWalker), typeof(CharacterController))]
public class BuildingSensor : MonoBehaviour
{
    [SerializeField, Tooltip("Radius of the sphere that is cast forward. Keep it smaller than the player's own radius.")]
    private float castRadius = 0.2f;

    [SerializeField, Tooltip("How far forward the sphere travels.")]
    private float reach = 1f;

    private PlayerWalker walker;
    private CharacterController body;

    public event Action<Building> TargetChanged;

    // The building the player would use right now, or null.
    public Building Target { get; private set; }

    private void Awake()
    {
        walker = GetComponent<PlayerWalker>();
        body = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Building found = FindBuilding();
        if (found == Target)
        {
            return;
        }

        Target = found;
        TargetChanged?.Invoke(Target);
    }

    private Building FindBuilding()
    {
        Vector3 origin = transform.TransformPoint(body.center);
        Vector3 direction = walker.Facing;
        bool hitSomething = Physics.SphereCast(
            origin, castRadius, direction, out RaycastHit hit, reach,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        return hitSomething ? hit.collider.GetComponent<Building>() : null;
    }
}
