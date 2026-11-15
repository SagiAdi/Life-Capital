using UnityEngine;

// One kind of unexpected expense that chases the player (data only, no logic).
// To add a new kind: create a new asset and add it to the Bill Spawner list.
[CreateAssetMenu(menuName = "Life Capital/Bill Type")]
public class BillType : ScriptableObject
{
    public string displayName = "Phone bill";
    public int amount = 300;
    public Color color = new Color(0.9f, 0.25f, 0.25f);

    [Tooltip("Side length in world units.")]
    public float size = 0.5f;

    [Tooltip("The force that pushes the bill towards the player. More force = faster bill.")]
    public float chaseForce = 3f;

    [Tooltip("Spin given when the bill appears (AddTorque).")]
    public float spinTorque = 1f;
}
