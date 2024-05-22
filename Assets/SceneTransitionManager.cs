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
        StartCoroutine(TransitionToScene(sceneToLoadIndex));
    }

    // Method to transition to the scene using the specified index
    public IEnumerator TransitionToScene(int sceneIndex)
    {
        // Load the scene asynchronously by build index
        Debug.Log("Loading scene index: " + sceneIndex);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Overloaded method to transition to the scene using the default index
    public IEnumerator TransitionToScene()
    {
        yield return TransitionToScene(sceneToLoadIndex);
    }
}
