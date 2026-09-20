using UnityEngine;

public class ChallengeButton : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.Challenge();
        }
    }
}