using UnityEngine;

// Connects the player's body to the wallet: coins that touch the player are received,
// and bills that touch the player are paid.
public class PlayerMoney : MonoBehaviour, IMoneyHolder
{
    [SerializeField] private Wallet wallet;
    [SerializeField] private MessageChannel messages;

    public void Receive(int amount, string source)
    {
        wallet.Add(amount);
        messages.Raise("+" + amount + " (" + source + ")");
    }

    public void Pay(int amount, string reason)
    {
        wallet.Charge(amount);
        messages.Raise("Unexpected expense! " + reason + ": -" + amount);
    }
}
