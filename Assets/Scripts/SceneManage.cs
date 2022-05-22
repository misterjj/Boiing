using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
    }
    public void Play()
    {
        Debug.Log("click");
        SceneManager.LoadScene("Game");
    }
    public void Feats()
    {
        SceneManager.LoadScene("ComingSoon");
    }
    public void Stats()
    {
        SceneManager.LoadScene("ComingSoon");
    }
    public void Credits()
    {
        SceneManager.LoadScene("ComingSoon");
    }
}
