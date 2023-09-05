using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryLocation : MonoBehaviour
{
    private float _timeBeforeEndGame = 30;
    private int _mainMenuSceneNumber = 0;

    private void OnEnable()
    {
        StartCoroutine(EndGame());
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(_timeBeforeEndGame);

        SceneManager.LoadScene(_mainMenuSceneNumber);
    }
}
