using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn_block : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject[] objectPrefab; // Prefab của đối tượng cần sinh ra
    [SerializeField] private Transform spawnPoint; // Vị trí spawn
    public int limit_Quantity_Spawn = 20;
    private int quantity_Spawn = 0;
    public float time_to_Stop_Present = 0.5f;

    [Header("Signal")]
    public Display_blocks display_Blocks;

    [Header("Sound")]
    public AudioClip gameOver_Sound;
    public AudioSource audioSource;

    [Header("GameOver")]
    public GameObject gameOver;
    public TextMeshProUGUI gameOver_Text;

    void Start()
    {
        spawnPoint = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
        display_Blocks.ActivateRandomObject();       
    }

    public void SpawnObject()
    {
        // Sinh ra đối tượng tại vị trí spawnPoint
        Instantiate(objectPrefab[display_Blocks.number_block], spawnPoint.position, spawnPoint.rotation);

        // gia tăng tốc độ cho game --> tăng độ khó sau mỗi - limit_Quantity_Spawn - block
        if (quantity_Spawn == limit_Quantity_Spawn && time_to_Stop_Present > 0.2f)
        {
            quantity_Spawn = 0;
            time_to_Stop_Present /= 1.2f;
        }    
        quantity_Spawn++;
    }

    public void EndGame()
    {
        audioSource.Pause(); // Tạm dừng phát âm thanh
        audioSource.PlayOneShot(gameOver_Sound);

        gameOver.SetActive(true);
        gameOver_Text.text = "SCORE: " + display_Blocks.score.ToString();
    }
}

