using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerateManager : MonoBehaviour
{
    [Header("Generator")]
    public bool generateRooms; //디버그용 재생성 버튼
    public Vector2Int mapSize; //맵의 크기
    public RoomData[,] map; //맵에 있는 방의 정보
    public GenerateStartPosition startPos; //방이 시작되는 위치
    public int targetRoomCount; //생성할 방들의 갯수
    private int roomCount = 0; //현재 생성한 방 갯수
    [Space]
    [Header("Visualize")]
    public Room roomObj; //방 표시용 프리펩
    private List<Room> createdRooms = new List<Room>(); //생성한 룸 프리펩을 담는 리스트

    private void Update()
    {
        if (generateRooms)
        {
            generateRooms = false;
            StartGenerate();
        }
    }

    public void StartGenerate()
    {
        roomCount = 0;
        if (targetRoomCount > mapSize.x * mapSize.y) targetRoomCount = mapSize.x * mapSize.y;
        map = new RoomData[mapSize.x, mapSize.y];
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                map[i, j] = new RoomData(new Vector2Int(i, j), new int[] { 0, 0, 0, 0 });
            }
        }

        CreateStartRoom();
        if (roomCount < targetRoomCount) StartGenerate();

        BlockUnFinishedRooms();
        DrawRooms();
    }

    private void CreateStartRoom()
    {
        Vector2Int pos = new Vector2Int(mapSize.x / 2, mapSize.y / 2);
        int[] walls = new int[4] {0,0,0,0};
        switch (startPos)
        {
            case GenerateStartPosition.Center:
                pos.x = mapSize.x / 2;
                pos.y = mapSize.y / 2;
                break;
            case GenerateStartPosition.Up:
                pos.x = mapSize.x / 2;
                pos.y = mapSize.y - 1;
                walls[0] = -1;
                break;
            case GenerateStartPosition.Right:
                pos.x = mapSize.x - 1;
                pos.y = mapSize.y / 2;
                walls[1] = -1;
                break;
            case GenerateStartPosition.Down:
                pos.x = mapSize.x / 2;
                pos.y = 0;
                walls[2] = -1;
                break;
            case GenerateStartPosition.Left:
                pos.x = 0;
                pos.y = mapSize.y / 2;
                walls[3] = -1;
                break;
        }

        map[pos.x, pos.y].CreateRandomRoom(walls);
        CreateRoom(map[pos.x, pos.y]);
    } 

    private void CreateRoom(RoomData rootRoom)
    {
        roomCount++;
        if (targetRoomCount == roomCount)
        {
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            if (rootRoom.isOpened[i] == 1)
            {
                Vector2Int newPos = Vector2Int.zero;
                switch (i)
                {
                    case 0:
                        newPos = rootRoom.roomPos + Vector2Int.up;
                        break;
                    case 1:
                        newPos = rootRoom.roomPos + Vector2Int.right;
                        break;
                    case 2:
                        newPos = rootRoom.roomPos + Vector2Int.down;
                        break;
                    case 3:
                        newPos = rootRoom.roomPos + Vector2Int.left;
                        break;
                }

                if (newPos.x >= 0 && newPos.x < mapSize.x && newPos.y >= 0 && newPos.y < mapSize.y)
                {
                    if (!map[newPos.x, newPos.y].isCreated)
                    {
                        map[newPos.x, newPos.y].CreateRandomRoom(GetRefOpened(newPos.x, newPos.y));

                        CreateRoom(map[newPos.x, newPos.y]);
                        if (targetRoomCount == roomCount)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    private int[] GetRefOpened(int x, int y)
    {
        int[] opened = new int[4];
        if (y + 1 >= mapSize.y) opened[0] = -1;
        else opened[0] = map[x, y + 1].isOpened[2];
        if (x + 1 >= mapSize.x) opened[1] = -1;
        else opened[1] = map[x + 1, y].isOpened[3];
        if (y - 1 >= 0) opened[2] = map[x, y - 1].isOpened[0];
        else opened[2] = -1;
        if (x - 1 >= 0) opened[3] = map[x - 1, y].isOpened[1];
        else opened[3] = -1;

        return opened;
    }

    private void BlockUnFinishedRooms()
    {
        int[] tmpOpened = new int[4];
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                tmpOpened = GetRefOpened(i, j);
                for (int k = 0; k < 4; k++)
                {
                    if (map[i, j].isCreated)
                    {
                        if (tmpOpened[k] == 0 && map[i, j].isOpened[k] != -1)
                        {
                            map[i, j].isOpened[k] = -1;
                        }
                    }
                }
            }
        }
    }

    private void DrawRooms()
    {
        foreach (Room room in createdRooms)
        {
            Destroy(room.gameObject);
        }
        createdRooms.Clear();

        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Room room = Instantiate(roomObj, new Vector3(map[i, j].roomPos.x - mapSize.x/2, map[i, j].roomPos.y - mapSize.y/2, 0), Quaternion.identity);
                room.transform.SetParent(transform, false);
                room.SetRoom(map[i, j]);
                createdRooms.Add(room);
            }
        }
    }
}
