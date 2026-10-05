using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TMP_Text _value;
    [SerializeField] private float _minLoadingTime;

    private Coroutine _loadingCoroutine;

    void Start()
    {
        if (_loadingCoroutine != null) StopCoroutine(_loadingCoroutine);
        _loadingCoroutine = StartCoroutine(LoadTargetScene());
    }

    private IEnumerator LoadTargetScene()
    {
        string targetScene = SceneTransition.TargetScence;
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("Target Scene is null!");
            yield break;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;

        float timer = 0f;
        while (!operation.isDone) 
        {
            timer += Time.deltaTime;

            float process = Mathf.Clamp01(operation.progress / 0.9f);

            if (_slider != null) _slider.value = process;
            if (_value != null) _value.text = $"{Mathf.RoundToInt(process * 100f)}%";
            if (operation.progress >= 0.9f && timer >= _minLoadingTime)
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
