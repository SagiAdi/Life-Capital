using UnityEngine;

// Base class for the places in the city (home, work, school, bank, exchange).
// Using a building raises an event, and the UI shows the building's panel.
// To add a new place: write one new subclass and put it in the scene. Nothing else changes.
public abstract class Building : MonoBehaviour
{
    protected const string NoActionsLeft = "No actions left this month. Go home and end the month.";

    [SerializeField] private BuildingChannel buildingEntered;

    public abstract string Title { get; }

    // One line about the current state, shown under the title (for example the share price).
    public abstract string Info { get; }

    public abstract BuildingAction[] GetActions();

    public void Interact()
    {
        buildingEntered.Raise(this);
    }
}
