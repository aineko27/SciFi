using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public string itemName = "Food";

    private float moveTime = 0.025f;
    private float inverseMoveTime;

    public static int maxItemAmount = 19;
    public static int maxItemAmountPerPage = 10;

    private BoxCollider2D boxCollider2D;
    private new Rigidbody2D rigidbody2D;

    private static  Vector2[] itemPuttionOrderArray = {new Vector2( 0, 0), new Vector2( 0, 1), new Vector2( 1, 0), new Vector2( 0,-1), new Vector2(-1, 0),
                                                       new Vector2( 1, 1), new Vector2( 1,-1), new Vector2(-1,-1), new Vector2(-1, 1), new Vector2( 0, 2),
                                                       new Vector2( 2, 0), new Vector2( 0,-2), new Vector2(-2, 0), new Vector2( 2, 2), new Vector2( 2,-2),
                                                       new Vector2(-2,-2), new Vector2(-2, 2), new Vector2( 1, 2), new Vector2( 2, 1), new Vector2( 2,-1),
                                                       new Vector2( 1,-2), new Vector2(-1,-2), new Vector2(-2,-1), new Vector2(-2, 1), new Vector2(-1, 2)}; 

    //孫要素(StatusWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    public static List<ItemController> ItemInBackpackList = new List<ItemController>();

    void Awake()
    {
        inverseMoveTime = 1 / moveTime;
        //各種コンポーネントの取得
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void start()
    {
    }

    public static Vector2 GetValidPuttingPosition(ItemController item, Vector2 startPosition)
    {
        Vector2 currentPosition = new Vector2();
        RaycastHit2D hit1 = new RaycastHit2D();
        RaycastHit2D hit2 = new RaycastHit2D();

        item.boxCollider2D.enabled = false;
        int i;
        for (i = 0; i < itemPuttionOrderArray.Length; i++)
        {
            currentPosition = startPosition + itemPuttionOrderArray[i];
            hit1 = Physics2D.Linecast(currentPosition, currentPosition + new Vector2(0.48f, 0.48f), GameManager.itemLayer);
            hit2 = Physics2D.Linecast(currentPosition, currentPosition + new Vector2(0.48f, 0.48f), GameManager.blockingLayer);
            if(i== itemPuttionOrderArray.Length-1)Debug.Log(hit2.transform != null);

            if(hit2.transform != null)
            {
                if(hit2.collider.gameObject.tag == "OuterWall" || hit2.collider.gameObject.tag == "Wall")
                {
                    continue;
                }
            }

            if (hit1.transform == null)
            {
                break;
            }
        }
        item.boxCollider2D.enabled = true;

        if (i == itemPuttionOrderArray.Length)
        {
            return new Vector2(100, 100);
        }
        return currentPosition;
    }

    public IEnumerator MoveItemSmoothly(Vector3 startPosition, Vector3 endPosition, RaycastHit2D hit)
    {
        GameManager.movingObjectCount++;
        float sqrRemainingDistance = (startPosition - endPosition).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rigidbody2D.position, endPosition, inverseMoveTime * Time.deltaTime);
            rigidbody2D.MovePosition(newPosition);
            
            sqrRemainingDistance = (transform.position - endPosition).sqrMagnitude;
            yield return null;
        }
        if (hit.transform != null)
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                Unit.unitList[0].DealDamage(hit.collider.gameObject.GetComponent<Unit>(), 5);
                Destroy(this.gameObject);
            }
        }

        Vector2 dropPosition = GetValidPuttingPosition(this, transform.position);

        if (dropPosition.x == 100)
        {
            CanvasController.StackMessageText(this.itemName + "は置き場所がなくて消滅した");
            Destroy(this.gameObject);
            ItemInBackpackList.Remove(this);
            GameManager.movingObjectCount--;
        }
        else
        {
            transform.position = dropPosition;
            ItemInBackpackList.Remove(this);
            GameManager.movingObjectCount--;
        }
    }
}
