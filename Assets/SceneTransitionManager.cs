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

    public void OnTransitionButtonClicked(int sceneIndex)
    {
        Debug.Log("Transition button clicked");
        StartCoroutine(TransitionToScene(sceneIndex));
    }


    private void OnTransitionButtonClicked()
    {
        Debug.Log("Transition button clicked");
        StartCoroutine(TransitionToScene(sceneToLoadIndex));
    }

    // Method to transition to the scene using the specified index
    public IEnumerator TransitionToScene(int sceneIndex)
    {
        // Load the scene synchronously by build index
        Debug.Log("Loading scene index: " + sceneIndex);
        SceneManager.LoadScene(sceneIndex);

        // Yield return null to allow Unity to process the scene change
        yield return null;
    }

    // Overloaded method to transition to the scene using the default index
    public IEnumerator TransitionToScene()
    {
        yield return TransitionToScene(sceneToLoadIndex);
    }
}
