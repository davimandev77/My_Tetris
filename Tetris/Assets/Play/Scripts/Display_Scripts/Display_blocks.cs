using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Display_blocks : MonoBehaviour
{
    [SerializeField] private GameObject[] Block;
    public int number_block;

    public int score = 0; 
    public TextMeshProUGUI scoreText;

    [Header("Sound")]
    public AudioClip put_block_Sound;
    public AudioClip get_point_Sound;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        Block = new GameObject[childTransforms.Length - 1];
        for (int i = 1; i < childTransforms.Length; i++)
        {
            Block[i - 1] = childTransforms[i].gameObject;
            childTransforms[i].gameObject.SetActive(false);
        }
    }

    public void ActivateRandomObject()
    {
        if (Block.Length > 0)
        {
            Block[number_block].SetActive(false);
            number_block = Random.Range(0, Block.Length);

            Block[number_block].SetActive(true);
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }

    public void Put_Block_Sound()
    {
        audioSource.PlayOneShot(put_block_Sound);
    }

    public void Get_Point_Sound()
    {
        audioSource.PlayOneShot(get_point_Sound);
    }
}
