using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    private Spawn_block spawn_Block;
    private GameObject main_Menu;

    private void Start()
    {
        spawn_Block = GameObject.Find("Spawn_block").GetComponent<Spawn_block>();
        main_Menu = GameObject.Find("Main_Menu");
    }
    public void StartGame()
    {
        spawn_Block.audioSource.time = 0;
        spawn_Block.audioSource.Play();
        spawn_Block.SpawnObject();
        main_Menu.SetActive(false);
    }

    public void HomeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Chuyển đến Scene chơi game (SampleScene)
    }

    public void QuitGame()
    {
        // Kiểm tra xem ứng dụng đang chạy trong trình duyệt hay không.
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                                        Application.Quit();
        #endif
    }
}
