using UnityEngine;

public class MenuMusicController : MonoBehaviour
{
    public static MenuMusicController Instance;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;

            DontDestroyOnLoad(transform.gameObject);
        }
    }
}