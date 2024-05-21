using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private int sceneToLoadIndex;
    [SerializeField] private Button transitionButton;

    private void Start()
    {
        if (transitionButton != null)
        {
            transitionButton.onClick.AddListener(OnTransitionButtonClicked);
        }
        else
        {
            Debug.LogError("Transition Button is not assigned in the Inspector");
        }
    }

    private void OnTransitionButtonClicked()
    {
        Debug.Log("Transition button clicked");
        StartCoroutine(TransitionToScene());
    }

    private IEnumerator TransitionToScene()
    {
        // Load the scene asynchronously by build index
        Debug.Log("Loading scene index: " + sceneToLoadIndex);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoadIndex);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
