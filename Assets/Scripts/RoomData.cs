using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData
{
    public bool isCreated = false;
    public Vector2Int roomPos;
    public int[] isOpened; //1: 통로, 0 : 없음, -1 : 벽

    public RoomData(Vector2Int pos, int[] isRefOpened)
    {
        isCreated = false;
        isOpened = new int[4] { 0, 0, 0, 0 };
        roomPos = pos;
        for (int i = 0; i < 4; i++)
        {
            isOpened[i] = isRefOpened[i];
        }
    }

    public void CreateRandomRoom(int[] isRefOpened)
    {
        isCreated = true;
        for (int i = 0; i < 4; i++)
        {
            if (isRefOpened[i] == 0)
            {
                isOpened[i] = Random.Range(0, 2) == 0 ? -1 : 1;
            }
            else
            {
                isOpened[i] = isRefOpened[i];
            }
        }
    }
}
