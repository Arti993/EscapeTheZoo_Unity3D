using System.Collections;
using UnityEngine;

public class BambooObjectPool : ObjectPool
{
    [SerializeField] private GameObject _bambooTemplate;

    private Coroutine _pauseBetweenThrowsCoroutine;
    private float _pauseTimeBetweenThrows = 0.2f;

    private void OnEnable()
    {
        Initialize(_bambooTemplate);
    }

    public bool TryGetBamboo()
    {
        bool isSuccesfull = false;

        if (_pauseBetweenThrowsCoroutine == null)
        {
            _pauseBetweenThrowsCoroutine = StartCoroutine(ThrowBamboo());

            isSuccesfull = true;
        }

        return isSuccesfull;
    }

    private void SetBamboo(GameObject bambooStick)
    {
        bambooStick.transform.position = transform.position;
        bambooStick.SetActive(true);
    }

    private IEnumerator ThrowBamboo()
    {
        if (TryGetObject(out GameObject bambooStick))
        {
            SetBamboo(bambooStick);
        }

        yield return new WaitForSeconds(_pauseTimeBetweenThrows);

        _pauseBetweenThrowsCoroutine = null;
    }
}
