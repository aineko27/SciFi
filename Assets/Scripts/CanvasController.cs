using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public static float scaleX;
    public static float scaleY;
    //自己要素をstaticとして宣言
    public static GameObject Canvas;

    //子要素(各ウィンドウ)の各ゲームオブジェクトの宣言
    public static GameObject MessageWindow;
    public static GameObject ComfirmWindow;
    public static GameObject CommandWindow;
    public static GameObject StatusWindow;
    public static GameObject ItemWindow;
    public static GameObject WeaponWindow;
    public static GameObject UpgradeWindow;
    public static GameObject FeetWindow;
    public static GameObject OtherWindow;
    public static GameObject ScoutWindow;
    public static GameObject AbortWindow;
    public static GameObject SwapWindow;
    public static GameObject InfoWindow;
    public static GameObject LevelWindow;

    //孫要素(InfoWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    public static GameObject WeaponInfo;
    public static GameObject WeaponInfoOutline;
    public static GameObject[] WeaponInfoIconArray = new GameObject[4];
    public static GameObject WeaponInfoIcon00;
    public static GameObject WeaponInfoIcon01;
    public static GameObject WeaponInfoIcon02;
    public static GameObject WeaponInfoIcon03;
    public static GameObject ScrapInfo;
    public static GameObject ScrapInfoIcon;
    public static GameObject ScrapInfoLavel;
    public static Text ScrapInfoLavelText;
    public static GameObject HpInfo;
    public static GameObject HpInfoIcon;
    public static GameObject HpInfoLavel;
    public static Text HpInfoLavelText;
    public static GameObject HpInfoSlider;
    public static Slider HpInfoSliderSlider;
    public static RectTransform HpInfoSliderRect;

    //孫要素(MessageWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject NextPageIcon;
    private static GameObject MessageText;
    private static Text messageTextScript;
    public static List<string> stackedMessageList = new List<string>();
    private static int messageWindowInactiveCount;

    //孫要素(ComfirmWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    public enum ComfirmTheme
    {
        UseStair
    }
    public static ComfirmTheme comfirmTheme;
    private static GameObject Yes;
    private static GameObject No;
    private static GameObject ComfirmSelectIcon;
    private static Animator ComfirmSelectIconAnim;
    public static int comfirmSelectIconIndex;

    //孫要素(CommandWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject CommandLavel;
    private static GameObject[] CommandLavelArray = new GameObject[6];
    private static GameObject SelectedCommandIcon;
    private static Animator selectedCommandIconAnim;
    private static int selectedCommandIndex = 0;

    //孫要素(StatusWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject StatusText;
    private static Text statusTextScript;

    //孫要素(ItemWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject SelectedItemIcon;
    private static Animator selectedItemIconAnim;
    private static GameObject NextItemPageIconRight;
    private static GameObject NextItemPageIconLeft;
    private static int selectedItemIndex = 0;
    private static GameObject ItemLavel;
    private static GameObject[] ItemLavelArray = new GameObject[10];
    private static Text[] ItemLavelTextArray = new Text[10];
    private static GameObject NoItemText;
    private static int itemPageIndex = 0;
    //曽孫要素(UseItemWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject UseItemWindow;
    private static Animator useItemIconAnim;
    private static GameObject UseItemIcon;
    private static GameObject UseItemLavel;
    private static GameObject[] UseItemLavelArray = new GameObject[5];
    private static int useItemIndex = 0;
    //曽孫要素(ItemDetailWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject ItemDetailWindow;
    private static Text itemDetailText;

    //孫要素(WeaponWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject[] WeaponBoxArray = new GameObject[4];
    private static GameObject[] WeaponBoxIconArray = new GameObject[4];
    private static int selectedWeaponIndex = 0;
    private static int weaponSwapIndexFormer = 0;
    private static int weaponSwapIndexLatter = 0;
    private static bool doWeaponSwap = false;
    private static Image[] WeaponImageWhiteArray = new Image[4];

    //孫要素(UpgradeWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject SelectedUpgradeIcon;
    private static GameObject UpgradeImage;
    private static GameObject[] UpgradeBoxArray0 = new GameObject[3];
    private static GameObject[] UpgradeBoxArray1 = new GameObject[3];
    private static GameObject[,] UpgradeFactor = new GameObject[3,3];
    private static Vector2Int selectedUpgradeLevelIndex = new Vector2Int();
    private static string[,] UpgradeFactorState = { { "selectable", "locked", "locked" }, { "selectable", "locked", "locked" }, { "selectable", "locked", "locked" } };
    private static string selectedUpgradeFactor = "ability";
    private static int commandUpgradeIndex = 0;
    private static int selectedUpgradeIndex = 0;
    private static bool upgradeIsReady = false;
    private static Sprite[] UpgradeFactorSprite;
    public Sprite[] UpgradeFactorSpriteTemp;

    //孫要素(FeetWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject SelectedFeetIcon;
    private static Animator selectedFeetIconAnim;
    private static int useFeetIndex = 0;
    private static GameObject FeetLavel;
    private static Text feetLavelText;
    private static GameObject UseFeetWindow;
    private static GameObject UseFeetIcon;
    private static Animator useFeetIconAnim;
    private static GameObject UseFeetLavel;
    private static GameObject[] UseFeetLavelArray = new GameObject[5];

    //孫要素(SwapWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject SelectedSwapIcon;
    private static Animator selectedSwapIconAnim;
    private static GameObject SwapLavel;
    private static GameObject[] SwapLavelArray = new GameObject[10];
    private static Text[] swapLavelTextArray = new Text[10];
    private static int swapIndex = 0;
    private static ItemController swapItem;

    //孫要素(ScoutWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject ScoutFloor;
    public static GameObject ScoutObject;
    private static GameObject ScoutAttention;
    private static float ScoutAttentionAlpha;

    //孫要素(DungeonLevelWindowで使用するゲームオブジェクト、コンポーネント等)の宣言
    private static GameObject LevelText;
    public static Text LevelTextText;

    //Start
    void Start()
    {
        //親要素のゲームオブジェクトの取得
        Canvas = gameObject;

        //子要素(各ウィンドウ)の各ゲームオブジェクトの取得
        InfoWindow = transform.Find("InfoWindow").gameObject;
        MessageWindow = transform.Find("MessageWindow").gameObject;
        ComfirmWindow = transform.Find("ComfirmWindow").gameObject;
        CommandWindow = transform.Find("CommandWindow").gameObject;
        StatusWindow = transform.Find("StatusWindow").gameObject;
        ItemWindow = transform.Find("ItemWindow").gameObject;
        WeaponWindow = transform.Find("WeaponWindow").gameObject;
        UpgradeWindow = transform.Find("UpgradeWindow").gameObject;
        FeetWindow = transform.Find("FeetWindow").gameObject;
        OtherWindow = transform.Find("OtherWindow").gameObject;
        ScoutWindow = transform.Find("ScoutWindow").gameObject;
        AbortWindow = transform.Find("AbortWindow").gameObject;
        LevelWindow = transform.Find("LevelWindow").gameObject;

        //InfoWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        WeaponInfo = InfoWindow.transform.Find("WeaponInfo").gameObject;
        WeaponInfoOutline = WeaponInfo.transform.Find("WeaponInfoOutline").gameObject;
        WeaponInfoIconArray[0] =WeaponInfo.transform.Find("WeaponInfoIcon00").gameObject;
        WeaponInfoIconArray[1] =WeaponInfo.transform.Find("WeaponInfoIcon01").gameObject;
        WeaponInfoIconArray[2] =WeaponInfo.transform.Find("WeaponInfoIcon02").gameObject;
        WeaponInfoIconArray[3] =WeaponInfo.transform.Find("WeaponInfoIcon03").gameObject;
        ScrapInfo = InfoWindow.transform.Find("ScrapInfo").gameObject;
        ScrapInfoIcon = ScrapInfo.transform.Find("ScrapInfoIcon").gameObject;
        ScrapInfoLavel = ScrapInfo.transform.Find("ScrapInfoLavel").gameObject;
        ScrapInfoLavelText = ScrapInfoLavel.GetComponent<Text>();
        HpInfo = InfoWindow.transform.Find("HpInfo").gameObject;
        HpInfoIcon = HpInfo.transform.Find("HpInfoIcon").gameObject;
        HpInfoLavel = HpInfo.transform.Find("HpInfoLavel").gameObject;
        HpInfoLavelText = HpInfoLavel.GetComponent<Text>();
        HpInfoSlider = HpInfo.transform.Find("HpInfoSlider").gameObject;
        HpInfoSliderSlider = HpInfoSlider.GetComponent<Slider>();
        HpInfoSliderRect = HpInfoSlider.GetComponent<RectTransform>();
        
        //MessageWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        NextPageIcon = MessageWindow.transform.Find("NextPageIcon").gameObject;
        MessageText = MessageWindow.transform.Find("MessageText").gameObject;
        messageTextScript = MessageText.GetComponent<Text>();
        messageTextScript.text = "MESSAGE WINDOW: \n" +
                                  "THIS TEXT IS \n" +
                                  "LOREM IPSUM.";

        //ComfirmWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        Yes = ComfirmWindow.transform.Find("Yes").gameObject;
        No = ComfirmWindow.transform.Find("No").gameObject;
        ComfirmSelectIcon = ComfirmWindow.transform.Find("ComfirmSelectIcon").gameObject;
        ComfirmSelectIconAnim = ComfirmSelectIcon.GetComponent<Animator>();

        //CommandWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        CommandLavel = CommandWindow.transform.Find("CommandLavel").gameObject;
        CommandLavelArray[0] = CommandLavel.transform.Find("CommandLavel00").gameObject;
        CommandLavelArray[1] = CommandLavel.transform.Find("CommandLavel01").gameObject;
        CommandLavelArray[2] = CommandLavel.transform.Find("CommandLavel02").gameObject;
        CommandLavelArray[3] = CommandLavel.transform.Find("CommandLavel03").gameObject;
        CommandLavelArray[4] = CommandLavel.transform.Find("CommandLavel04").gameObject;
        CommandLavelArray[5] = CommandLavel.transform.Find("CommandLavel05").gameObject;
        SelectedCommandIcon  = CommandWindow.transform.Find("SelectedCommandIcon").gameObject;
        selectedCommandIconAnim = SelectedCommandIcon.GetComponent<Animator>();

        //StatusWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        StatusText = StatusWindow.transform.Find("StatusText").gameObject;
        statusTextScript = StatusText.GetComponent<Text>();
        statusTextScript.text =   "  DUMMY01:" + UnityEngine.Random.Range(10, 100).ToString() + "            DUMMY02:" + UnityEngine.Random.Range(10, 100).ToString() +
                                "\n  DUMMY03:" + UnityEngine.Random.Range(10, 100).ToString() + "            DUMMY04:" + UnityEngine.Random.Range(10, 100).ToString() +
                                "\n  DUMMY05:" + UnityEngine.Random.Range(10, 100).ToString() + "            DUMMY06:" + UnityEngine.Random.Range(10, 100).ToString();

        //ItemWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        SelectedItemIcon = ItemWindow.transform.Find("SelectedItemIcon").gameObject;
        selectedItemIconAnim = SelectedItemIcon.GetComponent<Animator>();
        NextItemPageIconRight = ItemWindow.transform.Find("NextItemPageIconRight").gameObject;
        NextItemPageIconLeft = ItemWindow.transform.Find("NextItemPageIconLeft").gameObject;
        ItemLavel = ItemWindow.transform.Find("ItemLavel").gameObject;
        ItemLavelArray[0] = ItemLavel.transform.Find("ItemLavel00").gameObject;
        ItemLavelArray[1] = ItemLavel.transform.Find("ItemLavel01").gameObject;
        ItemLavelArray[2] = ItemLavel.transform.Find("ItemLavel02").gameObject;
        ItemLavelArray[3] = ItemLavel.transform.Find("ItemLavel03").gameObject;
        ItemLavelArray[4] = ItemLavel.transform.Find("ItemLavel04").gameObject;
        ItemLavelArray[5] = ItemLavel.transform.Find("ItemLavel05").gameObject;
        ItemLavelArray[6] = ItemLavel.transform.Find("ItemLavel06").gameObject;
        ItemLavelArray[7] = ItemLavel.transform.Find("ItemLavel07").gameObject;
        ItemLavelArray[8] = ItemLavel.transform.Find("ItemLavel08").gameObject;
        ItemLavelArray[9] = ItemLavel.transform.Find("ItemLavel09").gameObject;
        ItemLavelTextArray[0] = ItemLavelArray[0].GetComponent<Text>();
        ItemLavelTextArray[1] = ItemLavelArray[1].GetComponent<Text>();
        ItemLavelTextArray[2] = ItemLavelArray[2].GetComponent<Text>();
        ItemLavelTextArray[3] = ItemLavelArray[3].GetComponent<Text>();
        ItemLavelTextArray[4] = ItemLavelArray[4].GetComponent<Text>();
        ItemLavelTextArray[5] = ItemLavelArray[5].GetComponent<Text>();
        ItemLavelTextArray[6] = ItemLavelArray[6].GetComponent<Text>();
        ItemLavelTextArray[7] = ItemLavelArray[7].GetComponent<Text>();
        ItemLavelTextArray[8] = ItemLavelArray[8].GetComponent<Text>();
        ItemLavelTextArray[9] = ItemLavelArray[9].GetComponent<Text>();
        NoItemText = ItemLavel.transform.Find("NoItemText").gameObject;
        //UseItemWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        UseItemWindow = ItemWindow.transform.Find("UseItemWindow").gameObject;
        UseItemIcon = UseItemWindow.transform.Find("UseItemIcon").gameObject;
        useItemIconAnim = UseItemIcon.GetComponent<Animator>();
        UseItemLavel = UseItemWindow.transform.Find("UseItemLavel").gameObject;
        UseItemLavelArray[0] = UseItemLavel.transform.Find("UseItemLavel00").gameObject;
        UseItemLavelArray[1] = UseItemLavel.transform.Find("UseItemLavel01").gameObject;
        UseItemLavelArray[2] = UseItemLavel.transform.Find("UseItemLavel02").gameObject;
        UseItemLavelArray[3] = UseItemLavel.transform.Find("UseItemLavel03").gameObject;
        UseItemLavelArray[4] = UseItemLavel.transform.Find("UseItemLavel04").gameObject;
        //ItemDetailWindowで使用するゲームオブジェクト、コンポーネントなどの取得
        ItemDetailWindow = ItemWindow.transform.Find("ItemDetailWindow").gameObject;
        itemDetailText = ItemDetailWindow.transform.Find("ItemDetailText").GetComponent<Text>();

        //WeaponWindowで使用するゲームオブジェクト、コンポーネントなどの取得
        //WeaponCommandWindowで使用するゲームオブジェクト、コンポーネントなどの取得
        WeaponBoxArray[0] = WeaponWindow.transform.Find("WeaponBox00").gameObject;
        WeaponBoxArray[1] = WeaponWindow.transform.Find("WeaponBox01").gameObject;
        WeaponBoxArray[2] = WeaponWindow.transform.Find("WeaponBox02").gameObject;
        WeaponBoxArray[3] = WeaponWindow.transform.Find("WeaponBox03").gameObject;
        WeaponImageWhiteArray[0] = WeaponBoxArray[0].transform.Find("WeaponIcon").gameObject.GetComponent<Image>();
        WeaponImageWhiteArray[1] = WeaponBoxArray[1].transform.Find("WeaponIcon").gameObject.GetComponent<Image>();
        WeaponImageWhiteArray[2] = WeaponBoxArray[2].transform.Find("WeaponIcon").gameObject.GetComponent<Image>();
        WeaponImageWhiteArray[3] = WeaponBoxArray[3].transform.Find("WeaponIcon").gameObject.GetComponent<Image>();

        //UpgradeWindowで使用するゲームオブジェクト、コンポーネントなどの取得
        SelectedUpgradeIcon = UpgradeWindow.transform.Find("SelectedUpgradeIcon").gameObject;
        UpgradeBoxArray0[0] = UpgradeWindow.transform.Find("UpgradeBox00").gameObject;
        UpgradeBoxArray0[1] = UpgradeWindow.transform.Find("UpgradeBox01").gameObject;
        UpgradeBoxArray0[2] = UpgradeWindow.transform.Find("UpgradeBox02").gameObject;
        UpgradeBoxArray1[0] = UpgradeWindow.transform.Find("UpgradeBox10").gameObject;
        UpgradeBoxArray1[1] = UpgradeWindow.transform.Find("UpgradeBox11").gameObject;
        UpgradeBoxArray1[2] = UpgradeWindow.transform.Find("UpgradeBox12").gameObject;
        UpgradeFactor[0, 0] = UpgradeBoxArray0[0].transform.Find("UpgradeFactor00").gameObject;
        UpgradeFactor[0, 1] = UpgradeBoxArray0[0].transform.Find("UpgradeFactor01").gameObject;
        UpgradeFactor[0, 2] = UpgradeBoxArray0[0].transform.Find("UpgradeFactor02").gameObject;
        UpgradeFactor[1, 0] = UpgradeBoxArray0[1].transform.Find("UpgradeFactor00").gameObject;
        UpgradeFactor[1, 1] = UpgradeBoxArray0[1].transform.Find("UpgradeFactor01").gameObject;
        UpgradeFactor[1, 2] = UpgradeBoxArray0[1].transform.Find("UpgradeFactor02").gameObject;
        UpgradeFactor[2, 0] = UpgradeBoxArray0[2].transform.Find("UpgradeFactor00").gameObject;
        UpgradeFactor[2, 1] = UpgradeBoxArray0[2].transform.Find("UpgradeFactor01").gameObject;
        UpgradeFactor[2, 2] = UpgradeBoxArray0[2].transform.Find("UpgradeFactor02").gameObject;
        UpgradeFactorSprite = UpgradeFactorSpriteTemp;

        //FeetWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        FeetLavel = FeetWindow.transform.Find("FeetLavel").gameObject;
        feetLavelText = FeetLavel.GetComponent<Text>();
        SelectedFeetIcon = FeetWindow.transform.Find("SelectedFeetIcon").gameObject;
        selectedFeetIconAnim = SelectedFeetIcon.GetComponent<Animator>();
        UseFeetWindow = FeetWindow.transform.Find("UseFeetWindow").gameObject;
        UseFeetIcon = UseFeetWindow.transform.Find("UseFeetIcon").gameObject;
        useFeetIconAnim = UseFeetIcon.GetComponent<Animator>();
        UseFeetLavel = UseFeetWindow.transform.Find("UseFeetLavel").gameObject;
        UseFeetLavelArray[0] = UseFeetLavel.transform.Find("UseFeetLavel00").gameObject;
        UseFeetLavelArray[1] = UseFeetLavel.transform.Find("UseFeetLavel01").gameObject;
        UseFeetLavelArray[2] = UseFeetLavel.transform.Find("UseFeetLavel02").gameObject;
        UseFeetLavelArray[3] = UseFeetLavel.transform.Find("UseFeetLavel03").gameObject;
        UseFeetLavelArray[4] = UseFeetLavel.transform.Find("UseFeetLavel04").gameObject;

        //SwapWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        SwapWindow = transform.Find("SwapWindow").gameObject;
        SelectedSwapIcon = SwapWindow.transform.Find("SelectedSwapIcon").gameObject;
        selectedSwapIconAnim = SelectedSwapIcon.GetComponent<Animator>();
        SwapLavel = SwapWindow.transform.Find("SwapLavel").gameObject;
        SwapLavelArray[0] = SwapLavel.transform.Find("SwapLavel00").gameObject;
        SwapLavelArray[1] = SwapLavel.transform.Find("SwapLavel01").gameObject;
        SwapLavelArray[2] = SwapLavel.transform.Find("SwapLavel02").gameObject;
        SwapLavelArray[3] = SwapLavel.transform.Find("SwapLavel03").gameObject;
        SwapLavelArray[4] = SwapLavel.transform.Find("SwapLavel04").gameObject;
        SwapLavelArray[5] = SwapLavel.transform.Find("SwapLavel05").gameObject;
        SwapLavelArray[6] = SwapLavel.transform.Find("SwapLavel06").gameObject;
        SwapLavelArray[7] = SwapLavel.transform.Find("SwapLavel07").gameObject;
        SwapLavelArray[8] = SwapLavel.transform.Find("SwapLavel08").gameObject;
        SwapLavelArray[9] = SwapLavel.transform.Find("SwapLavel09").gameObject;
        swapLavelTextArray[0] = SwapLavelArray[0].GetComponent<Text>();
        swapLavelTextArray[1] = SwapLavelArray[1].GetComponent<Text>();
        swapLavelTextArray[2] = SwapLavelArray[2].GetComponent<Text>();
        swapLavelTextArray[3] = SwapLavelArray[3].GetComponent<Text>();
        swapLavelTextArray[4] = SwapLavelArray[4].GetComponent<Text>();
        swapLavelTextArray[5] = SwapLavelArray[5].GetComponent<Text>();
        swapLavelTextArray[6] = SwapLavelArray[6].GetComponent<Text>();
        swapLavelTextArray[7] = SwapLavelArray[7].GetComponent<Text>();
        swapLavelTextArray[8] = SwapLavelArray[8].GetComponent<Text>();
        swapLavelTextArray[9] = SwapLavelArray[9].GetComponent<Text>();

        //ScoutWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        ScoutFloor = ScoutWindow.transform.Find("ScoutFloor").gameObject;
        ScoutObject = ScoutWindow.transform.Find("ScoutObject").gameObject;
        ScoutAttention = ScoutWindow.transform.Find("ScoutAttention").gameObject;
        ScoutAttentionAlpha = ScoutAttention.GetComponent<Text>().color.a;

        //LevelWindowで使用するゲームオブジェクト、コンポーネント等の取得、設定
        LevelText = LevelWindow.transform.Find("LevelText").gameObject;
        LevelTextText = LevelText.GetComponent<Text>();

        //ゲーム開始時点では各ウィンドウを非表示にする
        InfoWindow.SetActive(false);
        MessageWindow.SetActive(false);
        ComfirmWindow.SetActive(false);
        CommandWindow.SetActive(false);
        StatusWindow.SetActive(false);
        ItemWindow.SetActive(false);
        WeaponWindow.SetActive(false);
        UpgradeWindow.SetActive(false);
        FeetWindow.SetActive(false);
        OtherWindow.SetActive(false);
        ScoutWindow.SetActive(false);
        AbortWindow.SetActive(false);
    }

    void Update()
    {
        scaleX = GetComponent<RectTransform>().localScale.x;
        scaleY = GetComponent<RectTransform>().localScale.y;
    }

    //遅延処理後にメッセージウィンドウを非表示状態にする連続処理関数関数
    public static IEnumerator SetWindowInactiveWithDelay(int waitCount)
    {
        //引数の時間に従い遅延処理を行う
        messageWindowInactiveCount = GameManager.frameCount + 90;
        
        //メッセージウィンドウを非表示状態にする
        while (GameManager.frameCount < messageWindowInactiveCount)
        {
            yield return null;
        }
        MessageWindow.SetActive(false);
    }

    //メッセージテキストを表示する関数
    public static void DisplayMessageText()
    {
        //メッセージウィンドウを有効化する
        MessageWindow.SetActive(true);

        //メッセージテキストを表示する
        messageTextScript.text = stackedMessageList[0];

        //今表示したメッセージリストの要素を消去する
        stackedMessageList.RemoveAt(0);

        //もしまだメッセージリストの要素が残っている場合、ページ送り画像を有効化し次のメッセージ表示を促す
        if (stackedMessageList.Count > 0)
        {
            NextPageIcon.SetActive(true);
        }
        //そうでない場合、メッセージスタッキング真偽値を偽にし、ページ送り画像を無効化する
        else
        {
            if(NextPageIcon.activeSelf == true)
            {
                GameManager.keyInputRestrictionCount = 24;

                NextPageIcon.SetActive(false);
            }
            GameManager.messageIsStacking = false;
        }
    }

    //引数のメッセージを蓄積メッセージテキストに追加する関数
    public static void StackMessageText(string displayMessageText)
    {
        //引数のメッセージテキストを蓄積メッセージリストに追加する
        stackedMessageList.Add(displayMessageText);

        //メッセージスタッキング真偽値を真にする
        GameManager.messageIsStacking = true;
    }

    //ダメージメッセージを作成する関数
    public static void MakeDamageText(string attackUnitName, string damagedUnitName, int damage)
    {
        //与えられた引数に従ってメッセージを作成する
        string displayMessageText = attackUnitName + "は" + damagedUnitName + "に\n" +
                    damage.ToString() + "のダメージを与えた。";

        //作成したメッセージを蓄積メッセージテキストに追加する
        StackMessageText(displayMessageText);
    }

    //死亡メッセージを作成する関数
    public static void MakeDeathText(string attackUnitName, string damagedUnitName)
    {
        //与えられた引数に従ってメッセージを表示する
        string displayMessageText = damagedUnitName + "をやっつけた\n";

        //作成したメッセージを蓄積メッセージテキストに追加する
        StackMessageText(displayMessageText);
    }

    public static void MakeGetItemText(string itemName)
    {
        string displayMessageText = itemName + "を拾った\n";

        StackMessageText(displayMessageText);
    }

    public static void MakeUseItemText(string itemName)
    {
        string displayMessageText = itemName + "を使った\n";
        StackMessageText(displayMessageText);
    }

    public static void MakeDropItemText(string itemName)
    {
        string displayMessageText = itemName + "を置いた\n";
        StackMessageText(displayMessageText);
    }

    //
    public static void CommandComfirmWindow()
    {
        if(comfirmTheme == ComfirmTheme.UseStair)
        {
            if (InputFunction.GetKeyDown("UpArrow") || InputFunction.GetKeyDown("DownArrow"))
            {
                comfirmSelectIconIndex = (comfirmSelectIconIndex + 1) % 2;
                ComfirmSelectIconAnim.SetTrigger("triggerRestart");
            }
            else if (InputFunction.GetKeyDown("D"))
            {
                if (comfirmSelectIconIndex == 1)
                {
                    ComfirmWindow.SetActive(false);
                    MessageWindow.SetActive(false);
                    GameManager.keyInputRestrictionCount += 30;
                }
                comfirmSelectIconIndex = 1;
                ComfirmSelectIconAnim.SetTrigger("triggerRestart");
            }
            else if (InputFunction.GetKeyDown("A"))
            {
                if (comfirmSelectIconIndex == 0)
                {
                    GameManager.floorLevel += 1;
                    GameManager.CleanupAllObject();
                    GameManager.isNextFloor = true;
                    LevelWindow.SetActive(true);
                    LevelTextText.text = "TEST Dungeon" + 
                                         "\n" +
                                         "\nFloor " + GameManager.floorLevel.ToString();
                    GameManager.keyInputRestrictionCount += 75;
                }

                //
                ComfirmWindow.SetActive(false);
                MessageWindow.SetActive(false);
                GameManager.keyInputRestrictionCount += 30;
            }

            if (comfirmSelectIconIndex == 0)
            {
                ComfirmSelectIcon.transform.position = new Vector2(ComfirmSelectIcon.transform.position.x, Yes.transform.position.y);
            }
            else
            {
                ComfirmSelectIcon.transform.position = new Vector2(ComfirmSelectIcon.transform.position.x, No.transform.position.y);
            }
        }
    }

    //
    public static void SelectCommand()
    {
        //Xが押された場合、コマンドウィンドウを非表示にし、処理を終了する
        if (InputFunction.GetKeyDown("D") == true || InputFunction.GetKeyDown("C") == true)
        {
            selectedCommandIndex = 0;
            CommandWindow.SetActive(false);
            StatusWindow.SetActive(false);
        }
        //キー入力に従って選んでいるコマンド要素を変更し、選択アイコンの位置をコマンド要素の高さに変更する
        if (InputFunction.GetKeyDown("UpArrow") == true)
        {
            selectedCommandIconAnim.SetTrigger("triggerRestart");
            selectedCommandIndex--;
        }
        if (InputFunction.GetKeyDown("DownArrow") == true)
        {
            selectedCommandIconAnim.SetTrigger("triggerRestart");
            selectedCommandIndex++;
        }
        selectedCommandIndex = UtilityFunction.Mod(selectedCommandIndex, CommandLavelArray.Length);
        SelectedCommandIcon.transform.position = new Vector3(SelectedCommandIcon.transform.position.x, CommandLavelArray[selectedCommandIndex].transform.position.y, 0f);

        if (InputFunction.GetKeyDown("S") == true || InputFunction.GetKeyDown("A") == true)
        {
            CommandWindow.SetActive(false);
            StatusWindow.SetActive(false);
            switch (selectedCommandIndex)
            {
                //Item
                case 0:
                    if (ItemController.ItemInBackpackList.Count == 0)
                    {
                        SelectedItemIcon.SetActive(false);
                        NoItemText.SetActive(true);

                        for (int i = 0; i < ItemLavelArray.Length; i++)
                        {
                            ItemLavelTextArray[i].text = "";
                        }
                    }
                    else
                    {
                        SelectedItemIcon.SetActive(true);
                        NoItemText.SetActive(false);

                        for (int i = 0; i < ItemLavelArray.Length; i++)
                        {
                            if (i < ItemController.ItemInBackpackList.Count)
                            {
                                ItemLavelTextArray[i].text = ItemController.ItemInBackpackList[i].itemName;
                            }
                            else
                            {
                                ItemLavelTextArray[i].text = "";
                            }
                        }

                    }
                    ItemWindow.SetActive(true);
                    break;

                //Weapon
                case 1:
                    WeaponWindow.SetActive(true);
                    break;


                //Feet
                case 2:
                    RaycastHit2D hit = new RaycastHit2D();
                    bool b = BoardManager.ExistLayer(Unit.unitList[0].transform.position, "ItemLayer", out hit);
                    if(b == false)
                    {
                        selectedCommandIndex = 0;
                        SelectedCommandIcon.transform.position = new Vector3(SelectedCommandIcon.transform.position.x, CommandLavelArray[selectedCommandIndex].transform.position.y, 0f);
                        StackMessageText("足元に何も置かれていない");
                        selectedItemIndex = 0;
                        ItemWindow.SetActive(false);
                        GameManager.keyInputRestrictionCount = 30;
                        return;
                    }
                    else
                    {
                        feetLavelText.text = hit.collider.gameObject.GetComponent<ItemController>().itemName;
                        FeetWindow.SetActive(true);
                    }

                    break;

                //Other
                case 3:
                    OtherWindow.SetActive(true);
                    break;

                //Scout
                case 4:
                    ScoutWindow.SetActive(true);
                    MyCameraController.SetupReferenceObject(ScoutObject);
                    break;

                //Abort
                case 5:
                    AbortWindow.SetActive(true);
                    break;
            }
        }
    }

    //Item
    public static void CommandItemWindow()
    {
        if (UseItemWindow.activeSelf == false)
        {
            if(itemPageIndex == 0 && (ItemController.ItemInBackpackList.Count > ItemController.maxItemAmountPerPage))
            {
                NextItemPageIconRight.SetActive(true);
            }
            else
            {
                NextItemPageIconRight.SetActive(false);
            }
            if(itemPageIndex == 1)
            {
                NextItemPageIconLeft.SetActive(true);
            }
            else
            {
                NextItemPageIconLeft.SetActive(false);
            }

            //Xキーが押された場合、ウィンドウを閉じる
            if (InputFunction.GetKeyDown("D") == true)
            {
                itemPageIndex = 0;
                selectedItemIndex = 0;
                NextItemPageIconLeft.SetActive(false);
                CommandWindow.SetActive(true);
                StatusWindow.SetActive(true);
                ItemWindow.SetActive(false);
            }

            else if (InputFunction.GetKeyDown("C") == true)
            {
                itemPageIndex = 0;
                selectedItemIndex = 0;
                NextItemPageIconLeft.SetActive(false);
                ItemWindow.SetActive(false);
            }

            if (ItemController.ItemInBackpackList.Count == 0)
            {
                return;
            }
            
            if (InputFunction.GetKeyDown("UpArrow") == true)
            {
                selectedItemIconAnim.SetTrigger("triggerRestart");
                selectedItemIndex--;
            }
            else if (InputFunction.GetKeyDown("DownArrow") == true)
            {
                selectedItemIconAnim.SetTrigger("triggerRestart");
                selectedItemIndex++;
            }
            else if (InputFunction.GetKeyDown("RightArrow") == true)
            {
                if(itemPageIndex == 0 && (ItemController.ItemInBackpackList.Count > ItemController.maxItemAmountPerPage))
                {
                    itemPageIndex = 1;
                    selectedItemIndex = 0;
                    for (int i = 0; i < ItemLavelArray.Length; i++)
                    {
                        if (i + ItemController.maxItemAmountPerPage < ItemController.ItemInBackpackList.Count)
                        {
                            ItemLavelTextArray[i].text = ItemController.ItemInBackpackList[i + ItemController.maxItemAmountPerPage].itemName;
                        }
                        else
                        {
                            ItemLavelTextArray[i].text = "";
                        }
                    }
                    selectedItemIconAnim.SetTrigger("triggerRestart");
                }
            }
            else if(InputFunction.GetKeyDown("LeftArrow") == true)
            {
                if (itemPageIndex == 1)
                {
                    itemPageIndex = 0;
                    selectedItemIndex = 0;
                    for (int i = 0; i < ItemLavelArray.Length; i++)
                    {
                        ItemLavelTextArray[i].text = ItemController.ItemInBackpackList[i].itemName;
                    }
                    selectedItemIconAnim.SetTrigger("triggerRestart");
                }
            }

            //キー入力に従って選んでいるコマンド要素を変更し、選択アイコンの位置をコマンド要素の高さに変更する
            int divisor = Mathf.Min((ItemController.ItemInBackpackList.Count - itemPageIndex * ItemController.maxItemAmountPerPage), ItemController.maxItemAmountPerPage );
            selectedItemIndex = UtilityFunction.Mod(selectedItemIndex, divisor);
            SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);

            if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
            {
                UseItemWindow.SetActive(true);
                selectedItemIconAnim.SetBool("ItemIsSelecting", true);
            }
        }
        else
        {
            if (InputFunction.GetKeyDown("UpArrow") == true)
            {
                useItemIconAnim.SetTrigger("triggerRestart");
                useItemIndex--;
            }
            if (InputFunction.GetKeyDown("DownArrow") == true)
            {
                useItemIconAnim.SetTrigger("triggerRestart");
                useItemIndex++;
            }
            useItemIndex = UtilityFunction.Mod(useItemIndex, UseItemLavelArray.Length);
            UseItemIcon.transform.position = new Vector3(UseItemIcon.transform.position.x, UseItemLavelArray[useItemIndex].transform.position.y, 0f);


            //Xキーが押された場合、ウィンドウを閉じる
            if (InputFunction.GetKeyDown("D") == true)
            {
                selectedItemIconAnim.SetBool("ItemIsSelecting", false);
                useItemIndex = 0;
                UseItemIcon.transform.position = new Vector3(UseItemIcon.transform.position.x, UseItemLavelArray[useItemIndex].transform.position.y, 0f);
                UseItemWindow.SetActive(false);
            }
            if (InputFunction.GetKeyDown("C") == true)
            {
                selectedItemIconAnim.SetBool("ItemIsSelecting", false);
                useItemIndex = 0;
                UseItemIcon.transform.position = new Vector3(UseItemIcon.transform.position.x, UseItemLavelArray[useItemIndex].transform.position.y, 0f);
                UseItemWindow.SetActive(false);
                itemPageIndex = 0;
                selectedItemIndex = 0;
                SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                ItemWindow.SetActive(false);
            }
            else if(InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
            {
                GameManager.keyInputRestrictionCount = 30;
                switch (useItemIndex)
                {
                    //Use
                    case 0:
                        Unit.unitList[0].UseItem(ItemController.ItemInBackpackList[selectedItemIndex]);
                        itemPageIndex = 0;
                        selectedItemIndex = 0;
                        NextItemPageIconLeft.SetActive(false);
                        SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        ItemWindow.SetActive(false);
                        useItemIndex = 0;
                        UseItemWindow.SetActive(false);
                        break;

                    //Throw
                    case 1:
                        Unit.unitList[0].ThrowItem(ItemController.ItemInBackpackList[selectedItemIndex]);
                        itemPageIndex = 0;
                        selectedItemIndex = 0;
                        NextItemPageIconLeft.SetActive(false);
                        SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        ItemWindow.SetActive(false);
                        useItemIndex = 0;
                        UseItemIcon.transform.position = new Vector3(UseItemIcon.transform.position.x, UseItemLavelArray[useItemIndex].transform.position.y, 0f);
                        UseItemWindow.SetActive(false);
                        break;

                    //Drop
                    case 2:
                        Unit.unitList[0].TryToDropItem(ItemController.ItemInBackpackList[selectedItemIndex]);
                        itemPageIndex = 0;
                        selectedItemIndex = 0;
                        NextItemPageIconLeft.SetActive(false);
                        SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        ItemWindow.SetActive(false);
                        useItemIndex = 0;
                        UseItemIcon.transform.position = new Vector3(UseItemIcon.transform.position.x, UseItemLavelArray[useItemIndex].transform.position.y, 0f);
                        UseItemWindow.SetActive(false);
                        break;

                    //Swap
                    case 3:
                        RaycastHit2D hit = new RaycastHit2D();
                        bool b = BoardManager.ExistLayer(Unit.unitList[0].transform.position, "ItemLayer", out hit);
                        if(b == true)
                        {
                            swapItem = hit.collider.gameObject.GetComponent<ItemController>();
                            Unit.unitList[0].SwapItem(swapItem, ItemController.ItemInBackpackList[selectedItemIndex]);
                        }
                        else
                        {
                            StackMessageText("足元に交換できるアイテムが置かれていない");
                        }
                        itemPageIndex = 0;
                        selectedItemIndex = 0;
                        NextItemPageIconLeft.SetActive(false);
                        SelectedItemIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        ItemWindow.SetActive(false);
                        useItemIndex = 0;
                        UseItemWindow.SetActive(false);
                        break;

                    //Return
                    case 4:
                        useItemIndex = 0;
                        UseItemWindow.SetActive(false);
                        selectedItemIconAnim.SetBool("ItemIsSelecting", false);
                        break;

                    default:
                        Debug.Log("ERROR: CanvasController.CommandItemWindow");
                        break;
                }
            }
        }
    }

    //public static void SetWeaponWindow()
    //{
    //    SetWeaponWindow()
    //}

    //Weapon
    public static void CommandWeaponWindow()
    {
        for (int i = 0; i < WeaponImageWhiteArray.Length; i++)
        {
            WeaponImageWhiteArray[i].sprite = WeaponController.EquippedWeapon[i].IconWhiteback;
        }

        if (InputFunction.AnyKey() == true)
        {
            if (InputFunction.GetKeyDown("RightArrow") == true || InputFunction.GetKeyDown("LeftArrow") == true)
            {
                if (selectedWeaponIndex % 2 == 0)
                {
                    selectedWeaponIndex += 1;
                }
                else
                {
                    selectedWeaponIndex -= 1;
                }
            }
            else if (InputFunction.GetKeyDown("UpArrow") == true || InputFunction.GetKeyDown("DownArrow") == true)
            {
                selectedWeaponIndex += 2;
            }

            //Xキーが押された場合、ウィンドウを閉じる
            else if (InputFunction.GetKeyDown("D") == true)
            {
                selectedWeaponIndex = 0;
                CommandWindow.SetActive(true);
                StatusWindow.SetActive(true);
                WeaponWindow.SetActive(false);
            }

            else if (InputFunction.GetKeyDown("C") == true)
            {
                selectedWeaponIndex = 0;
                WeaponWindow.SetActive(false);
            }

            else if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S"))
            {
                WeaponWindow.SetActive(false);
                UpgradeWindow.SetActive(true);
                SetUpgradeWindow();
            }

            else if (InputFunction.GetKeyDown("Z") == true)
            {
                weaponSwapIndexFormer = selectedWeaponIndex;
            }


            selectedWeaponIndex = UtilityFunction.Mod(selectedWeaponIndex, WeaponBoxArray.Length);

            for (int i = 0; i < WeaponBoxArray.Length; i++)
            {
                WeaponBoxArray[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            }
            WeaponBoxArray[selectedWeaponIndex].GetComponent<Image>().color = new Color(0f, 0.7f, 1f, 1f);
        }
    }

    public static void SetUpgradeWindow()
    {
        //アップグレードボックスの脚色
        for (int i = 0; i < UpgradeFactor.GetLength(0); i++)
        {   
            //アビリティボックスの脚色
            for (int j = 0; j < UpgradeFactor.GetLength(1); j++)
            {
                //レベルボックスの脚色
                if (selectedUpgradeLevelIndex != new Vector2Int(i, j) || selectedUpgradeFactor != "level")
                {
                    if (UpgradeFactorState[i, j] == "powered")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[0];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "selected")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[2];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "selectable")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[4];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "locked")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[6];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                    }
                }
                else
                {
                    if (UpgradeFactorState[i, j] == "powered")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[1];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "selected")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[3];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "selectable")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[5];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (UpgradeFactorState[i, j] == "locked")
                    {
                        UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[7];
                        UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                    }
                }
            }
        }

    }

    public static void CommandUpgradeWindow()
    {
        if (InputFunction.AnyKey() == true)
        {
            if (selectedUpgradeFactor == "ability")
            {
                if (InputFunction.GetKeyDown("RightArrow") == true)
                {
                    selectedUpgradeIndex++;
                }
                else if (InputFunction.GetKeyDown("LeftArrow") == true)
                {
                    selectedUpgradeIndex--;
                }
                selectedUpgradeIndex = UtilityFunction.Mod(selectedUpgradeIndex, UpgradeBoxArray0.Length);
                

                if (InputFunction.GetKeyDown("UpArrow") == true || InputFunction.GetKeyDown("DownArrow") == true)
                {
                    selectedUpgradeFactor = "command";
                }
                else if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
                {
                    selectedUpgradeFactor = "level";
                    selectedUpgradeLevelIndex = new Vector2Int(selectedUpgradeIndex, 0);
                }
                else if (InputFunction.GetKeyDown("C") == true)
                {
                    upgradeIsReady = false;
                    selectedUpgradeIndex = 0;
                    selectedWeaponIndex = 0;
                    UpgradeWindow.SetActive(false);
                }
                else if (InputFunction.GetKeyDown("D") == true)
                {
                    if (upgradeIsReady == false)
                    {
                        selectedUpgradeFactor = "ability";
                        selectedUpgradeIndex = 0;
                        commandUpgradeIndex = 0;
                        UpgradeWindow.SetActive(false);
                        WeaponWindow.SetActive(true);
                    }
                    else
                    {
                        selectedUpgradeFactor = "command";
                        selectedUpgradeIndex = 1;
                        commandUpgradeIndex = 1;
                    }
                }

                if(upgradeIsReady == true)
                {
                    commandUpgradeIndex = selectedUpgradeIndex;
                }

                //アビリティボックスの脚色
                for (int i = 0; i < UpgradeBoxArray0.Length; i++)
                {
                    UpgradeBoxArray0[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                }
                if (selectedUpgradeFactor != "command")
                {
                    UpgradeBoxArray0[selectedUpgradeIndex].GetComponent<Image>().color = new Color(1f, 0.65f, 0f, 1f);
                }
            }

            //
            else if(selectedUpgradeFactor == "command")
            {
                if (InputFunction.GetKeyDown("RightArrow") == true)
                {
                    commandUpgradeIndex++;
                }
                else if (InputFunction.GetKeyDown("LeftArrow") == true)
                {
                    commandUpgradeIndex--;
                }
                commandUpgradeIndex = UtilityFunction.Mod(commandUpgradeIndex, UpgradeBoxArray0.Length);

                if (InputFunction.GetKeyDown("UpArrow") == true || InputFunction.GetKeyDown("DownArrow") == true)
                {
                    if (upgradeIsReady == false)
                    {
                        commandUpgradeIndex = 0;
                    }
                    selectedUpgradeFactor = "ability";
                }
                else if (InputFunction.GetKeyDown("C") == true)
                {
                    upgradeIsReady = false;
                    selectedUpgradeFactor = "ability";
                    selectedUpgradeIndex = 0;
                    commandUpgradeIndex = 0;
                    selectedWeaponIndex = 0;
                    UpgradeWindow.SetActive(false);
                }
                else if (InputFunction.GetKeyDown("D") == true)
                {
                    if(upgradeIsReady == false)
                    {
                        selectedUpgradeFactor = "ability";
                        selectedUpgradeIndex = 0;
                        commandUpgradeIndex = 0;
                        UpgradeWindow.SetActive(false);
                        WeaponWindow.SetActive(true);
                    }
                    else
                    {
                        selectedUpgradeFactor = "command";
                        selectedUpgradeIndex = 1;
                        commandUpgradeIndex = 1;
                    }
                }
                else if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
                {
                    //Confirm
                    if(commandUpgradeIndex == 0)
                    {
                        for(int i = 0; i < UpgradeFactorState.GetLength(0); i++)
                        {
                            for(int j = UpgradeFactorState.GetLength(1) - 1; j >= 0; j--)
                            {
                                if(UpgradeFactorState[i, j] == "selected")
                                {
                                    UpgradeFactorState[i, j] = "powered";
                                }
                            }
                        }
                        upgradeIsReady = false;
                    }
                    //Undo
                    else if(commandUpgradeIndex == 1)
                    {
                        for (int i = 0; i < UpgradeFactorState.GetLength(0); i++)
                        {
                            for (int j = UpgradeFactorState.GetLength(1) - 1; j >= 0; j--)
                            {
                                if (UpgradeFactorState[i, j] == "selected")
                                {
                                    UpgradeFactorState[i, j] = "selectable";
                                    if (j != UpgradeFactorState.GetLength(1) - 1)
                                    {
                                        UpgradeFactorState[i, j + 1] = "locked";
                                    }
                                }
                            }
                        }
                        upgradeIsReady = false;
                    }
                    //Back
                    else
                    {
                        if (upgradeIsReady == false)
                        {
                            selectedUpgradeFactor = "ability";
                            selectedUpgradeIndex = 0;
                            commandUpgradeIndex = 0;
                            UpgradeWindow.SetActive(false);
                            WeaponWindow.SetActive(true);
                        }
                    }
                }

                if(upgradeIsReady == false)
                {
                    commandUpgradeIndex = UpgradeBoxArray1.Length - 1;
                }


                //コマンドボックスの脚色
                if (upgradeIsReady == true)
                {
                    for (int i = 0; i < UpgradeBoxArray1.Length - 1; i++)
                    {
                        UpgradeBoxArray1[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        UpgradeBoxArray1[i].transform.Find("Lavel").gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    }
                    UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);

                    if (selectedUpgradeFactor == "command")
                    {
                        UpgradeBoxArray1[commandUpgradeIndex].GetComponent<Image>().color = new Color(1f, 0.65f, 0f, 1f);
                    }
                    selectedUpgradeIndex = commandUpgradeIndex;
                }
                else
                {
                    for (int i = 0; i < UpgradeBoxArray1.Length - 1; i++)
                    {
                        UpgradeBoxArray1[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
                        UpgradeBoxArray1[i].transform.Find("Lavel").gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.4f);
                    }
                    UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                    UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].transform.Find("Lavel").gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);

                    if (selectedUpgradeFactor == "command")
                    {
                        UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].GetComponent<Image>().color = new Color(1f, 0.65f, 0f, 1f);
                    }
                }
            }
            
            //level
            else if(selectedUpgradeFactor == "level")
            {
                string selectedFactorState = UpgradeFactorState[selectedUpgradeLevelIndex.x, selectedUpgradeLevelIndex.y];
                if (InputFunction.GetKeyDown("DownArrow") == true)
                {
                    selectedUpgradeLevelIndex = selectedUpgradeLevelIndex + Vector2Int.up;
                }
                else if (InputFunction.GetKeyDown("UpArrow") == true)
                {
                    selectedUpgradeLevelIndex = selectedUpgradeLevelIndex + Vector2Int.down;
                }
                else if (InputFunction.GetKeyDown("D") == true)
                {
                    selectedUpgradeFactor = "ability";
                }
                else if (InputFunction.GetKeyDown("A") ==  true || InputFunction.GetKeyDown("S") == true)
                {
                    if(selectedFactorState == "selectable")
                    {
                        UpgradeFactorState[selectedUpgradeLevelIndex.x, selectedUpgradeLevelIndex.y] = "selected";

                        if(selectedUpgradeLevelIndex.y + 1 < UpgradeFactor.GetLength(1))
                        {
                            UpgradeFactorState[selectedUpgradeLevelIndex.x, selectedUpgradeLevelIndex.y+1] = "selectable";
                        }

                        upgradeIsReady = true;
                        for (int i = 0; i < UpgradeBoxArray1.Length; i++)
                        {
                            UpgradeBoxArray1[i].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                            UpgradeBoxArray1[i].transform.Find("Lavel").gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.4f);
                        UpgradeBoxArray1[UpgradeBoxArray1.Length - 1].transform.Find("Lavel").gameObject.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.4f);

                        if (selectedUpgradeFactor == "command")
                        {
                            UpgradeBoxArray1[commandUpgradeIndex].GetComponent<Image>().color = new Color(1f, 0.65f, 0f, 1f);
                        }
                    }
                }

                int numTemp = UpgradeFactor.GetLength(1);
                selectedUpgradeLevelIndex = new Vector2Int((selectedUpgradeLevelIndex.x + numTemp) % numTemp, (selectedUpgradeLevelIndex.y + numTemp) % numTemp);

                //アップグレードボックスの脚色
                for (int i = 0; i < UpgradeFactor.GetLength(0); i++)
                {
                    if(selectedUpgradeLevelIndex.x != i)
                    {
                        UpgradeBoxArray0[i].GetComponent<Image>().color = new Color(1f, 1f, 1f);
                    }
                    else
                    {
                        UpgradeBoxArray0[i].GetComponent<Image>().color = new Color(1f, 0f, 0f);
                    }

                    ////アビリティボックスの脚色
                    //for (int j = 0; j < UpgradeFactor.GetLength(1); j++)
                    //{
                    //    //レベルボックスの脚色
                    //    if (selectedUpgradeLevelIndex != new Vector2Int(i, j) || selectedUpgradeFactor != "level")
                    //    {
                    //        if (UpgradeFactorState[i, j] == "powered")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[0];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "selected")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[2];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "selectable")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[4];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "locked")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[6];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (UpgradeFactorState[i, j] == "powered")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[1];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "selected")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[3];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "selectable")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[5];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                    //        }
                    //        else if (UpgradeFactorState[i, j] == "locked")
                    //        {
                    //            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[7];
                    //            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                    //        }
                    //    }
                    //}
                }



            }
            //アップグレードボックスの脚色
            for (int i = 0; i < UpgradeFactor.GetLength(0); i++)
            {
                //アビリティボックスの脚色
                for (int j = 0; j < UpgradeFactor.GetLength(1); j++)
                {
                    //レベルボックスの脚色
                    if (selectedUpgradeLevelIndex != new Vector2Int(i, j) || selectedUpgradeFactor != "level")
                    {
                        if (UpgradeFactorState[i, j] == "powered")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[0];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "selected")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[2];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "selectable")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[4];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "locked")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[6];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                        }
                    }
                    else
                    {
                        if (UpgradeFactorState[i, j] == "powered")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[1];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "selected")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[3];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "selectable")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[5];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (UpgradeFactorState[i, j] == "locked")
                        {
                            UpgradeFactor[i, j].GetComponent<Image>().sprite = UpgradeFactorSprite[7];
                            UpgradeFactor[i, j].transform.Find("Lavel").GetComponent<Text>().color = new Color(0.65f, 0.65f, 0.65f, 1f);
                        }
                    }
                }
            }



        }
    }
    
    //Feet
    public static void CommandFeetWindow()
    {
        if (UseFeetWindow.activeSelf == false)
        {
            RaycastHit2D hit = new RaycastHit2D();
            BoardManager.ExistLayer(Unit.unitList[0].transform.position, "ItemLayer", out hit);

            //Xキーが押された場合、ウィンドウを閉じる
            if (InputFunction.GetKeyDown("D") == true)
            {
                selectedFeetIconAnim.SetBool("ItemIsSelecting", true);
                CommandWindow.SetActive(true);
                StatusWindow.SetActive(true);
                FeetWindow.SetActive(false);
            }
            else if (InputFunction.GetKeyDown("C") == true)
            {
                selectedCommandIndex = 0;
                selectedFeetIconAnim.SetBool("ItemIsSelecting", false);
                FeetWindow.SetActive(false);
            }

            else if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
            {
                if(hit.collider.gameObject.tag == "Stair")
                {
                    FeetWindow.SetActive(false);
                    CanvasController.StackMessageText("Go to next floor?");
                    GameManager.activateComfirmWindow = true;
                    CanvasController.comfirmSelectIconIndex = 0;
                    CanvasController.comfirmTheme = CanvasController.ComfirmTheme.UseStair;

                    return;
                }
                selectedFeetIconAnim.SetBool("ItemIsSelecting", true);
                UseFeetWindow.SetActive(true);
            }
        }
        else
        {
            //Xキーが押された場合、ウィンドウを閉じる
            if (InputFunction.GetKeyDown("D") == true)
            {
                selectedFeetIconAnim.SetBool("ItemIsSelecting", false);
                useFeetIndex = 0;
                UseFeetWindow.SetActive(false);
            }
            else if (InputFunction.GetKeyDown("C") == true)
            {;
                selectedFeetIconAnim.SetBool("ItemIsSelecting", false);
                useFeetIndex = 0;
                UseFeetWindow.SetActive(false);
                FeetWindow.SetActive(false);
            }

            else if (InputFunction.GetKeyDown("UpArrow") == true)
            {
                useFeetIconAnim.SetTrigger("triggerRestart");
                useFeetIndex--;
            }

            else if (InputFunction.GetKeyDown("DownArrow") == true)
            {
                useFeetIconAnim.SetTrigger("triggerRestart");
                useFeetIndex++;
            }

            else if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
            {
                selectedFeetIconAnim.SetBool("ItemIsSelecting", true);

                RaycastHit2D hit = new RaycastHit2D();
                BoardManager.ExistLayer(Unit.unitList[0].transform.position, "ItemLayer", out hit);
                ItemController item = hit.collider.gameObject.GetComponent<ItemController>();
                switch (useFeetIndex)
                {
                    //PickUp
                    case 0:
                        selectedCommandIndex = 0;
                        SelectedCommandIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        Unit.unitList[0].PickUpItem(item);
                        FeetWindow.SetActive(false);
                        useFeetIndex = 0;
                        UseFeetWindow.SetActive(false);
                        break;

                    //Use
                    case 1:
                        selectedCommandIndex = 0;
                        SelectedCommandIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        Unit.unitList[0].UseItem(item);
                        FeetWindow.SetActive(false);
                        useFeetIndex = 0;
                        UseFeetWindow.SetActive(false);
                        break;

                    //Throw
                    case 2:
                        selectedCommandIndex = 0;
                        SelectedCommandIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                        Unit.unitList[0].ThrowItem(item);
                        FeetWindow.SetActive(false);
                        useFeetIndex = 0;
                        UseFeetWindow.SetActive(false);
                        break;

                    //Swap
                    case 3:
                        if(ItemController.ItemInBackpackList.Count < 1)
                        {
                            selectedCommandIndex = 0;
                            SelectedCommandIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
                            StackMessageText("交換するアイテムをもっていない");
                            FeetWindow.SetActive(false);
                            useFeetIndex = 0;
                            UseFeetWindow.SetActive(false);
                            break;
                        }
                        for (int i = 0; i < SwapLavelArray.Length; i++)
                        {
                            if (i < ItemController.ItemInBackpackList.Count)
                            {
                                swapLavelTextArray[i].text = ItemController.ItemInBackpackList[i].itemName;
                            }
                            else
                            {
                                swapLavelTextArray[i].text = "";
                            }
                        }
                        FeetWindow.SetActive(false);
                        UseFeetWindow.SetActive(false);
                        SwapWindow.SetActive(true);
                        swapItem = item;
                        break;

                    //Return
                    case 4:
                        Debug.Log("Details");
                        useFeetIndex = 0;
                        UseFeetWindow.SetActive(false);
                        break;

                    default:
                        Debug.Log("ERROR: CanvasController.CommandItemWindow");
                        break;
                }
            }


            //キー入力に従って選んでいるコマンド要素を変更し、選択アイコンの位置をコマンド要素の高さに変更する
            useFeetIndex = UtilityFunction.Mod(useFeetIndex, UseFeetLavelArray.Length);
            UseFeetIcon.transform.position = new Vector3(UseFeetIcon.transform.position.x, UseFeetLavelArray[useFeetIndex].transform.position.y, 0f);
        }
    }

    //Swap
    public static void CommandSwapWindow()
    {
        //Xキーが押された場合、ウィンドウを閉じる
        if (InputFunction.GetKeyDown("D") == true)
        {
            SwapWindow.SetActive(false);
            FeetWindow.SetActive(true);
        }

        else if (InputFunction.GetKeyDown("C") == true)
        {
            selectedCommandIndex = 0;
            SelectedCommandIcon.transform.position = new Vector3(SelectedItemIcon.transform.position.x, ItemLavelArray[selectedItemIndex].transform.position.y, 0f);
            SwapWindow.SetActive(false);
        }

        if (ItemController.ItemInBackpackList.Count == 0)
        {
            return;
        }

        if (InputFunction.GetKeyDown("UpArrow") == true)
        {
            selectedSwapIconAnim.SetTrigger("triggerRestart");
            swapIndex--;
        }

        if (InputFunction.GetKeyDown("DownArrow") == true)
        {
            selectedSwapIconAnim.SetTrigger("triggerRestart");
            swapIndex++;
        }
        //キー入力に従って選んでいるコマンド要素を変更し、選択アイコンの位置をコマンド要素の高さに変更する
        swapIndex = UtilityFunction.Mod(swapIndex, ItemController.ItemInBackpackList.Count);
        SelectedSwapIcon.transform.position = new Vector3(SelectedSwapIcon.transform.position.x, SwapLavelArray[swapIndex].transform.position.y, 0f);

        if (InputFunction.GetKeyDown("A") == true || InputFunction.GetKeyDown("S") == true)
        {
            Unit.unitList[0].SwapItem(swapItem, ItemController.ItemInBackpackList[swapIndex]);
            SwapWindow.SetActive(false);
        }
    }

    //Other
    public static void CommandOtherWindow()
    {
        //Xキーが押された場合、ウィンドウを閉じる
        if (InputFunction.GetKeyDown("D") == true)
        {
            CommandWindow.SetActive(true);
            StatusWindow.SetActive(true);
            OtherWindow.SetActive(false);
        }
        else if (InputFunction.GetKeyDown("C") == true)
        {
            OtherWindow.SetActive(false);
        }
    }

    //Scout
    public static void CommandScoutWindow()
    {
        //Xキーが押された場合、ウィンドウを閉じる
        if (InputFunction.GetKeyDown("D") == true)
        {
            CommandWindow.SetActive(true);
            StatusWindow.SetActive(true);
            ScoutWindow.SetActive(false);
            MyCameraController.SetupReferenceObject(MyCameraController.Player);
        }
        else if (InputFunction.GetKeyDown("C") == true || InputFunction.GetKeyDown("V") == true)
        {
            ScoutWindow.SetActive(false);
            MyCameraController.ReferenceObject = GameObject.FindGameObjectWithTag("Player");
            MyCameraController.SetupReferenceObject(MyCameraController.Player);
            selectedCommandIndex = 0;
            SelectedCommandIcon.transform.position = new Vector3(SelectedCommandIcon.transform.position.x, CommandLavelArray[selectedCommandIndex].transform.position.y, 0f);
        }

        Vector3 testPosition = Vector3.zero;
        Vector2 d = BoardManager.outerRoomDelta - BoardManager.innerRoomDelta;

        if (InputFunction.GetKey("RightArrow") == true)
        {
            testPosition += new Vector3(0.25f, 0, 0);
        }
        if (InputFunction.GetKey("LeftArrow") == true)
        {
            testPosition -= new Vector3(0.25f, 0, 0);
        }
        if (InputFunction.GetKey("UpArrow") == true)
        {
            testPosition += new Vector3(0, 0.25f, 0);
        }
        if (InputFunction.GetKey("DownArrow") == true)
        {
            testPosition -= new Vector3(0, 0.25f, 0);
        }
        
        Vector2 testPosition2 = testPosition + MyCameraController.offset + ScoutObject.transform.position;
        int xMin = 100;
        int xMax = 0;
        int yMin = 100;
        int yMax = 0;
        for(int n = 0; n < BoardManager.RoomList.Count; n++)
        {
            xMin = Mathf.Min(xMin, BoardManager.RoomList[n].innerLeftBottom.x);
            xMax = Mathf.Max(xMax, BoardManager.RoomList[n].innerRightBottom.x);
            yMin = Mathf.Min(yMin, BoardManager.RoomList[n].innerLeftBottom.y);
            yMax = Mathf.Max(yMax, BoardManager.RoomList[n].innerRightTop.y);
        }
        
        if (testPosition2.x > xMin + d.x / 2.01f - 1 && testPosition2.x < xMax + d.x / 1.99f)
        {
            ScoutObject.transform.position += (Vector3) Vector2.Scale(testPosition, Vector2.right);
        }
        if (testPosition2.y > yMin + d.y / 2.01f - 1 && testPosition2.y < yMax + d.y / 1.99f)
        {
            ScoutObject.transform.position += (Vector3)Vector2.Scale(testPosition, Vector2.up);
        }
    }

    //Abort
    public static void CommandAbortWindow()
    {
        //Xキーが押された場合、ウィンドウを閉じる
        if (InputFunction.GetKeyDown("D") == true)
        {
            CommandWindow.SetActive(true);
            StatusWindow.SetActive(true);
            AbortWindow.SetActive(false);
        }
        else if (InputFunction.GetKeyDown("C") == true)
        {
            AbortWindow.SetActive(false);
        }
    }








}
