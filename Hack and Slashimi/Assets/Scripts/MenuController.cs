using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [SerializeField] string SceneToLoad;

    public void onClick()
    {
        SceneManager.LoadScene(SceneToLoad);
    }
}
