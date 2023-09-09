using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Border : MonoBehaviour
{   
    [Header("Down")]
    public float raycastDistance = 0.25f;
    public LayerMask layerMask_Stop;

    [Header("Left Right")]
    public float raycastDistance_left_right = 0.25f;

    [Header("Turn")]
    public GameObject[] block_Test;

    private Moving_block moving_Block;

    void Start()
    {
        moving_Block = GetComponent<Moving_block>();

        Transform[] childTransforms = GameObject.Find(gameObject.name + " -Test").GetComponentsInChildren<Transform>();
        block_Test = new GameObject[childTransforms.Length];
        for (int i = 0; i < childTransforms.Length; i++)
        {
            block_Test[i] = childTransforms[i].gameObject;
        }      
    }

    // xoay block
    public void Next_spin(Transform transform)
    {
        bool up = false;

        // block thay thế đến chỗ vị trí block hiện tại
        block_Test[0].transform.position = transform.position;
        block_Test[0].transform.rotation = transform.rotation;

        // Nếu có block bị chạm vào tường --> dịnh sang bên
        if (Check_block_L_to_spin() == true && Check_block_R_to_spin() == false)
            block_Test[0].transform.Translate(Vector2.left * moving_Block.speed_move, Space.World);
        else if (Check_block_R_to_spin() == true && Check_block_L_to_spin() == false)
            block_Test[0].transform.Translate(Vector2.right * moving_Block.speed_move, Space.World);


        // sự di chuyển xoay của block thay thế và phản ứng của nó đối với các block khác
        block_Test[0].transform.Rotate(0, 0, transform.position.z - 90f, Space.World);

        bool is_sticky = false;
        // kiểm tra xem có dính phải block nào ko 
        foreach (GameObject block_1 in block_Test)
        {          
            if (Physics2D.Raycast(block_1.transform.position, Vector2.left, raycastDistance_left_right * 0.5f, layerMask_Stop))
            {
                up = true;

                block_Test[0].transform.position = transform.position;
                block_Test[0].transform.Translate(Vector2.up * moving_Block.speed_Down, Space.World);

                foreach (GameObject block_2 in block_Test)
                    if (Physics2D.Raycast(block_2.transform.position, Vector2.left, raycastDistance_left_right * 0.5f, layerMask_Stop))
                    {
                        is_sticky = true;
                        break;
                    }    

                break;
            }    
        }      
            
        if (!is_sticky)
        {
            transform.position = block_Test[0].transform.position;
            transform.rotation = block_Test[0].transform.rotation;

            if (up == true)
            {
                moving_Block.Call_S_and_D();
            }
        }
    }

    // kiểm tra xem có bị dính vào tường ko
    public void Check_border_block()
    {
        foreach (GameObject block in moving_Block.block_junior)
        {
            // Nếu block dính vào block khác --> dịch lên 1 ô
            if (Physics2D.Raycast(block.transform.position, Vector2.left, raycastDistance_left_right * 0.5f, layerMask_Stop))
                transform.Translate(Vector2.up * moving_Block.speed_Down, Space.World);
        }
    }

    // Kiểm tra xem bước tiếp theo xuống dưới block có vật thể gì ko
    public bool Check_block_Down()
    {
        foreach (GameObject block in moving_Block.block_junior)
            if (Physics2D.Raycast(block.transform.position, Vector2.down, raycastDistance, layerMask_Stop))
                return true;
        return false;
    }

    // Kiểm tra xem bên trái block có vật thể gì ko 
    public bool Check_block_L_to_spin()
    {
        if (Physics2D.Raycast(moving_Block.block_junior[0].transform.position, Vector2.left, raycastDistance_left_right, layerMask_Stop))
            return false;
        return true;
    }

    // Kiểm tra xem bên phải block có vật thể gì ko
    public bool Check_block_R_to_spin()
    {
        if (Physics2D.Raycast(moving_Block.block_junior[0].transform.position, Vector2.right, raycastDistance_left_right, layerMask_Stop))
            return false;
        return true;
    }


    // Kiểm tra xem bên trái block có vật thể gì ko 
    public bool Check_block_L()
    {
        foreach (GameObject block in moving_Block.block_junior)
            if (Physics2D.Raycast(block.transform.position, Vector2.left, raycastDistance_left_right, layerMask_Stop))
                return false;
        return true;
    }

    // Kiểm tra xem bên phải block có vật thể gì ko
    public bool Check_block_R()
    {
        foreach (GameObject block in moving_Block.block_junior)
            if (Physics2D.Raycast(block.transform.position, Vector2.right, raycastDistance_left_right, layerMask_Stop))
                return false;
        return true;
    }
}
