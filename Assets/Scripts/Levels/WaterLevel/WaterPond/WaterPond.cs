using UnityEngine;

public class WaterPond : MonoBehaviour
{
    [SerializeField] private AudioSource _waterSound;

    public void StartPlaySounds()
    {
        if (_waterSound.isPlaying == false)
            _waterSound.Play();
    }

    public void StopPlaySounds()
    {
        if (_waterSound.isPlaying == true)
            _waterSound.Stop();
    }
}
