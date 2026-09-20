using UnityEngine;

public class RedirectToWebsite : MonoBehaviour
{
    [SerializeField] private string url = "https://www.image2url.com/r2/default/files/1788027143008-b5a4d44b-7a03-45eb-a4c5-f48232fe6f68.txt";

    public void OpenWebsite()
    {
        Application.OpenURL(url);
    }
}