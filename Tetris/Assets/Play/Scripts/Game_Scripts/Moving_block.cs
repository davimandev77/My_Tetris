using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(Border))] 
public class Moving_block : MonoBehaviour
{

    [Header("Move down")]
    public float speed_move = 0.5f;
    public float speed_Down = 0.5f;
    public float time_to_Delay;

    [Header("Stop")]
    public bool stop_Game;
    public bool stop;
    public GameObject[] block_junior;

    [Header("Delete")]
    private bool cancelled;
    private int quantity_destroy;

    private Border border;
    private Spawn_block spawn_Block;
    private Display_blocks display_Blocks;
    private Delete_block delete_Block;

    private void Awake()
    {
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        block_junior = new GameObject[childTransforms.Length - 1];
        for (int i = 1; i < childTransforms.Length; i++)
        {
            block_junior[i-1] = childTransforms[i].gameObject;
        }      
    }
    void Start()
    {
        border = GetComponent<Border>();
        spawn_Block = GameObject.Find("Spawn_block").GetComponent<Spawn_block>();
        display_Blocks = GameObject.Find("Display_board").GetComponent<Display_blocks>();
        delete_Block = GameObject.Find("Block_detection").GetComponent<Delete_block>();

        time_to_Delay = spawn_Block.time_to_Stop_Present;
        Invoke("Move_and_stop", 0.1f);     
    }
    void Update()
    {       
        if (!stop)
            move();
    }

    public void Spawn()
    {      
        if (!stop_Game)
        {
            spawn_Block.SpawnObject();
            display_Blocks.ActivateRandomObject();
        }
    }       

    private void move()
    {
        Debug.DrawRay(block_junior[0].transform.position, Vector2.left * border.raycastDistance_left_right, Color.white);
        Debug.DrawRay(block_junior[0].transform.position, Vector2.right * border.raycastDistance_left_right, Color.white);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stop = true;
            Vector2 teleport_Distance = Teleport_Distance();
            transform.Translate(teleport_Distance, Space.World);
            
            Invoke("Call_S_and_D", 0.1f);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && gameObject.name != "Block (6)(Clone)")
            border.Next_spin(transform);  

        if (Input.GetKeyDown(KeyCode.DownArrow) && border.Check_block_Down() == false)
            transform.Translate(Vector2.down * speed_Down, Space.World); 

        if (Input.GetKeyDown(KeyCode.LeftArrow) && border.Check_block_L() == true)
            transform.Translate(Vector2.left * speed_move, Space.World);   
        else if (Input.GetKeyDown(KeyCode.RightArrow) && border.Check_block_R() == true)  
            transform.Translate(Vector2.right * speed_move, Space.World);

        // chuyển tọa độ về số thập phân đầu tiên và làm tròn về dạng bội số của 0.5f
        transform.position = new Vector2(Mathf.Floor(transform.position.x / 0.5f + 0.25f) * 0.5f, transform.position.y);
    }

    private void Move_and_stop()
    {
        if (!stop && border.Check_block_Down() == true)
        {
            Call_S_and_D();
        }
        else if (!stop)
        {
            transform.Translate(Vector2.down * speed_Down, Space.World);
            border.Check_border_block();
            Invoke("Move_and_stop", time_to_Delay);
        }
    }

    public void Call_S_and_D()
    {
        stop = true;
        StartCoroutine(Stop_and_destroy());
    }    

    public IEnumerator Stop_and_destroy()
    {
        display_Blocks.Put_Block_Sound();
        foreach (var item in block_junior)
            if (item.transform.position.y >= 4)
            {
                stop_Game = true;
                spawn_Block.EndGame();
                goto End;
            }


        // tìm kiếm các hàng đã đầy
        List<float> address_Destroy_Y = new List<float>();
        foreach (GameObject jun in block_junior)
        {
            jun.gameObject.layer = 3;
            delete_Block.transform.position = new Vector2(delete_Block.transform.position.x, jun.transform.position.y);

            if (delete_Block.Enough_Counting())
                address_Destroy_Y.Add(delete_Block.transform.position.y);
        }


        // đánh dấy ản hủy lên các hàng đó
        foreach (float add_y in address_Destroy_Y)
        {
            delete_Block.transform.position = new Vector2(delete_Block.transform.position.x, add_y);
            delete_Block.Effect_Detroy();
        }
        yield return new WaitForSeconds(0.2f);
        display_Blocks.Get_Point_Sound();


        // hủy các hàng đó
        foreach (float add_y in address_Destroy_Y)
        {
            delete_Block.transform.position = new Vector2(delete_Block.transform.position.x, add_y);

            delete_Block.Destroy_Block();
            cancelled = true;
            quantity_destroy++;
        }


        // các hàng khác rơi xuống
        if (cancelled)
        {
            int i = 0;
            do
            {
                delete_Block.Move_Block_after_destroy(address_Destroy_Y[i]);
                i++;
            } while (i < quantity_destroy);
        }    

        Invoke("Spawn", 0.5f);
        End:;
    }

    private Vector2 Teleport_Distance()
    {
        float shortestDistance = Mathf.Infinity;

        foreach (var item in block_junior)
        {
            RaycastHit2D hit = Physics2D.Raycast(item.transform.position, Vector2.down, Mathf.Infinity, border.layerMask_Stop);

            float distance = Vector2.Distance(item.transform.position, hit.point);
            if (hit.collider != null && distance < shortestDistance)
                shortestDistance = distance;
        }

        return new Vector2(0, -shortestDistance + border.raycastDistance);
    }

}
