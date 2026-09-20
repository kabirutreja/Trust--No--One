using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneManagerr : MonoBehaviour
{
    public Animator transitionAnim;
    public string sceneName;
    public string sceneNameGame;
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    public void SceneChange()
    {
        StartCoroutine(LoadScene());

    }
    public void SceneChangeGame()
    {
        StartCoroutine(LoadSceneGame());

    }
    public void End()
    {
        Application.Quit();
    }
    IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneName);
    }
    IEnumerator LoadSceneGame()
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneNameGame);
    }
}
