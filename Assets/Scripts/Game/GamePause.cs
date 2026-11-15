using UnityEngine;

// One switch that stops the action in the city while a panel is open or the game is over.
public class GamePause : MonoBehaviour
{
    public bool IsPaused { get; private set; }

    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }
}
