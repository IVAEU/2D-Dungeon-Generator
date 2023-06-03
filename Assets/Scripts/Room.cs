using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] sps = new SpriteRenderer[4];
    [SerializeField]
    private Sprite[] sprites = new Sprite[8];
    private RoomData roomData;

    public void SetRoom(RoomData data)
    {
        roomData = data;
        for (int i = 0; i < 4; i++)
        {
            if (roomData.isOpened[i] == -1) sps[i].sprite = sprites[i];
            else if (roomData.isOpened[i] == 1) sps[i].sprite = sprites[i + 4];
        }
    }

    public RoomData GetData()
    {
        return roomData;
    }
}
