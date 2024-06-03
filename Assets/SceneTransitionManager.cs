using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private int sceneToLoadIndex;
    [SerializeField] private Button transitionButton;

    private bool isTransitioning = false;

    private void Start()
    {
        if (transitionButton != null)
        {
            transitionButton.onClick.AddListener(() => StartCoroutine(OnTransitionButtonClicked(sceneToLoadIndex)));
        }
        else
        {
            Debug.LogError("Transition Button is not assigned in the Inspector");
        }
    }

    // Method to transition to the scene using the specified index
    public IEnumerator OnTransitionButtonClicked(int sceneIndex)
    {
        if (isTransitioning)
        {
            Debug.LogWarning("Transition is already in progress");
            yield break;
        }

        isTransitioning = true;
        Debug.Log("Loading scene index: " + sceneIndex);

        yield return null;

        // Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        // Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        // Check if asyncOperation is null
        if (asyncOperation == null)
        {
            Debug.LogError("AsyncOperation is null. Scene loading failed.");
            isTransitioning = false;
            yield break;
        }

        Debug.Log("Pro :" + asyncOperation.progress);

        // When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            // Output the current progress
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                // Change the Text to show the Scene is ready
                Debug.Log("Scene is ready");

                yield return new WaitForSeconds(1);

                // Activate the Scene
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene loaded successfully");
        isTransitioning = false;
    }
}
