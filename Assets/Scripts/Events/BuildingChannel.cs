using UnityEngine;

// Raised by a building when the player uses it. The building panel (UI) listens and opens.
[CreateAssetMenu(menuName = "Life Capital/Events/Building Channel")]
public class BuildingChannel : EventChannel<Building>
{
}
