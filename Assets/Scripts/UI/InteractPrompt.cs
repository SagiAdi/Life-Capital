using UnityEngine;
using UnityEngine.UI;

// The on-screen hint ("Enter (E): Bank") for the building in front of the player.
public class InteractPrompt : MonoBehaviour
{
    [SerializeField] private BuildingSensor sensor;

    private Text label;

    private void Awake()
    {
        if (sensor == null)
        {
            sensor = FindAnyObjectByType<BuildingSensor>();
        }

        Canvas canvas = UIBuilder.CreateCanvas("Interact Prompt", 0);
        label = UIBuilder.CreateText(canvas.transform, "", 36, new Vector2(0, -300), new Vector2(900, 60));
    }

    private void OnEnable()
    {
        sensor.TargetChanged += Show;
    }

    private void OnDisable()
    {
        sensor.TargetChanged -= Show;
    }

    private void Show(Building target)
    {
        label.text = target != null ? "Enter (E): " + target.Title : "";
    }
}
