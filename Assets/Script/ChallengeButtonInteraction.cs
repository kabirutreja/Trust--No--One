using UnityEngine;

 // use Collider2D below if this is a 2D project
public class ChallengeButtonInteraction : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private GameManager gameManager;

    private void OnMouseEnter()
    {
        PlaySound(hoverClip);
    }

    private void OnMouseDown()
    {
        PlaySound(clickClip);

        if (gameManager != null)
            gameManager.Challenge();
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
}