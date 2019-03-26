using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCameraController : MonoBehaviour
{
    //ゲームオブジェクトの取得
    public static GameObject ReferenceObject;
    public static GameObject MainCamera;
    public static GameObject MiniMapCamera;
    public static GameObject Player;
    public static GameObject ScoutObject;
    public static SpriteRenderer miniMapSpritePlayer;
    public static Camera miniMapCameraCamera;

    //メインカメラとプレイヤーのオフセット値の宣言
    public static Vector3 offset;

    //Awake
    void Awake()
    {

    }

    //Start
    void Start()
    {
        //参照オブジェクトを取得する
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        MiniMapCamera = GameObject.FindGameObjectWithTag("MiniMapCamera");
        miniMapCameraCamera = MiniMapCamera.GetComponent<Camera>();
        Player = GameObject.FindGameObjectWithTag("Player");
        ScoutObject = CanvasController.ScoutObject;
        
        SetupReferenceObject(Player);

        miniMapSpritePlayer = Player.transform.Find("MiniMapTile").gameObject.GetComponent<SpriteRenderer>();
    }

    //Update
    void Update()
    {
        if (ReferenceObject == null)
        {
            return;
        }
        else
        {
            //MainCameraの位置情報を更新する
            transform.position = ReferenceObject.transform.position + offset;
        }
    }

    public static void SetupReferenceObject(GameObject Reference)
    {
        MainCamera.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -100);

        //参照オブジェクトを取得する
        ReferenceObject = Reference;
        //カメラのオフセット値を求める
        offset = MainCamera.transform.position - Reference.transform.position;
    }
}
