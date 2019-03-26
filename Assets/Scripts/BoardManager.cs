using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : SingletonMonoBehaviour<BoardManager>
{
    //各種変数の定義
    public static Vector2Int boardWidth = new Vector2Int(-8, 8);
    public static Vector2Int boardHeight = new Vector2Int(-8, 8);
    

    public static Vector2[] searchEnemyArray = {new Vector2(0, 1), new Vector2(1,  0), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 1),
                                                new Vector2(1,-1), new Vector2(-1,-1), new Vector2(-1, 1) };
    public static int searchEnemyCount = 0;

    //
    public static GameObject Board;
    public static LayerMask blockingLayer;
    public static LayerMask itemLayer;
    public static LayerMask floorLayer;
    public static LayerMask floorTargetLayer;

    public static List<GameObject> FloorObjectList = new List<GameObject>();
    public static List<GameObject> FloorCheckSpriteList = new List<GameObject>();
    public static List<GameObject> FloorTargetSpriteList = new List<GameObject>();
    public static List<GameObject> FloorRedSpriteList = new List<GameObject>();
    public static List<GameObject> FloorYellowSpriteList = new List<GameObject>();
    public static List<GameObject> FloorGraySpriteList = new List<GameObject>();

    public GameObject[] FloorSpriteArray;
    public GameObject[] OuterWallSpriteArray;
    public GameObject Stair;


    public static RaycastHit2D hit1;
    public static RaycastHit2D hit2;
    public static RaycastHit2D hit3;
    public static Vector2 chosenVector;

    //BoardManagerを生成する準備
    public static BoardManager instance = null;

    //
    public static List<Room> RoomList = new List<Room>();
    public static List<Root> RootList = new List<Root>();
    public static Vector2Int outerRoomDelta = new Vector2Int(60, 60);
    public static Vector2Int innerRoomDelta = new Vector2Int(40, 40);
    private static Vector2Int dd = new Vector2Int((outerRoomDelta - innerRoomDelta).x / 2, (outerRoomDelta - innerRoomDelta).y / 2);

    public static Vector2Int[] pairRoomArray = {new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(1, 3), new Vector2Int(1, 4), new Vector2Int(2, 3),
                                                new Vector2Int(3, 6), new Vector2Int(4, 5), new Vector2Int(4, 6), new Vector2Int(5, 7), new Vector2Int(6, 7)};

    //クラス:ルームの宣言
    public class Room: MonoBehaviour
    {
        public Vector2Int leftBottom = new Vector2Int();
        public Vector2Int rightBottom = new Vector2Int();
        public Vector2Int leftTop = new Vector2Int();
        public Vector2Int rightTop = new Vector2Int();
        public Vector2Int delta = new Vector2Int();
        public int area;

        public List<Vector2> entrancePointList = new List<Vector2>();

        public Vector2Int innerLeftBottom = new Vector2Int();
        public Vector2Int innerRightBottom = new Vector2Int();
        public Vector2Int innerRightTop = new Vector2Int();
        public Vector2Int innerLeftTop = new Vector2Int();
        public Vector2Int innerDelta = new Vector2Int();

    }

    public class Root: MonoBehaviour
    {
        public Vector2Int startPos = new Vector2Int();
        public Vector2Int endPos = new Vector2Int();
        public string direction = "";
        public List<int> roomOnTop = new List<int>();
        public List<int> roomOnRight = new List<int>();
        public List<int> roomOnDown = new List<int>();
        public List<int> roomOnLeft = new List<int>();
    }

    //Awake
    void Awake()
    {
        //boardManagerをシングルトンで生成する
        if(this != Instance)
        {
            Destroy(this);
            return;
        }
    }

    //Start
    void Start()
    {
        Board = GameObject.Find("Board(Clone)");
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        itemLayer = LayerMask.GetMask("ItemLayer");
        floorLayer = LayerMask.GetMask("FloorLayer");
        floorTargetLayer = LayerMask.GetMask("FloorTargetLayer");
    }

    public static Room SetNewRoom(Vector2Int leftBottom, Vector2Int delta)
    {
        Room room = Board.AddComponent<Room>();
        room.leftBottom = leftBottom;
        room.rightBottom = leftBottom + delta * Vector2Int.right;
        room.leftTop = leftBottom + delta * Vector2Int.up;
        room.rightTop = leftBottom + delta;
        room.delta = delta;
        room.area = delta.x * delta.y;

        room.innerLeftBottom = leftBottom + new Vector2Int(Random.Range(2, 4), Random.Range(2, 4));
        room.innerRightTop = leftBottom + delta - new Vector2Int(Random.Range(3, 5), Random.Range(3, 5));
        room.innerRightBottom = new Vector2Int(room.innerRightTop.x, room.innerLeftBottom.y);
        room.innerLeftTop = new Vector2Int(room.innerLeftBottom.x, room.innerRightTop.y);
        room.innerDelta = room.innerRightTop - room.innerLeftBottom;

        return room;
    }

    public static Root SetNewRoot(Vector2Int startPos, Vector2Int endPos, string direction)
    {
        Root root = Board.AddComponent<Root>();
        root.startPos = startPos;
        root.endPos = endPos;
        root.direction = direction;
        return root;
    }

    //
    public static bool DevideRoom(int roomNum, string splitWay)
    {
        Room room = RoomList[roomNum];
        Room newRoom1;
        Room newRoom2;
        Root newRoot;
        Vector2Int newDelta = new Vector2Int();
        float multiplier = Random.Range(0.38f, 0.62f);

        if(splitWay == "Vertical" || (splitWay == "Random" && Random.Range(0,2) == 0) )
        {
            newDelta = new Vector2Int((int) (room.delta.x * multiplier), room.delta.y);
            newRoom1 = SetNewRoom(room.leftBottom, newDelta);
            newRoom2 = SetNewRoom(room.leftBottom + new Vector2Int(newDelta.x, 0), room.delta - new Vector2Int(newDelta.x, 0));
            newRoot = SetNewRoot(room.leftBottom + newDelta * Vector2Int.right - Vector2Int.one, room.leftBottom + newDelta - Vector2Int.one, "Vertical");
        }
        else
        {
            newDelta = new Vector2Int(room.delta.x, (int) (room.delta.y * multiplier));
            newRoom1 = SetNewRoom(room.leftBottom, newDelta);
            newRoom2 = SetNewRoom(room.leftBottom + new Vector2Int(0, newDelta.y), room.delta - new Vector2Int(0, newDelta.y));
            newRoot = SetNewRoot(room.leftBottom + newDelta * Vector2Int.up - Vector2Int.one, room.leftBottom + newDelta - Vector2Int.one, "Horizontal");
        }

        int min =5;
        if(newRoom1.innerDelta.x < min || newRoom1.innerDelta.y < min || newRoom2.innerDelta.x < min || newRoom2.innerDelta.y < min)
        {
            return false;
        }
        else
        {
            RoomList.Add(newRoom1);
            RoomList.Add(newRoom2);
            RoomList.RemoveAt(roomNum);
            RootList.Add(newRoot);
        }

        //RoomList.RemoveAt(roomNum);
        return true;
    }

    public void SetupBoard()
    {
        RoomList.Clear();
        RootList.Clear();
        FloorCheckSpriteList.Clear();
        FloorTargetSpriteList.Clear();
        FloorRedSpriteList.Clear();
        FloorYellowSpriteList.Clear();
        FloorGraySpriteList.Clear();

        //部屋を適当に分割する
        RoomList.Add(SetNewRoom(Vector2Int.zero, innerRoomDelta));


        int devideTimes = Random.Range(5, 8);
        int loopLimits = 30;
        while(devideTimes > 0 && loopLimits > 0)
        {
            loopLimits--;
            int roomNum = Random.Range(0, RoomList.Count);
            if (DevideRoom(roomNum, "Random") == true)
            {
                devideTimes--;
            }
        }

        //床を生成する
        for (int n = 0; n < RoomList.Count; n++)
        {
            Room r = RoomList[n];
            Vector2Int d = outerRoomDelta - innerRoomDelta;
            for (int i = r.innerLeftBottom.x + d.x / 2; i < r.innerRightTop.x + d.x / 2; i++)
            {
                for (int j = r.innerLeftBottom.y + d.y / 2; j < r.innerRightTop.y + d.y / 2; j++)
                {
                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];

                    //Instance関数で床を生成する
                    GameObject instance = Instantiate(chosenSprite, new Vector3(i, j, n), Quaternion.identity);
                    instance.transform.SetParent(transform);
                }
            }
        }

        //通路を生成する
        for (int n = 0; n < RootList.Count; n++)
        {
            Root thisRoot = RootList[n];
            Vector2Int d = outerRoomDelta - innerRoomDelta;

            if(thisRoot.direction == "Vertical")
            {
                for(int j = thisRoot.startPos.y+1; j < thisRoot.endPos.y; j++)
                {
                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(thisRoot.startPos.x + d.x / 2, j + d.y / 2, -n-1), Quaternion.identity);
                    instance.transform.SetParent(transform);
                }
            }
            else
            {
                for(int j = thisRoot.startPos.x+1; j < thisRoot.endPos.x; j++)
                {
                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(j + d.x / 2,thisRoot.startPos.y + d.y / 2, -n-1), Quaternion.identity);
                    instance.transform.SetParent(transform);
                }
            }
        }

        //各部屋がどの通路と接しているかを調べ、関係性を通路の子要素として保存する
        for (int n = 0; n < RoomList.Count; n++)
        {
            Room thisRoom = RoomList[n];
            Vector2Int currentPosition = thisRoom.rightTop - (Vector2Int.one + Vector2Int.one) + dd;
            
            if(ExistLayer(currentPosition + Vector2Int.up, "FloorLayer", out hit1) == true)
            {
                int rootNum = (int) GetRaycastHit2D(currentPosition + Vector2Int.up, "FloorLayer").collider.gameObject.transform.position.z;
                RootList[-rootNum - 1].roomOnDown.Add(n);
            }
            if (ExistLayer(currentPosition + Vector2Int.right, "FloorLayer", out hit1) == true)
            {
                int rootNum = (int)GetRaycastHit2D(currentPosition + Vector2Int.right, "FloorLayer").collider.gameObject.transform.position.z;
                RootList[-rootNum - 1].roomOnLeft.Add(n);
            }

            currentPosition = thisRoom.leftBottom + dd;
            if(ExistLayer(currentPosition + Vector2Int.down, "FloorLayer", out hit1) == true)
            {
                int roomNum = (int)GetRaycastHit2D(currentPosition + Vector2Int.down, "FloorLayer").collider.gameObject.transform.position.z;
                RootList[-roomNum - 1].roomOnTop.Add(n);
            }
            if(ExistLayer(currentPosition + Vector2Int.left, "FloorLayer", out hit1) == true)
            {
                int roomNum = (int)GetRaycastHit2D(currentPosition + Vector2Int.left, "FloorLayer").collider.gameObject.transform.position.z;
                RootList[-roomNum - 1].roomOnRight.Add(n);
            }
        }

        //各通路について、通路をつなげる部屋の番号を決め、部屋から通路へ通路を伸ばす
        for (int n = 0; n < RootList.Count; n++)
        {
            Root thisRoot = RootList[n];
            //上方向
            int rootAmountMax = thisRoot.roomOnTop.Count;
            if(rootAmountMax > 0)
            {
                //int rootAmount = Mathf.Min(Random.Range(1, rootAmountMax + 1), 1);
                int rootAmount = 1;
                for (int m = 0; m < rootAmount; m++)
                {
                    //ここで通路をつなげる部屋の番号を決める
                    int roomNumIndex = Random.Range(0, thisRoot.roomOnTop.Count);
                    Room thisRoom = RoomList[thisRoot.roomOnTop[roomNumIndex]];
                    //部屋から通路へ通路を伸ばすにあたり、部屋の出口を決める
                    Vector2Int currentPosition = new Vector2Int(Random.Range(thisRoom.innerLeftBottom.x, thisRoom.innerRightBottom.x), thisRoom.innerLeftBottom.y) + Vector2Int.down + dd;

                    //
                    thisRoom.entrancePointList.Add(currentPosition);

                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, thisRoot.roomOnTop[roomNumIndex]), Quaternion.identity);
                    instance.transform.SetParent(transform);

                    currentPosition = currentPosition + Vector2Int.down;
                    while (ExistLayer(currentPosition, "FloorLayer", out hit1) == false)
                    {
                        chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                        instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, -n - 1), Quaternion.identity);
                        instance.transform.SetParent(transform);

                        currentPosition = currentPosition + Vector2Int.down;
                    }
                    thisRoot.roomOnTop.RemoveAt(roomNumIndex);
                }
            }
            //右方向
            rootAmountMax = thisRoot.roomOnRight.Count;
            if (rootAmountMax > 0)
            {
                //int rootAmount = Mathf.Min(Random.Range(1, rootAmountMax + 1), 1);
                int rootAmount = 1;
                for (int m = 0; m < rootAmount; m++)
                {
                    int roomNumIndex = Random.Range(0, thisRoot.roomOnRight.Count);
                    Room thisRoom = RoomList[thisRoot.roomOnRight[roomNumIndex]];
                    Vector2Int currentPosition = new Vector2Int(thisRoom.innerLeftBottom.x, Random.Range(thisRoom.innerLeftBottom.y, thisRoom.innerLeftTop.y)) + Vector2Int.left + dd;
                    //
                    thisRoom.entrancePointList.Add(currentPosition);

                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, thisRoot.roomOnRight[roomNumIndex]), Quaternion.identity);
                    instance.transform.SetParent(transform);

                    currentPosition = currentPosition + Vector2Int.left;
                    while (ExistLayer(currentPosition, "FloorLayer", out hit1) == false)
                    {
                        chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                        instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, -n - 1), Quaternion.identity);
                        instance.transform.SetParent(transform);

                        currentPosition = currentPosition + Vector2Int.left;
                    }
                    thisRoot.roomOnRight.RemoveAt(roomNumIndex);
                }
            }
            //下方向
            rootAmountMax = thisRoot.roomOnDown.Count;
            if(rootAmountMax > 0)
            {
                //int rootAmount = Mathf.Min(Random.Range(1, rootAmountMax + 1), 1);
                int rootAmount = 1;
                for (int m = 0; m < rootAmount; m++)
                {
                    int roomNumIndex = Random.Range(0, thisRoot.roomOnDown.Count);
                    Room thisRoom = RoomList[thisRoot.roomOnDown[roomNumIndex]];
                    Vector2Int currentPosition = new Vector2Int(Random.Range(thisRoom.innerLeftTop.x, thisRoom.innerRightTop.x), thisRoom.innerLeftTop.y) + Vector2Int.zero + dd;
                    //
                    thisRoom.entrancePointList.Add(currentPosition);

                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, thisRoot.roomOnDown[roomNumIndex]), Quaternion.identity);
                    instance.transform.SetParent(transform);

                    currentPosition = currentPosition + Vector2Int.up;
                    while (ExistLayer(currentPosition, "FloorLayer", out hit1) == false)
                    {
                        chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                        instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, -n - 1), Quaternion.identity);
                        instance.transform.SetParent(transform);

                        currentPosition = currentPosition + Vector2Int.up;
                    }
                    thisRoot.roomOnDown.RemoveAt(roomNumIndex);
                }
            }
            //左方向
            rootAmountMax = thisRoot.roomOnLeft.Count;
            if (rootAmountMax > 0)
            {
                //int rootAmount = Mathf.Min(Random.Range(1, rootAmountMax + 1), 1);
                int rootAmount = 1;
                for (int m = 0; m < rootAmount; m++)
                {
                    int roomNumIndex = Random.Range(0, thisRoot.roomOnLeft.Count);
                    Room thisRoom = RoomList[thisRoot.roomOnLeft[roomNumIndex]];
                    Vector2Int currentPosition = new Vector2Int(thisRoom.innerRightBottom.x, Random.Range(thisRoom.innerRightBottom.y, thisRoom.innerRightTop.y)) + dd;
                    //
                    thisRoom.entrancePointList.Add(currentPosition);

                    GameObject chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                    GameObject instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, thisRoot.roomOnLeft[roomNumIndex]), Quaternion.identity);
                    instance.transform.SetParent(transform);

                    currentPosition = currentPosition + Vector2Int.right;
                    while (ExistLayer(currentPosition, "FloorLayer", out hit1) == false)
                    {
                        chosenSprite = FloorSpriteArray[Random.Range(0, FloorSpriteArray.Length)];
                        instance = Instantiate(chosenSprite, new Vector3(currentPosition.x, currentPosition.y, -n - 1), Quaternion.identity);
                        instance.transform.SetParent(transform);

                        currentPosition = currentPosition + Vector2Int.right;
                    }
                    thisRoot.roomOnLeft.RemoveAt(roomNumIndex);
                }
            }
        }
        //無駄な通路を削除する
        for (int n = 0; n < RootList.Count; n++)
        {
            Root thisRoot = RootList[n];
            Vector2Int currentPosition = thisRoot.startPos + dd;
            if(thisRoot.direction == "Vertical")
            {
                while(ExistLayer(currentPosition + Vector2Int.right, "FloorLayer", out hit1) == false && ExistLayer(currentPosition + Vector2Int.left, "FloorLayer", out hit1) == false)
                {
                    if(ExistLayer(currentPosition, "FloorLayer", out hit1) == true)
                    {
                        GameObject thisGameObject = GetRaycastHit2D(currentPosition, "FloorLayer").collider.gameObject;
                        thisGameObject.tag = "DeadObject";
                        Destroy(thisGameObject);
                    }
                    currentPosition = currentPosition + Vector2Int.up;
                }
                currentPosition = thisRoot.endPos + dd;
                while (ExistLayer(currentPosition + Vector2Int.right, "FloorLayer", out hit1) == false && ExistLayer(currentPosition + Vector2Int.left, "FloorLayer", out hit1) == false)
                {
                    if (ExistLayer(currentPosition, "FloorLayer", out hit1) == true)
                    {
                        GameObject thisGameObject = GetRaycastHit2D(currentPosition, "FloorLayer").collider.gameObject;
                        thisGameObject.tag = "DeadObject";
                        Destroy(thisGameObject);
                    }
                    currentPosition = currentPosition + Vector2Int.down;
                }
            }
            else
            {
                while (ExistLayer(currentPosition + Vector2Int.up, "FloorLayer", out hit1) == false && ExistLayer(currentPosition + Vector2Int.down, "FloorLayer", out hit1) == false)
                {
                    if (ExistLayer(currentPosition, "FloorLayer", out hit1) == true)
                    {
                        GameObject thisGameObject = GetRaycastHit2D(currentPosition, "FloorLayer").collider.gameObject;
                        thisGameObject.tag = "DeadObject";
                        Destroy(thisGameObject);
                    }
                    currentPosition = currentPosition + Vector2Int.right;
                }
                currentPosition = thisRoot.endPos + dd;
                while (ExistLayer(currentPosition + Vector2Int.up, "FloorLayer", out hit1) == false && ExistLayer(currentPosition + Vector2Int.down, "FloorLayer", out hit1) == false)
                {
                    if (ExistLayer(currentPosition, "FloorLayer", out hit1) == true)
                    {
                        GameObject thisGameObject = GetRaycastHit2D(currentPosition, "FloorLayer").collider.gameObject;
                        thisGameObject.tag = "DeadObject";
                        Destroy(thisGameObject);
                    }
                    currentPosition = currentPosition + Vector2Int.left;
                }

            }
        }

        //部屋の入り口を１マス分拡張する？
        for (int n = 0; n < RoomList.Count; n++)
        {
            Room r = RoomList[n];
            for(int m = 0; m < r.entrancePointList.Count; m++)
            {
                Vector3 changePosition = GetRaycastHit2D(r.entrancePointList[m], "FloorLayer").collider.gameObject.transform.position;
                GetRaycastHit2D(r.entrancePointList[m], "FloorLayer").collider.gameObject.transform.position = new Vector3(changePosition.x, changePosition.y, n);
            }
        }

        RaycastHit2D hit;
        bool b;
        //外壁を生成する
        for (int i = 0; i < outerRoomDelta.x; i++)
        {
            for (int j = 0; j < outerRoomDelta.y; j++)
            {
                b = BoardManager.ExistLayer(new Vector2(i, j), "FloorLayer", out hit);
                if (b == false)
                {
                    //外壁スプライトの中からランダムでフロアを選択
                    GameObject chosenSprite = OuterWallSpriteArray[Random.Range(0, OuterWallSpriteArray.Length)];

                    //Instance関数で外壁を生成する
                    GameObject instance = Instantiate(chosenSprite, new Vector3(i, j, 0f), Quaternion.identity);
                    instance.transform.SetParent(transform);
                }
            }
        }
        FloorObjectList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Floor"));
        for (int i = 0; i < FloorObjectList.Count; i++)
        {
            FloorCheckSpriteList.Add(FloorObjectList[i].transform.Find("floor_check_white").gameObject);
            FloorTargetSpriteList.Add(FloorObjectList[i].transform.Find("floor_target_white").gameObject);
            FloorRedSpriteList.Add(FloorObjectList[i].transform.Find("floor_red").gameObject);
            FloorYellowSpriteList.Add(FloorObjectList[i].transform.Find("floor_yellow").gameObject);
            FloorGraySpriteList.Add(FloorObjectList[i].transform.Find("floor_gray").gameObject);
        }
    }

    public static Vector2 ChooseRandomFloor()
    {
        int randomIndex = Random.Range(0, FloorObjectList.Count);
        return (Vector2) FloorObjectList[randomIndex].transform.position;
    }

    public static Vector2 ChooseRandomValidFloor()
    {
        int loopCount = 0;
        do
        {
            chosenVector = ChooseRandomFloor();
            hit1 = Physics2D.Linecast(chosenVector + new Vector2(0.48f, 0.48f), chosenVector, blockingLayer);
            hit2 = Physics2D.Linecast(chosenVector + new Vector2(0.48f, 0.48f), chosenVector, itemLayer);
            hit3 = Physics2D.Linecast(chosenVector + new Vector2(0.48f, 0.48f), chosenVector, floorLayer);
            
            if(loopCount > 1000)
            {
                Debug.LogError("MyError:BoardManager.ChooseRandomValidFloor");
                break;
            }
        } while (hit1.transform != null || hit2.transform != null || (hit3.transform != null && Mathf.Round(hit3.collider.gameObject.transform.position.z) < 0));

        return chosenVector;
    }

    public static bool ExistLayer(Vector2 targetPosition, string layerName, out RaycastHit2D hit)
    {
        LayerMask layerMask = new LayerMask();

        if(layerName == "BlockingLayer")
        {
            layerMask = blockingLayer;
        }
        else if(layerName == "ItemLayer")
        {
            layerMask = itemLayer;
        }
        else if(layerName == "FloorLayer")
        {
            layerMask = floorLayer;
        }
        else if(layerName == "FloorTargetLayer")
        {
            layerMask = floorTargetLayer;
        }
        else
        {
            Debug.LogError("MyError:BoardManager.ExistLayer");
        }

        hit = Physics2D.Linecast(targetPosition, targetPosition + new Vector2(0.48f, 0.48f), layerMask);
        if (hit.transform == null) return false;
        return true;
    }

    public static RaycastHit2D GetRaycastHit2D(Vector2 targetPosition, string layerName)
    {
        LayerMask layerMask = new LayerMask();

        if (layerName == "BlockingLayer")
        {
            layerMask = blockingLayer;
        }
        else if (layerName == "ItemLayer")
        {
            layerMask = itemLayer;
        }
        else if (layerName == "FloorLayer")
        {
            layerMask = floorLayer;
        }
        else if (layerName == "FloorTargetLayer")
        {
            layerMask = floorTargetLayer;
        }
        else
        {
            Debug.LogError("MyError:BoardManager.ExistLayer");
        }

        return Physics2D.Linecast(targetPosition, targetPosition + new Vector2(0.48f, 0.48f), layerMask);
    }
}
