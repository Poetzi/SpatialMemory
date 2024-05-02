using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public void GoToSceneAsync(int sceneIndex)
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
