using UnityEngine;

public class FishSpawner : ObjectPool
{
    [SerializeField] private GameObject[] _fishTemplates;

    private float _minSecondsBetweenSpawn = 2;
    private float _maxSecondsBetweenSpawn = 5;
    private float _secondsBetweenSpawn;
    private float _elapsedTime = 0;

    private void Start()
    {
        Initialize(_fishTemplates);

        _secondsBetweenSpawn = Random.Range(_minSecondsBetweenSpawn, _maxSecondsBetweenSpawn);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _secondsBetweenSpawn)
        {
            if (TryGetObject(out GameObject fish))
            {
                _elapsedTime = 0;

                SetFish(fish);
            }
        }
    }

    private void SetFish(GameObject fish)
    {
        fish.transform.position = transform.position;
        fish.SetActive(true);
    }
}


