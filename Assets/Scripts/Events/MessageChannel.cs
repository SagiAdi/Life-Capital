using UnityEngine;

// Short messages for the player ("+100 overtime pay"). The game raises them, the HUD shows them.
[CreateAssetMenu(menuName = "Life Capital/Events/Message Channel")]
public class MessageChannel : EventChannel<string>
{
}
