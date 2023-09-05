using System.Collections;
using UnityEngine;

public class BananaObjectPool : ObjectPool
{
    [SerializeField] private GameObject _bananaTemplate;
    [SerializeField] private AudioSource _throwSound;

    private float _firstPauseTime = 1;
    private float _pauseTimeBetweenThrows = 2.05f;
    private bool _isFirstPauseCompleted = false;

    private Coroutine _currentCoroutine;
    private Coroutine _firstPauseCoroutine;

    public bool TryGetBanana()
    {
        bool isSuccesfull = false;

        if (_isFirstPauseCompleted)
        {
            if (_currentCoroutine == null)
            {
                _currentCoroutine = StartCoroutine(ThrowBanana());

                isSuccesfull = true;
            }
        }
        else
        {
            if (_firstPauseCoroutine == null)
                _firstPauseCoroutine = StartCoroutine(WaitFirstPause(_firstPauseTime));
        }

        return isSuccesfull;
    }

    public void ResetFirstPause()
    {
        _isFirstPauseCompleted = false;
        _firstPauseCoroutine = null;
    }

    private void Start()
    {
        Initialize(_bananaTemplate);
    }

    private void SetBanana(GameObject banana)
    {
        banana.transform.position = transform.position;
        banana.SetActive(true);
        _throwSound.Play();
    }

    private IEnumerator ThrowBanana()
    {
        WaitForSeconds pause = new WaitForSeconds(_pauseTimeBetweenThrows);

        if (TryGetObject(out GameObject banana))
        {
            SetBanana(banana);
        }

        yield return pause;

        _currentCoroutine = null;
    }

    private IEnumerator WaitFirstPause(float duration)
    {
        yield return new WaitForSeconds(duration);

        _isFirstPauseCompleted = true;
    }
}
