using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialDeletePlayerPrefs : MonoBehaviour
{
    public ClearPlayerPrefs clearPlayerPrefs;
    // Start is called before the first frame update
    void Start()
    {
        clearPlayerPrefs.ClearSavedData();
    }
}
