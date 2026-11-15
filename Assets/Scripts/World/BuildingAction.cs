using System;

// One thing the player can do inside a building: the button text, and what happens when it is pressed.
public class BuildingAction
{
    public BuildingAction(string label, Func<string> run, bool leavesBuilding = false)
    {
        Label = label;
        Run = run;
        LeavesBuilding = leavesBuilding;
    }

    public string Label { get; }

    // Does the action and returns a short message about what happened.
    public Func<string> Run { get; }

    // True when the player has to walk right after the action, so the panel closes.
    public bool LeavesBuilding { get; }
}
