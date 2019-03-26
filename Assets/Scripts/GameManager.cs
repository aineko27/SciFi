using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    //フレームカウントを宣言、各フレームごとにインクリメントする
    [Watch] public static int frameCount = 0;
    public static int turnCount = 0;
    public static int floorLevel = 0;

    //キー入力が保存されているファイルの宣言
    private static StreamReader streamReader;

    //使用するプレハブの宣言(Inspectorで取得する)
    public GameObject Enemy;
    public GameObject Item;
    public GameObject Stair;
    public static GameObject EnemyPrefab;
    public static GameObject ItemPrefab;
    public static GameObject StairPrefab;

    public static GameObject EnemyObject;
    public static GameObject ItemObject;

    public static LayerMask blockingLayer;
    public static LayerMask itemLayer;

    //ランダムシードを宣言する
    private static int randomSeed;

    //
    public static bool isNextFloor = false;

    //ミニマップの透明度
    private static float miniMapSpriteAlpha = 0.9f;
    
    //が移動中であるユニットを数える整数値の宣言
    public static int movingObjectCount = 0;

    //アニメーションが作動しているかを示す真偽値の宣言
    public static int runningAnimationCount = 0;

    //
    public static bool activateComfirmWindow;

    //メッセージテキストがスタックしているかを示す真偽値の宣言
    public static bool messageIsStacking = false;

    //コマンドウィンドウが表示されているかを示す真偽値の宣言
    public static bool commandWindowIsDisplaying = false;

    //現在表示されているコマンドウィンドウの列挙体と変数の宣言
    public enum CommandWindowState
    {
        Window1
    }
    public static CommandWindowState commandWindowState;

    //キー入力が制限されているかを示す真偽値の宣言
    public static int keyInputRestrictionCount = 0;

    //gameStateの列挙体と変数の宣言
    public enum GameState
    {
        TitleWindow,
        InDungeon
    }
    private static GameState gameState;

    //turnStateの列挙体と変数の宣言
    public enum TurnState
    {
        DecidePlayerBehaviour,
        DecideEnemyBehaviour,
        StartUnitMovement,
        StartPlayerAttack,
        AfterPlayerAttack,
        WaitPlayerAttackComplete,
        WaitMovementComplete,
    }
    [Watch] public static TurnState turnState;

    //playStateの列挙体と変数の宣言
    public enum PlayState
    {
        Neutral,
        Save,
        Replay
    }
    public PlayState playState;

    //GameManagerをシングルトンで生成する準備
    public static GameManager instance = null;

    //Awake
    void Awake()
    {
        //GameManagerをシングルトンで生成する
        if (this != Instance){
            Destroy(this);
            return;
        }

        //シーン遷移時にこのObjectを受け継ぐ
        DontDestroyOnLoad(gameObject);
    }
    
    //Start
    void Start()
    {
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        itemLayer = LayerMask.GetMask("ItemLayer");

        //ファイル読み込み、書き込みの設定
        //もしプレイステートがSaveの場合
        if (playState == PlayState.Save)
        {
            //まずは既存のInputtedKeyData.txtファイルを消去する
            System.IO.File.Delete("./InputtedKeyData.txt");

            //ランダムシードを設定し、Random.InitState関数に代入し実行する
            randomSeed = UnityEngine.Random.Range(0, 10000);
            //randomSeed = 42;
            UnityEngine.Random.InitState(randomSeed);

            //InputtedKeyData.txtを開き、設定したランダムシードを１行目に書き込み、ファイルを閉じる
            StreamWriter streamWriter = new StreamWriter("./InputtedKeyData.txt", true);
            streamWriter.WriteLine(randomSeed.ToString());
            streamWriter.Flush();
            streamWriter.Close();

        }
        //もしプレイステートがReplayの場合
        else if (playState == PlayState.Replay)
        {
            //InputtedKeyData.txtを開き、ファイルからランダムシードを読み込み、InitState関数に代入し実行する
            streamReader = new StreamReader("./InputtedKeyData.txt");
            randomSeed = int.Parse(streamReader.ReadLine());
            UnityEngine.Random.InitState(randomSeed);
        }

        EnemyPrefab = Enemy;
        ItemPrefab = Item;
        StairPrefab = Stair;

        EnemyObject = new GameObject("EnemyObject");
        ItemObject = new GameObject("ItemObject");

        isNextFloor = true;

        //ゲーム開始
        InitGame();
    }

    //ゲーム開始起動時に処理する関数
    private void InitGame()
    {
        //turnStateをタイトルウィンドウにする
        gameState = GameState.TitleWindow;

        //自機ユニットのステータスを設定し、自機ユニットををユニットリストの0番目に加える
        //PlayerManager.SetUnitStatus();
        Unit.unitList.Add(PlayerManager.instance.GetComponent<PlayerManager>());
        Unit.unitList[0].hitPointMax = 20;
        Unit.unitList[0].hitPoint = 20;

        //
        floorLevel = 1;
        
        //ダンジョンをセットアップする
        //SetupNewFloor(floorLevel, UnityEngine.Random.Range(4,7), UnityEngine.Random.Range(4, 7));
    }

    public static void CleanupAllObject()
    {
        foreach (Transform n in GameObject.Find("Board(Clone)").transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (Transform n in EnemyObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        for(int i = Unit.unitList.Count - 1; i > 0; i--)
        {
            Unit.unitList.RemoveAt(i);
        }
        foreach (Transform n in ItemObject.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        GameObject.Destroy(GameObject.Find("Stair(Clone)").gameObject);
    }

    //ダンジョンをセットアップする関数
    public static void SetupNewFloor(int floorLevel, int enemyAmount, int itemAmount)
    {
        //各種ステートの更新
        gameState = GameState.InDungeon;
        turnState = TurnState.DecidePlayerBehaviour;

        //ボードのセットアップ
        BoardManager.Instance.SetupBoard();

        // 
        Unit.unitList[0].DeployUnit();

        //階段の配置
        GameObject stairInstance = Instantiate(StairPrefab);
        stairInstance.transform.position = BoardManager.ChooseRandomValidFloor();

        //敵の配置
        for (int i = 0; i < enemyAmount; i++)
        {
            GameObject enemyInstance = Instantiate(EnemyPrefab);
            enemyInstance.GetComponent<Unit>().SetUnitStatus(EnemyManager.UnitName.EliteSkelton);
            enemyInstance.GetComponent<Unit>().DeployUnit();
            enemyInstance.transform.SetParent(EnemyObject.transform);
        }

        //アイテムの配置
        for (int i = 0; i < itemAmount; i++)
        {
            GameObject itemInstance = Instantiate(ItemPrefab, BoardManager.ChooseRandomValidFloor(), Quaternion.identity);
            itemInstance.transform.SetParent(ItemObject.transform);
        }
    }

    //ゲームオーバー時の処理
    public static void GameOver()
    {

    }

    void Update()
    {
        //Debug.Log(111111);
        ////Debug.Log(Input.GetKey(KeyCode.UpArrow));
        ////Debug.Log(Input.GetKey(KeyCode.RightArrow));
        ////Debug.Log(Input.GetKey(KeyCode.DownArrow));
        ////Debug.Log(Input.GetKey(KeyCode.LeftArrow));
        ////Debug.Log(Input.GetAxisRaw("Horizontal"));
        ////Debug.Log(Input.GetAxisRaw("Vertical")); if (Input.anyKeyDown)
        //foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        //{
        //    if (Input.GetKey(code))
        //    {
        //        Debug.Log(code);
        //    }
        //}

        //return;

        //frameCountのインクリメント
        frameCount++;

        if(isNextFloor == true)
        {
            SetupNewFloor(floorLevel, UnityEngine.Random.Range(4, 7), UnityEngine.Random.Range(4, 7));
            isNextFloor = false;
        }

        if(CanvasController.LevelWindow.activeSelf == true)
        {
            if(keyInputRestrictionCount > 0)
            {
                keyInputRestrictionCount--;
                return;
            }
            CanvasController.LevelWindow.SetActive(false);
        }

        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS【開始】デバッグ関連のシステム処理【開始】SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS
        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】デバッグ用の処理sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss
        //プレイステートがSaveの場合、押されているキーコードをInputtedKeyData.txtファイルに書き込む
        if (playState == PlayState.Save)
        {
            InputFunction.WriteInputtedKey();
        }
        //プレイステートがReadの場合、InputtedKeyData.txtファイルから保存されたキーコードを読み込む
        else if (playState == PlayState.Replay)
        {
            InputFunction.ReadInputtedKey(streamReader);
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】デバッグ用の処理eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        //EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE【終了】デバッグ関連のシステム処理【終了】EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
        


        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS【開始】ターン前の演出・操作関連の処理【開始】SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS

        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】InfoWindowの情報更新sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss

        //HpInfoの情報更新
        Unit.unitList[0].hitPointMax = Math.Min(Unit.unitList[0].hitPointMax, Unit.unitList[0].hitPointMaxLimit);
        Unit.unitList[0].hitPoint = Math.Min(Unit.unitList[0].hitPoint, Unit.unitList[0].hitPointMax);

        float HpSliderWidth = 70 * Mathf.Log(Unit.unitList[0].hitPointMax, 20);
        HpSliderWidth = Math.Max(Math.Min(HpSliderWidth, 140), 70);
        CanvasController.HpInfoSliderRect.sizeDelta = new Vector2(HpSliderWidth, 20);
        CanvasController.HpInfoSliderSlider.value = 1.0f * Unit.unitList[0].hitPoint / Unit.unitList[0].hitPointMax;

        string HpText = String.Format("{0, 3}", Unit.unitList[0].hitPoint);
        string HpTextMax = String.Format("{0, 3}", Unit.unitList[0].hitPointMax);
        CanvasController.HpInfoLavelText.text = HpText + "/" + HpTextMax;

        //Scrapの情報更新
        Unit.unitList[0].ownedScrap = Math.Min(Unit.unitList[0].ownedScrap, Unit.unitList[0].ownedScrapLimit);
        CanvasController.ScrapInfoLavelText.text = String.Format("{0, 5}", Unit.unitList[0].ownedScrap);

        //MiniMapSpriteRendererの情報更新
        if (true)
        {
            if (InputFunction.GetKey("Space"))
            {
                //MiniMap(Unit)
                for (int i = 0; i < Unit.unitList.Count; i++)
                {
                    Unit.unitList[i].transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }
                //MiniMap(Floor)
                for (int i = 0; i < BoardManager.FloorObjectList.Count; i++)
                {
                    BoardManager.FloorObjectList[i].transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }

                //MiniMap(Item)
                Transform itemTransform = GameObject.Find("ItemObject").transform;
                foreach (Transform child in itemTransform.transform)
                {
                    child.gameObject.transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }

                //MiniMap(Stair)
                GameObject.Find("Stair(Clone)").transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                //MiniMap(Unit)
                for (int i = 0; i < Unit.unitList.Count; i++)
                {
                    Unit.unitList[i].transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, miniMapSpriteAlpha);
                }
                //MiniMap(Floor)
                for (int i = 0; i < BoardManager.FloorObjectList.Count; i++)
                {
                    BoardManager.FloorObjectList[i].transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, miniMapSpriteAlpha);
                }

                //MiniMap(Item)
                Transform itemTransform = GameObject.Find("ItemObject").transform;
                foreach (Transform child in itemTransform.transform)
                {
                    child.gameObject.transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, miniMapSpriteAlpha);
                }

                //MiniMap(Stair)
                GameObject.Find("Stair(Clone)").transform.Find("MiniMapTile").GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, miniMapSpriteAlpha);
            }
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】InfoWindowの情報更新eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee


        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】ユニットが移動中の処理sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss
        //ユニット移動中真偽値が真である場合
        if (movingObjectCount > 0)
        {
            return;
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】ユニットが移動中の処理eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee


        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】アニメーションが作動している場合sssssssssssssssssssssssssssssssssssssssssssssssssssss
        //アニメーション再生中真偽値が真である場合
        if (runningAnimationCount > 0)
        {
            return;
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】アニメーションが作動している場合eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee


        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】コンフィルムウィンドウが表示されている場合ssssssssssssssssssssssssssssssssssssssssssssss

        if(activateComfirmWindow == true)
        {
            //メッセージテキストを更新し表示
            CanvasController.DisplayMessageText();
            CanvasController.ComfirmWindow.SetActive(true);

            activateComfirmWindow = false;
        }
        if(CanvasController.ComfirmWindow.activeSelf == true)
        {
            CanvasController.CommandComfirmWindow();
            return;
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】コンフィルムウィンドウが表示されている場合eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee


        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】テキストウィンドウが表示されている場合sssssssssssssssssssssssssssssssssssssssssssssss
        //メッセージ蓄積真偽値がもし真である場合、メッセージを表示する
        if (messageIsStacking == true)
        {
            //まだメッセージウィンドウが表示されていない場合(又は)zキーが押された場合、メッセージテキストを更新し表示する
            if (CanvasController.MessageWindow.activeSelf == false || InputFunction.GetKeyDown("A") == true || turnState == TurnState.DecidePlayerBehaviour)

            {
                //メッセージテキストを更新し表示
                CanvasController.DisplayMessageText();

                //もしこれ以上メッセージの蓄積がない場合、キー入力制限真偽値を真にし、遅延処理後にメッセージウィンドウを非表示にする
                if (messageIsStacking == false)
                {
                    keyInputRestrictionCount += 0;
                    StartCoroutine(CanvasController.SetWindowInactiveWithDelay(90));
                }
                return;
            }
            //zキーが押されない場合、ターン処理をここで打ち切る
            else
            {
                return;
            }
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】テキストウィンドウが表示されている場合eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee


        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】キー入力制限がかかっている場合sssssssssssssssssssssssssssssssssssssssssssssssssssssss
        //キー入力制限カウントが0より大きかった場合、カウントをディクリメントし、ターン処理をここで打ち切る
        if (keyInputRestrictionCount > 0)
        {
            if (InputFunction.AnyKey() == false) keyInputRestrictionCount = 0;
            keyInputRestrictionCount--;
            return;
        } 
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】キー入力制限がかかっている場合eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        
        //EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE【終了】ターン前の演出・操作関連の処理【終了】EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE



        //SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS【開始】ターン処理【開始】SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS

        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】自機ユニットの振る舞い予定を決定するsssssssssssssssssssssssssssssssssssssssssssssssss

        //現在のゲームステートによってキー受付を変える
        //ゲームステートが自機ユニットの振る舞い前だったら
        if (turnState == TurnState.DecidePlayerBehaviour)
        {
            //自機ユニットの振る舞い予定を決める
            Unit.unitList[0].DecideBehaviour();

            //もし自機ユニットが行動を決定した場合、turnCountをインクリメントする
            if (Unit.unitList[0].scheduledBehaviour != PlayerManager.ScheduledBehaviour.NotDecided)
            {
                //turnCountのインクリメント
                turnCount++;
            
                //もし自機ユニットの振る舞い予定が移動だった場合、turnStateをDecideEnemyBehaveに遷移させる
                if (Unit.unitList[0].scheduledBehaviour == PlayerManager.ScheduledBehaviour.Move)
                {
                    turnState = TurnState.DecideEnemyBehaviour;
                }
                //もし自機ユニットの振る舞い予定が攻撃だった場合、turnStateをDecideEnemyBehave(に遷移させる
                else
                {
                    turnState = TurnState.StartPlayerAttack;
                }
            }
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】自機ユニットの振る舞い予定を決定するeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee

        
        //ssssssssssssssssssssssssssssssssssssssssssssssss【始】自機ユニットの振る舞い予定に従いターン処理を場合分けするsssssssssssssssssssssssssssss
        //もし自機ユニットが振る舞い予定を決定していた場合
        if (Unit.unitList[0].scheduledBehaviour != PlayerManager.ScheduledBehaviour.NotDecided)
        {
            BoardManager.searchEnemyCount--;
            if (BoardManager.searchEnemyCount < 0)
            {
                BoardManager.searchEnemyCount = 7;
            }
            //================================CASE.1:自機ユニットの振る舞い予定が移動だった場合========================
            if (Unit.unitList[0].scheduledBehaviour == PlayerManager.ScheduledBehaviour.Move)
            {
                // 01.//全敵ユニットの振る舞い予定を決め、turnSateを遷移させる
                if (turnState == TurnState.DecideEnemyBehaviour)
                {
                    for(int i = 1; i < Unit.unitList.Count; i++)
                    {
                        Unit.unitList[i].DecideBehaviour();
                    }
                    turnState = TurnState.StartUnitMovement;
                }
                // 02.ユニット移動中真偽値を真にし、振る舞い予定が移動である全ユニットについて移動を始め、turnStateを遷移させる
                if (turnState == TurnState.StartUnitMovement)
                {
                    for(int i = 0; i < Unit.unitList.Count; i++)
                    {
                        if(Unit.unitList[i].scheduledBehaviour == PlayerManager.ScheduledBehaviour.Move)
                        {
                            Unit.unitList[i].StartUnitMovement();
                        }
                    }
                    turnState = TurnState.DecidePlayerBehaviour;
                }
            }
            //================================CASE.1:自機ユニットの振る舞い予定が移動だった場合========================

            //================================CASE.2:自機ユニットの振る舞い予定が攻撃だった場合========================
            if (PlayerManager.instance.scheduledBehaviour != PlayerManager.ScheduledBehaviour.Move)
            {
                // 01.自機ユニットの攻撃アニメーション開始し(敵の受撃アニメーションの開始も行う)、trunStateを遷移させ、ターン処理をここで打ち切る
                if (turnState == TurnState.StartPlayerAttack)
                {
                    if(Unit.unitList[0].scheduledBehaviour == Unit.ScheduledBehaviour.Attack)
                    {
                        Unit.unitList[0].StartAttackAnimation();
                    }
                    turnState = TurnState.AfterPlayerAttack;
                    return;
                }
                // 02.戦闘の結果を解決し、turnStateを遷移させる
                if (turnState == TurnState.AfterPlayerAttack)
                {
                    Unit.ResolveBattleResult();
                    turnState = TurnState.DecideEnemyBehaviour;
                }
                // 0x.敵ユニットの振る舞い予定を決め、turnStateを遷移させる
                if (turnState == TurnState.DecideEnemyBehaviour)
                {
                    for (int i = 1; i < Unit.unitList.Count; i++)
                    {
                        Unit.unitList[i].DecideBehaviour();
                    }
                    turnState = TurnState.StartUnitMovement;
                }
                // 0x.ユニット移動中真偽値を真にし、振る舞い予定が移動である全ユニットについて移動を始め、turnStateを遷移させる
                if (turnState == TurnState.StartUnitMovement)
                {
                    //unitsAreMoving = true;
                    for (int i = 0; i < Unit.unitList.Count; i++)
                    {
                        if (Unit.unitList[i].scheduledBehaviour == PlayerManager.ScheduledBehaviour.Move)
                        {
                            Unit.unitList[i].StartUnitMovement();
                        }
                    }
                    turnState = TurnState.DecidePlayerBehaviour;
                }
            }
            //================================CASE.2:自機ユニットの振る舞い予定が攻撃だった場合========================
        }
        //eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee【終】自機ユニットの振る舞い予定に従いターン処理を場合分けするeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        
        //EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE【終了】ターン処理【終了】EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
    }
}
