// Something that can receive and pay money. The player implements it (PlayerMoney).
// Coins and bills only know this interface, not the player.
public interface IMoneyHolder
{
    void Receive(int amount, string source);

    void Pay(int amount, string reason);
}
