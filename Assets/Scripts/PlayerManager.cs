using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Unit
{
    //
    public static int equipedWeaponIndex = 0;
    public static int diagonalFaceLastFrame = 0;
    
    //PlayerManagerをシングルトンで生成する準備
    public static PlayerManager instance = null;

    //Awake(virtualの上書き)
    protected override void Awake()
    {
        //UnitクラスのAwakeを実行
        base.Awake();

        //ゲーム開始時にGameManagerをinstanceに指定
        if (instance == null)
        {
            instance = this;
        }
        //このオブジェクト以外にGameManagerが存在する時
        else if (instance != this)
        {
            //このオブジェクトを破壊する
            Destroy(gameObject);
        }
    }

    //Start(virtualの上書き)
    protected override void Start()
    {
        //UnitクラスのStartを実行
        base.Start();

        //子要素のゲームオブジェクトとそれに付属するコンポーネントの宣言
        Model = transform.Find("Model").gameObject;
        modelAnimator = Model.GetComponent<Animator>();
    }

    //自機ユニットの振る舞いを決定する関数(abstractの上書き)
    public override void DecideBehaviour()
    {
        //scheduledBehaviourを未定の状態にする
        scheduledBehaviour = ScheduledBehaviour.NotDecided;
        
        for (int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
        {
            CanvasController.InfoWindow.SetActive(true);
            CanvasController.WeaponInfo.SetActive(true);
            CanvasController.ScrapInfo.SetActive(true);
            CanvasController.HpInfo.SetActive(true);

            BoardManager.FloorCheckSpriteList[i].SetActive(false);
            BoardManager.FloorTargetSpriteList[i].GetComponent<Renderer>().enabled = false;
            BoardManager.FloorRedSpriteList[i].SetActive(false);
            BoardManager.FloorYellowSpriteList[i].SetActive(false);
            BoardManager.FloorGraySpriteList[i].SetActive(false);
            //
            MyCameraController.MiniMapCamera.SetActive(true);
            MyCameraController.miniMapCameraCamera.clearFlags = CameraClearFlags.Nothing;
            MyCameraController.miniMapCameraCamera.rect = new Rect(-0.1f, -0.1f, 0.6f, 0.6f);
        }

        //もしキー入力がされていなかったらすぐにreturnする
        if (InputFunction.AnyKey() == false)
        {
            return;
        }

        //もしコマンドウィンドウが表示されている場合
        if (CanvasController.CommandWindow.activeSelf == true)
        {
            CanvasController.SelectCommand();
            return;
        }
        //もしアイテムウィンドウが表示されている場合
        else if (CanvasController.ItemWindow.activeSelf == true)
        {
            CanvasController.CommandItemWindow();
            return;
        }
        //もしウェポンウィンドウが表示されている場合
        else if (CanvasController.WeaponWindow.activeSelf == true)
        {
            CanvasController.CommandWeaponWindow();
            return;
        }
        else if (CanvasController.UpgradeWindow.activeSelf == true)
        {
            CanvasController.CommandUpgradeWindow();
        }
        //もし足元ウィンドウが表示されている場合
        else if (CanvasController.FeetWindow.activeSelf == true)
        {
            CanvasController.CommandFeetWindow();
            return;
        }

        else if (CanvasController.SwapWindow.activeSelf == true)
        {
            CanvasController.CommandSwapWindow();
            return;
        }
        else if (CanvasController.ScoutWindow.activeSelf == true)
        {
            CanvasController.CommandScoutWindow();
            return;
        }

        //コマンドウィンドウが表示されていない場合
        else if (scheduledBehaviour == Unit.ScheduledBehaviour.NotDecided)
        {
            if (InputFunction.GetKey("LeftControl") == true)
            {
                //ミニマップの表示をオフにする
                MyCameraController.MiniMapCamera.SetActive(false);

                //
                CanvasController.WeaponInfo.SetActive(false);
                CanvasController.ScrapInfo.SetActive(false);
                CanvasController.HpInfo.SetActive(false);

                for (int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
                {
                    BoardManager.FloorCheckSpriteList[i].SetActive(true);
                }

                if (InputFunction.GetKeyDown("LeftControl") == true)
                {
                    //DetectEnemyToAttack
                    DetectEnemyToAttack(true);
                }

                //TryToLaunchAttack
                TryToLaunchAttack();
            }
            if(InputFunction.GetKey("LeftShift") == true)
            {
                //ミニマップの表示をオフにする
                MyCameraController.MiniMapCamera.SetActive(false);

                for (int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
                {
                    BoardManager.FloorCheckSpriteList[i].SetActive(true);
                }

                Vector2 currentPosition;
                Vector2 diagonalDirection = new Vector2();
                RaycastHit2D hit;
                bool b;
                for (int i = 0; i < 4; i++)
                {
                    currentPosition = transform.position;
                    if (i == 0) diagonalDirection = new Vector2(1, 1);
                    else if (i == 1) diagonalDirection = new Vector2(-1, 1);
                    else if (i == 2) diagonalDirection = new Vector2(1, -1);
                    else if (i == 3) diagonalDirection = new Vector2(-1, -1);

                    int loopCount = 0;
                    while (loopCount < 1000)
                    {
                        currentPosition += diagonalDirection;
                        b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                        if(b == true)
                        {
                            hit.collider.gameObject.transform.Find("floor_gray").gameObject.SetActive(true);
                        }

                        b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                        if (b == true)
                        {
                            if (hit.collider.gameObject.tag == "OuterWall")
                            {
                                break;
                            }
                        }
                        if (loopCount > 990) Debug.LogError("MyError:PlayerManager.DecideBehaviour");
                        loopCount++;
                    }
                }
            }
            
            if (InputFunction.GetKey("Space") == true)
            {
                MyCameraController.miniMapCameraCamera.clearFlags = CameraClearFlags.Skybox;
                MyCameraController.miniMapCameraCamera.rect = new Rect(-0.1f, -0.1f, 1.2f, 1.2f);
            }

            if (InputFunction.GetKeyDown("S"))
            {
                CanvasController.CommandWindow.SetActive(true);
                CanvasController.StatusWindow.SetActive(true);
                CanvasController.MessageWindow.SetActive(false);
                return;
            }

            if (InputFunction.GetKeyDown("V"))
            {
                CanvasController.ScoutWindow.SetActive(true);
                MyCameraController.SetupReferenceObject(CanvasController.ScoutObject);
                return;
            }

            if (InputFunction.GetKey("Q") || InputFunction.GetKey("W") || InputFunction.GetKey("E") || InputFunction.GetKey("R"))
            {
                for (int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
                {
                    BoardManager.FloorCheckSpriteList[i].SetActive(true);
                }
                
                //
                if ( (equipedWeaponIndex == 1 && InputFunction.GetKey("W")) || (equipedWeaponIndex == 2 && InputFunction.GetKey("E")) || (equipedWeaponIndex == 3 && InputFunction.GetKey("R")))
                {
                    if ((equipedWeaponIndex == 1 && InputFunction.GetKeyDown("W")) || (equipedWeaponIndex == 2 && InputFunction.GetKeyDown("E")) || (equipedWeaponIndex == 3 && InputFunction.GetKeyDown("R")))
                    {
                        DetectEnemyToAttack(true);
                    }
                }
                else if (InputFunction.GetKey("Q"))
                {
                    if(InputFunction.GetKeyDown("Q") == true)
                    {
                        if(equipedWeaponIndex == 0)
                        {
                            DetectEnemyToAttack(true);
                        }
                        else
                        {
                            attackType = AttackType.Punch;
                            DetectEnemyToAttack(false);
                        }
                    }

                    if(equipedWeaponIndex != 0)
                    {
                        CanvasController.WeaponInfoOutline.transform.position = CanvasController.WeaponInfoIconArray[0].transform.position;
                        CanvasController.WeaponInfoIconArray[0].GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                        CanvasController.WeaponInfoIconArray[equipedWeaponIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

                        attackType = AttackType.Punch;
                        equipedWeaponIndex = 0;
                        DetectEnemyToAttack(false);
                    }
                }
                else if (InputFunction.GetKey("W"))
                {
                    if (InputFunction.GetKeyDown("W") == true)
                    {
                        if (equipedWeaponIndex == 1)
                        {
                            DetectEnemyToAttack(true);
                        }
                        else
                        {
                            attackType = AttackType.Laser;
                            DetectEnemyToAttack(false);
                        }
                    }

                    if (equipedWeaponIndex != 1)
                    {
                        CanvasController.WeaponInfoOutline.transform.position = CanvasController.WeaponInfoIconArray[1].transform.position;
                        CanvasController.WeaponInfoIconArray[1].GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                        CanvasController.WeaponInfoIconArray[equipedWeaponIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

                        attackType = AttackType.Laser;
                        equipedWeaponIndex = 1;
                        DetectEnemyToAttack(false);
                    }
                }
                else if (InputFunction.GetKey("E"))
                {
                    if (InputFunction.GetKeyDown("E") == true)
                    {
                        if (equipedWeaponIndex == 2)
                        {
                            DetectEnemyToAttack(true);
                        }
                        else
                        {
                            attackType = AttackType.Beam;
                            DetectEnemyToAttack(false);
                        }
                    }

                    if (equipedWeaponIndex != 2)
                    {
                        CanvasController.WeaponInfoOutline.transform.position = CanvasController.WeaponInfoIconArray[2].transform.position;
                        CanvasController.WeaponInfoIconArray[2].GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                        CanvasController.WeaponInfoIconArray[equipedWeaponIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

                        attackType = AttackType.Beam;
                        equipedWeaponIndex = 2;
                        DetectEnemyToAttack(false);
                    }
                }
                else if (InputFunction.GetKey("R"))
                {
                    if (InputFunction.GetKeyDown("R") == true)
                    {
                        if (equipedWeaponIndex == 3)
                        {
                            DetectEnemyToAttack(true);
                        }
                        else
                        {
                            attackType = AttackType.Null;
                            DetectEnemyToAttack(false);
                        }
                    }

                    if (equipedWeaponIndex != 3)
                    {
                        CanvasController.WeaponInfoOutline.transform.position = CanvasController.WeaponInfoIconArray[3].transform.position;
                        CanvasController.WeaponInfoIconArray[3].GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
                        CanvasController.WeaponInfoIconArray[equipedWeaponIndex].GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);

                        attackType = AttackType.Null;
                        equipedWeaponIndex = 3;
                        DetectEnemyToAttack(false);
                    }

                }
                //TryToLaunchAttack
                TryToLaunchAttack();
            }

            //
            Model.transform.rotation = Quaternion.LookRotation(new Vector3(faceDirection.x, faceDirection.y, 0), Vector3.back);

            //pが押された場合パスし、振る舞い予定をパスにする
            if (InputFunction.GetKey("P"))
            {
                scheduledBehaviour = ScheduledBehaviour.Pass;
            }

            //移動判定を検知する。移動可能であれば振る舞い予定を移動にする。
            if (scheduledBehaviour == ScheduledBehaviour.NotDecided)
            {
                TryToMove();
            }

            //次に攻撃判定を検知する。攻撃後、振る舞い予定を攻撃にする
            if (scheduledBehaviour == ScheduledBehaviour.NotDecided)
            {
                TryToAttack();
            }
        }
    }

    //移動を試みる関数(abstractの上書き)
    protected override void TryToMove()
    {
        RaycastHit2D hit;

        //移動方向の検知
        moveDirection.x = (int)InputFunction.GetAxisRaw("Horizontal");
        moveDirection.y = (int)InputFunction.GetAxisRaw("Vertical");

        //左右同時押しなどでmoveDirection.x,yがともに0であれば、ここでreturnする
        if (moveDirection.x == 0 && moveDirection.y == 0) return;

        //shiftが押されているのであれば、斜め方向にしか向く(移動)することができない
        if (InputFunction.GetKey("LeftShift") && (moveDirection.x == 0 || moveDirection.y == 0)) return;

        //自機ユニットの向きを変える(この時点では振る舞いをしたことにならない)
        if (moveDirection.x != 0 || moveDirection.y != 0)
        {
            if (InputFunction.GetAxisRaw("Horizontal") * InputFunction.GetAxisRaw("Vertical") != 0)
            {
                diagonalFaceLastFrame = GameManager.frameCount;
                faceDirection = moveDirection;
            }

            if (InputFunction.GetAxisRaw("Horizontal") * InputFunction.GetAxisRaw("Vertical") == 0)
            {
                if (GameManager.frameCount > diagonalFaceLastFrame + 6)
                {
                    faceDirection = moveDirection;
                }
            }

            //faceDirectionにmoveDirectionを代入する
            //faceDirection = moveDirection;

            //キャラクターを振り向かせる
            Model.transform.rotation = Quaternion.LookRotation(new Vector3(faceDirection.x, faceDirection.y, 0), Vector3.back);
        }

        //LeftControlが押されている場合は移動することができない(振る舞いをしていない判定で戻り値を返す)
        if (InputFunction.GetKey("LeftControl") || InputFunction.GetKey("Q") || InputFunction.GetKey("W") || InputFunction.GetKey("E") || InputFunction.GetKey("R") )
        {
            for(int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
            {
                BoardManager.FloorCheckSpriteList[i].SetActive(true);
            }
            return;
        }
        
        //ユニットが移動可能かを判定する(障害物がないかのチェック)
        canMove = CheckCanMove(out hit);
        if (canMove == true)
        {
            //アイテムゲット時
            if(hit.transform != null)
            {
                if (hit.collider.gameObject.tag == "Item")
                {
                    PickUpItem(hit.collider.gameObject.GetComponent<ItemController>());
                }
                else if(hit.collider.gameObject.tag == "Stair")
                {
                    CanvasController.StackMessageText("Go to next floor?");
                    GameManager.activateComfirmWindow = true;
                    CanvasController.comfirmSelectIconIndex = 0;
                    CanvasController.comfirmTheme = CanvasController.ComfirmTheme.UseStair;
                }
            }

            //Unitの当たり判定を先んじて移動先にずらしておく(こうしないと次のUnitと移動先がかぶることがある)
            boxCollider2D.offset = moveDirection;

            //今いる部屋の情報を取得
            this.presentRoomIndex = Mathf.RoundToInt(BoardManager.GetRaycastHit2D((Vector2)transform.position + moveDirection, "FloorLayer").transform.position.z);

            //振る舞い予定をmoveにする
            scheduledBehaviour = ScheduledBehaviour.Move;
        }
    }

    //移動可能かどうかを判定し、真偽値を返す関数
    //protected bool CheckCanMove() => Unit.CheckCanMove()を継承

    //ユニットの連続的な移動を始める関数
    //public void StartUnitMovement() => Unit.StartUnitMovement()を継承

    //ユニットを連続的に移動させる反復処理関数
    //protected IEnumerator MoveSmoothly() => Unit.MoveSmoothly()を継承

    //DetectEnemyToAttack
    private void DetectEnemyToAttack(bool willSearchCurrentDirection)
    {
        int iStart = 0;
        if (willSearchCurrentDirection == true) iStart++;

        RaycastHit2D hit;
        bool b;
        Vector2 currentPosition = transform.position;

        if (attackType == AttackType.Punch)
        {
            for (int i = iStart; i < BoardManager.searchEnemyArray.Length + iStart; i++)
            {
                int j = (i + BoardManager.searchEnemyCount) % BoardManager.searchEnemyArray.Length;
                currentPosition = (Vector2)transform.position + BoardManager.searchEnemyArray[j];
                b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                if (b == true)
                {
                    if (hit.collider.gameObject.tag == "Enemy")
                    {
                        faceDirection = BoardManager.searchEnemyArray[j];
                        BoardManager.searchEnemyCount = j;
                        break;
                    }
                }
            }
        }

        else if(attackType == AttackType.Laser || attackType == AttackType.Beam)
        {
            for (int i = iStart; i < BoardManager.searchEnemyArray.Length + iStart; i++)
            {
                currentPosition = transform.position;
                int j = (i + BoardManager.searchEnemyCount) % BoardManager.searchEnemyArray.Length;

                int loopCount = 0;
                while (loopCount < 1000)
                {
                    currentPosition += BoardManager.searchEnemyArray[j];

                    b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                    if (b == true)
                    {
                        if (hit.collider.gameObject.tag == "OuterWall")
                        {
                            break;
                        }
                        else if (hit.collider.gameObject.tag == "Enemy")
                        {
                            faceDirection = BoardManager.searchEnemyArray[j];
                            BoardManager.searchEnemyCount = j;
                            i = BoardManager.searchEnemyArray.Length + 1;
                            break;
                        }
                    }
                    if (loopCount > 990) Debug.LogError("MyError:PlayerManager.DecideBehaviour");
                    loopCount++;
                }
            }
        }
    }

    //TryToLaunchAttack
    private void TryToLaunchAttack()
    {
        Vector2 currentPosition;
        bool b;
        RaycastHit2D hit;
        int loopCount = 0;

        currentPosition = transform.position;
        switch (attackType)
        {
            case AttackType.Punch:
                currentPosition += faceDirection;

                b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                if(b == true)
                {
                    hit.collider.gameObject.transform.Find("floor_red").gameObject.SetActive(true);
                    hit.collider.gameObject.transform.Find("floor_target_white").gameObject.GetComponent<Renderer>().enabled = true;
                }
                break;

            case AttackType.Laser:
                loopCount = 0;
                while (loopCount < 1000)
                {
                    currentPosition += faceDirection;
                    b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                    if (b == true)
                    {
                        if (hit.collider.gameObject.tag == "OuterWall")
                        {
                            if (loopCount == 0) break;
                            b = BoardManager.ExistLayer(currentPosition - faceDirection, "FloorLayer", out hit);
                            hit.collider.gameObject.transform.Find("floor_yellow").gameObject.SetActive(false);
                            hit.collider.gameObject.transform.Find("floor_red").gameObject.SetActive(true);
                            hit.collider.gameObject.transform.Find("floor_target_white").gameObject.GetComponent<Renderer>().enabled = true;
                            break;
                        }
                    }

                    b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                    hit.collider.gameObject.transform.Find("floor_yellow").gameObject.SetActive(true);

                    b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                    if (b == true && hit.collider.gameObject.tag == "Enemy")
                    {
                        b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                        hit.collider.gameObject.transform.Find("floor_yellow").gameObject.SetActive(false);
                        hit.collider.gameObject.transform.Find("floor_red").gameObject.SetActive(true);
                        hit.collider.gameObject.transform.Find("floor_target_white").gameObject.GetComponent<Renderer>().enabled = true;
                        break;
                    }
                    if (loopCount > 990) Debug.LogError("MyError:PlayerManager.DecideBehaviour");
                    loopCount++;
                }
                break;

            case AttackType.Beam:
                loopCount = 0;
                while (loopCount < 1000)
                {
                    currentPosition += faceDirection;
                    b = BoardManager.ExistLayer(currentPosition, "BlockingLayer", out hit);
                    if (b == true && hit.collider.gameObject.tag == "OuterWall")
                    {
                        break;
                    }
                    if (b == true && hit.collider.gameObject.tag == "Enemy")
                    {
                        b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                        hit.collider.gameObject.transform.Find("floor_target_white").gameObject.GetComponent<Renderer>().enabled = true;
                        hit.collider.gameObject.transform.Find("floor_red").gameObject.SetActive(true);
                    }
                    else
                    {
                        b = BoardManager.ExistLayer(currentPosition, "FloorLayer", out hit);
                        hit.collider.gameObject.transform.Find("floor_yellow").gameObject.SetActive(true);
                    }
                    if (loopCount > 990) Debug.LogError("MyError:PlayerManager.DecideBehaviour");
                    loopCount++;
                }
                break;
            
            default:
                Debug.Log("MyError: PlayerManager.TryToLaunchAttack");
                break;
        }
    }

    //移動を試みる関数
    protected override void TryToAttack()
    {
        //攻撃ボタン"A"が押されていたら攻撃
        if (InputFunction.GetKey("A") == true)
        {
            for (int i = 0; i < BoardManager.FloorCheckSpriteList.Count; i++)
            {
                BoardManager.FloorCheckSpriteList[i].SetActive(false);
                BoardManager.FloorTargetSpriteList[i].GetComponent<Renderer>().enabled = false;
                BoardManager.FloorRedSpriteList[i].SetActive(false);
                BoardManager.FloorYellowSpriteList[i].SetActive(false);
                BoardManager.FloorGraySpriteList[i].SetActive(false);
            }

            //振る舞い予定を攻撃にする
            scheduledBehaviour = ScheduledBehaviour.Attack;

            //攻撃を発射する
            LaunchAttack();
        }
    }

    //攻撃を発射し、攻撃衝突判定を行う関数
    //protected void LaunchAttack() => Unit.LaunchAttack()を継承

    //thisが対象にダメージを与える関数
    //protected void DealDamage() => Unit.DealDamage()を継承

    //ユニットの攻撃アニメーションを始める関数
    //protected void StartAttackAnimation() => Unit.StartAttackAnimation()を継承

    //ユニットが死亡したときの処理を行う関数
    //public void ResolveDeath() => Unit.ResolveDeath()を継承

    //ユニットの消去を行う関数
    //public void RemoveUnit() => Unit.RemoveUnit()を継承
}