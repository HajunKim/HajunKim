using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenechanger : MonoBehaviour
{
    public void Scenechange()
    {
        SceneManager.LoadScene("Scene+controller_map");
    }
}
