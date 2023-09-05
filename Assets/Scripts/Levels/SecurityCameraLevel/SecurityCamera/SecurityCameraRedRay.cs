using System.Collections;
using UnityEngine;

public class SecurityCameraRedRay : MonoBehaviour
{
    [SerializeField] private AudioSource _alarmSound;

    private float _alarmTime = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player player))
        {
            StartCoroutine(Alarm());

            LevelsChanger.Instance.CurrentLevel.Restart();
        }
    }

    private IEnumerator Alarm()
    {
        _alarmSound.Play();

        yield return new WaitForSeconds(_alarmTime);

        _alarmSound.Stop();
    }
}
