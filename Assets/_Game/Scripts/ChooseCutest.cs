using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseCutest : MonoBehaviour
{
    public void SpawnHero(GameObject hero)
    {
        GameManager.Instance.player = hero;
        SceneManager.LoadScene("Tutorial map1");
    }
}
