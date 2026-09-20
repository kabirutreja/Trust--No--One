using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    // Fires automatically when the mouse cursor enters the button's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverClip);
    }

    private void PlayClickSound()
    {
        PlaySound(clickClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }
}