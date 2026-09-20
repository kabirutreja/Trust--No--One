using UnityEngine;

public class Randomassign : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects;
    [SerializeField] private Sprite bombSprite;
    [SerializeField] private Sprite diamondSprite;

    void Start()
    {
        int[] indices = new int[gameObjects.Length];

        // Create indices
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = i;
        }

        // Shuffle indices
        for (int i = 0; i < indices.Length; i++)
        {
            int randomIndex = Random.Range(i, indices.Length);

            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // First 5 cards = BOMBS
        for (int i = 0; i < 5; i++)
        {
            GameObject cardObject = gameObjects[indices[i]];

            SpriteRenderer sr = cardObject.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = bombSprite;
            }

           Card card = cardObject.GetComponentInParent<Card>();

            if (card != null)
            {
                card.SetBomb(true);
            }
        }

        // Remaining cards = DIAMONDS
        for (int i = 5; i < gameObjects.Length; i++)
        {
            GameObject cardObject = gameObjects[indices[i]];

            SpriteRenderer sr = cardObject.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sprite = diamondSprite;
            }

            Card card = cardObject.GetComponentInParent<Card>();

            if (card != null)
            {
                card.SetBomb(false);
            }
        }
    }
}