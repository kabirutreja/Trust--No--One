using UnityEngine;

public class Card : MonoBehaviour
{
    public bool IsBomb;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    public void SetBomb(bool bomb)
    {
        IsBomb = bomb;
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.PlayerPickedCard(gameObject);
        }
    }
}