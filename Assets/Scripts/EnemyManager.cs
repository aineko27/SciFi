using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Unit
{
    //敵ユニットオブジェクトを統括するゲームオブジェクトの宣言
    public static GameObject Enemy;

    //
    private int notMoveCount = 0;
        
    //Watch用に使用する変数の宣言
    [Watch]public ScheduledBehaviour testWatch;

    //Awake(virtualの上書き)
    protected override void Awake()
    {
        //UnitクラスのAwakeを実行
        moveTime = 0.085f;


        base.Awake();

        //Enemyという名前のゲームオブジェクトを生成する。このゲームオブジェクトで敵ユニットの管理を行う
        //Enemy = new GameObject();
        //Enemy.name = "Enemy";
    }

    //Start(virtualの上書き)
    protected override void Start()
    {
        //UnitクラスのStartを実行
        base.Start();
    }

    //LateUpdate(デバッグ用にしようがなく使う)
    void LateUpdate()
    {
        testWatch = scheduledBehaviour;
    }

    public override void SetUnitStatus(UnitName name)
    {
        //Enemyをインスタンス化し、EnemyManagerの子要素にする
        //transform.SetParent(Enemy.transform);

        //UnitクラスのSetUnitStatusを実行
        base.SetUnitStatus(name);
    }

    //敵ユニットの振る舞いを決定する関数(abstractの上書き)
    public override void DecideBehaviour()
    {
        //scheduledBehaviourを未定の状態にする
        scheduledBehaviour = ScheduledBehaviour.NotDecided;

        //攻撃を試みる
        TryToAttack();

        //移動を試みる
        TryToMove();
    }

    //移動を試みる関数(abstractの上書き)
    protected override void TryToMove()
    {
        //
        RaycastHit2D hit;

        //目的地設定
        //もし、非移動カウントが5より大きかったら、顔方向を反転させる
        if(notMoveCount > 5)
        {
            this.faceDirection = this.faceDirection * -Vector2.one;
            notMoveCount = 0;
        }

        //もし自分が部屋の中にいるのであれば、状況に合わせて目的地を設定するかどうかを決める
        if(this.presentRoomIndex >= 0)
        {
            BoardManager.Room thisRoom = BoardManager.RoomList[presentRoomIndex];
            //もし同じ部屋にプレイヤーがいるのであれば、そこを目的地にする
            if(this.presentRoomIndex == Unit.unitList[0].presentRoomIndex)
            {
                this.destinationPoint = ((Vector2)Unit.unitList[0].transform.position + Unit.unitList[0].moveDirection) - (Vector2)this.transform.position;
            }
            //そうでなく、今の部屋に出入り口が０つしかないなら
            else if(thisRoom.entrancePointList.Count == 0)
            {
                Debug.LogError("EnemyManager.TryToMove: ***thisRoom.entrancePointList.Count==0*** ");
            }
            //そうでなく、今の部屋に出入り口が１つしかないなら
            else if(thisRoom.entrancePointList.Count == 1)
            {
                //もし自分の現在地が部屋の出入り口なら、新たな目的地を作らない
                if(thisRoom.entrancePointList[0] == (Vector2)this.transform.position)
                {
                    //Debug.Log("PASS");
                }
                //そうでなく、もし目的地がないなら、部屋の出入り口を新たな目的地にする
                else if(this.destinationPoint == Vector2.zero)
                {
                    this.destinationPoint = thisRoom.entrancePointList[0] - (Vector2)this.transform.position;
                }
            }
            //そうでない(部屋に出入り口が２つ以上有り)なら
            else if (this.destinationPoint == Vector2.zero)
            {
                bool isEntrance = false;
                //もし現在地が出入り口なら、新たな目的地を作らない
                for (int i = 0; i < thisRoom.entrancePointList.Count; i++)
                {
                    if((Vector2)this.transform.position == thisRoom.entrancePointList[i])
                    {
                        isEntrance = true;
                    }
                }
                if(isEntrance == false)
                {
                    int[] randomIndexArray = UtilityFunction.RandomIndexArray(thisRoom.entrancePointList.Count);
                    for (int i = 0; i < thisRoom.entrancePointList.Count; i++)
                    {
                        //もし"現在地引く進行方向の地点"がランダムで選ばれた出入り口と違う地点なら、そこを新しい目的地にする
                        if ((Vector2)this.transform.position - this.faceDirection != thisRoom.entrancePointList[randomIndexArray[i]])
                        {
                            this.destinationPoint = thisRoom.entrancePointList[randomIndexArray[i]] - (Vector2)this.transform.position;
                        }
                    }
                }
            }
        }//目的地設定、ここまで

        //進行方向(moveDirection)を決定する。そのために仮進行方向を定義する
        Vector2 moveDirectionTemp = new Vector2();
        //もし目的地があるのであれば、仮進行方向をその方向にする
        if(destinationPoint != Vector2.zero)
        {
            moveDirectionTemp.x = UtilityFunction.LimitRangeFloat(-1, destinationPoint.x, 1);
            moveDirectionTemp.y = UtilityFunction.LimitRangeFloat(-1, destinationPoint.y, 1);
        }
        //そうでないなら、条件により仮進行方向を決める
        else
        {
            if(faceDirection != Vector2.zero)
            {
                moveDirectionTemp = faceDirection + Vector2.zero; //参照渡しにならないか心配だったため
            }
            else
            {
                moveDirectionTemp = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2));
            }
        }

        //仮進行方向を基に、左折法で進行方向を決める
        for(int i = 0; i < 5; i++)
        {
            //左手法により進行方向を変化させる
            if (i == 0)
            {
                moveDirection = moveDirectionTemp + Vector2.zero;
            }

            if (i == 1)
            {
                if (moveDirectionTemp.x * moveDirectionTemp.y == 0) {
                    moveDirection = new Vector2(moveDirectionTemp.x - moveDirectionTemp.y, moveDirectionTemp.x + moveDirectionTemp.y);
                }
                else
                {
                    moveDirection = new Vector2((moveDirectionTemp.x - moveDirectionTemp.y) / 2, (moveDirectionTemp.x + moveDirectionTemp.y) / 2);
                }
            }
            else if(i == 2)
            {
                if (moveDirectionTemp.x * moveDirectionTemp.y == 0)
                {
                    moveDirection = new Vector2(moveDirectionTemp.x + moveDirectionTemp.y, -moveDirectionTemp.x + moveDirectionTemp.y);
                }
                else
                {
                    moveDirection = new Vector2((moveDirectionTemp.x + moveDirectionTemp.y) / 2, (-moveDirectionTemp.x + moveDirectionTemp.y) / 2);
                }
            }
            else if(i == 3)
            {
                moveDirection = new Vector2(-moveDirectionTemp.y, moveDirectionTemp.x);
            }
            else if(i == 4)
            {
                moveDirection = new Vector2(moveDirectionTemp.y, -moveDirectionTemp.x);
            }

            canMove = CheckCanMove(out hit);
            if(canMove == true)
            {
                //
                notMoveCount = 0;

                //Unitの当たり判定を先んじて移動先にずらしておく(こうしないと次のUnitと移動先がかぶることがある)
                boxCollider2D.offset = moveDirection;

                //移動できる場合、振る舞い予定をmoveにして戻り値を返す。
                scheduledBehaviour = ScheduledBehaviour.Move;
                faceDirection = moveDirection;
                if(this.destinationPoint != Vector2.zero)
                {
                    destinationPoint -= moveDirection;
                }

                //今いる部屋の情報を取得
                this.presentRoomIndex = Mathf.RoundToInt(BoardManager.GetRaycastHit2D((Vector2)transform.position + moveDirection, "FloorLayer").transform.position.z);
                break;
            }
        }
        //進行方向(moveDirection)を決定する。ここまで

        //もし、移動不可能だった場合
        if (canMove == false)
        {
            notMoveCount++;
            scheduledBehaviour = ScheduledBehaviour.Pass;
        }

        if (this.presentRoomIndex == Unit.unitList[0].presentRoomIndex && this.presentRoomIndex != -1)
        {
            this.destinationPoint = ((Vector2)Unit.unitList[0].transform.position + Unit.unitList[0].moveDirection) - ((Vector2)transform.position + moveDirection);
            return;
        }
        for (int n = 0; n < BoardManager.searchEnemyArray.Length; n++)
        {
            Vector2 currentPosition = (Vector2)transform.position + BoardManager.searchEnemyArray[n];
            RaycastHit2D h = BoardManager.GetRaycastHit2D(currentPosition, "BlockingLayer");
            if (h.transform != null && h.collider.gameObject.tag == "Player")
            {
                this.destinationPoint = ((Vector2)Unit.unitList[0].transform.position + Unit.unitList[0].moveDirection) - ((Vector2)transform.position + moveDirection);
                return;
            }
        }

        //if (destinationPoint != Vector2.zero)
        //{
        //    moveDirectionTemp.x = UtilityFunction.LimitRangeFloat(-1, destinationPoint.x, 1);
        //    moveDirectionTemp.y = UtilityFunction.LimitRangeFloat(-1, destinationPoint.y, 1);

        //    moveDirection = moveDirectionTemp;
        //    canMove = CheckCanMove(out hit);

        //    if(canMove == false)
        //    {
        //        if(this.destinationPoint.x > this.destinationPoint.y)
        //        {
        //            moveDirection = moveDirectionTemp * Vector2Int.right;
        //        }
        //        else
        //        {
        //            moveDirection = moveDirectionTemp * Vector2Int.up;
        //        }
        //        canMove = CheckCanMove(out hit);
        //    }

        //    if(canMove == false)
        //    {
        //        if (this.destinationPoint.x > this.destinationPoint.y)
        //        {
        //            moveDirection = moveDirectionTemp * Vector2Int.up;
        //        }
        //        else
        //        {
        //            moveDirection = moveDirectionTemp * Vector2Int.right;
        //        }
        //        canMove = CheckCanMove(out hit);
        //    }

        //    //移動可能の場合
        //    if (canMove == true)
        //    {
        //        //Unitの当たり判定を先んじて移動先にずらしておく(こうしないと次のUnitと移動先がかぶることがある)
        //        boxCollider2D.offset = moveDirection;

        //        //移動できる場合、振る舞い予定をmoveにして戻り値を返す。
        //        scheduledBehaviour = ScheduledBehaviour.Move;
        //        faceDirection = moveDirection;
        //        destinationPoint -= moveDirection;

        //        //今いる部屋の情報を取得
        //        this.presentRoomIndex = Mathf.RoundToInt(BoardManager.GetRaycastHit2D((Vector2)transform.position + moveDirection, "FloorLayer").transform.position.z);
        //    }
        //    //移動不可能の場合
        //    else
        //    {
        //        if(this.destinationPoint.x > this.destinationPoint.y)
        //        {
        //            moveDirection = moveDirection * Vector2Int.right;
        //        }
        //    }
        //}
        //else
        //{
        //    //移動する方向を決める
        //    if(faceDirection == Vector2.zero)
        //    {
        //        moveDirection.x = Random.Range(-1, 2);
        //        moveDirection.y = Random.Range(-1, 2);
        //    }
        //    else
        //    {
        //        moveDirection = faceDirection;
        //    }

        //    //敵ユニットが移動可能かを判定する(障害物がないかのチェック)
        //    canMove = CheckCanMove(out hit);
            
        //    //
        //    if (canMove == false)
        //    {
        //        this.moveDirection = new Vector2(this.moveDirection.y, this.moveDirection.x);
        //        canMove = CheckCanMove(out hit);
        //    }
        //    //
        //    if (canMove == false)
        //    {
        //        this.moveDirection = new Vector2(-this.moveDirection.x, -this.moveDirection.y);
        //        canMove = CheckCanMove(out hit);
        //    }


        //    //移動可能の場合
        //    if (canMove == true)
        //    {
        //        //
        //        this.faceDirection = this.moveDirection;

        //        //Unitの当たり判定を先んじて移動先にずらしておく(こうしないと次のUnitと移動先がかぶることがある)
        //        boxCollider2D.offset = moveDirection;

        //        //移動できる場合、振る舞い予定をmoveにして戻り値を返す。
        //        scheduledBehaviour = ScheduledBehaviour.Move;

        //        //今いる部屋の情報を取得
        //        this.presentRoomIndex = Mathf.RoundToInt(BoardManager.GetRaycastHit2D((Vector2)transform.position + moveDirection, "FloorLayer").transform.position.z);
        //    }
        //    //移動不可能の場合
        //    else
        //    {
        //        scheduledBehaviour = ScheduledBehaviour.Pass;
        //    }
        //}
        //if (this.presentRoomIndex == Unit.unitList[0].presentRoomIndex && this.presentRoomIndex != -1)
        //{
        //    this.destinationPoint = ((Vector2)Unit.unitList[0].transform.position+ Unit.unitList[0].moveDirection) - ((Vector2)transform.position + moveDirection);
        //    return;
        //}
        //for(int n = 0; n < BoardManager.searchEnemyArray.Length; n++)
        //{
        //    Vector2 currentPosition = (Vector2)transform.position + BoardManager.searchEnemyArray[n];
        //    RaycastHit2D h = BoardManager.GetRaycastHit2D(currentPosition, "BlockingLayer");
        //    if(h.transform != null && h.collider.gameObject.tag == "Player")
        //    {
        //        this.destinationPoint = ((Vector2)Unit.unitList[0].transform.position + Unit.unitList[0].moveDirection) - ((Vector2)transform.position + moveDirection);
        //        return;
        //    }
        //}
    }

    //移動可能かどうかを判定し、真偽値を返す関数
    //protected bool CheckCanMove() => Unit.CheckCanMove()を継承

    //ユニットの連続的な移動を始める関数
    //public void StartUnitMovement() => Unit.StartUnitMovement()を継承

    //ユニットを連続的に移動させる反復処理関数
    //protected IEnumerator MoveSmoothly() => Unit.MoveSmoothly()を継承

    //攻撃を試みる関数
    protected override void TryToAttack()
    {

    }

    //攻撃を発射し、攻撃衝突判定を行う関数
    //protected void LaunchAttack() => Unit.LaunchAttack()を継承

    //thisが対象にダメージを与える関数
    //protected void DealDamage() => Unit.DealDamage()を継承

    //ユニットの攻撃アニメーションを始める関数
    //public void StartAttackAnimation() => Unit.StartAttackAnimation()を継承

    //ユニットが死亡したときの処理を行う関数
    //public void ResolveDeath() => Unit.ResolveDeath()を継承

    //ユニットの消去を行う関数
    //public void RemoveUnit() => Unit.RemoveUnit()を継承
}
