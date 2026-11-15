using UnityEngine;

// Uses the building in front of the player when the Interact input is pressed.
[RequireComponent(typeof(PlayerInputReader), typeof(BuildingSensor))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private GamePause pause;

    private PlayerInputReader input;
    private BuildingSensor sensor;

    private void Awake()
    {
        input = GetComponent<PlayerInputReader>();
        sensor = GetComponent<BuildingSensor>();
    }

    private void OnEnable()
    {
        input.InteractPressed += Use;
    }

    private void OnDisable()
    {
        input.InteractPressed -= Use;
    }

    private void Use()
    {
        if (pause.IsPaused || sensor.Target == null)
        {
            return;
        }

        sensor.Target.Interact();
    }
}
