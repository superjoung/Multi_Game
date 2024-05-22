using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class RouletteDate
{
    public Sprite icon;
    public string description;

    public int chance = 100;

    [HideInInspector]
    public int index;
    [HideInInspector]
    public int weight;
}

public class GameManager : MonoBehaviour
{
    public enum ETouchState { None, Begin, Move, End };
    public ETouchState playerTouch = ETouchState.None;
    public TMP_Text titleText;
    private Vector2 touchPosition = new Vector2(0, 0);

    // Mode Dice
    public List<Sprite> dices = new List<Sprite>();
    public GameObject dice;
    public GameObject hideEffectPanel;
    public bool canDice = true;

    public float hideTime;
    public float changeTime;
    public float shackTime;
    public enum EDiceState { Begin, End, Continue, None };
    public EDiceState diceState = EDiceState.None;

    private int diceCount = 0;
    private Vector2 firstPos;


    // Mode Rullette
    public GameObject RoulettePanel;
    public TMP_Text GetItemText;
    public GameObject Rulette;
    public Transform piecePrefab;
    public Transform linePrefab;
    public Transform pieceParent;
    public Transform lineParent;
    public enum ERouletteState { Begin, End, Continue, None };
    public ERouletteState RouletteState = ERouletteState.None;
    [SerializeField]
    public RouletteDate[] rouletteDate;

    public int spinDuration;
    public AnimationCurve spinningCurve;

    private float pieceAngle;
    private float halfPieceAngle;
    private float halfPieceAngleWithPaddings;
    private int accumulateWeight;
    private bool isSpinning = false;
    private int selectedIndex = 0;


    // Mode Bachingko
    public GameObject bachingkoPanel;

    private void Start()
    {
        Initialized();
    }

    private void Update()
    {
        TouchSetup();
        UpdateMode();
    }

    private void Initialized()
    {
        switch (PlayerPrefs.GetInt("GameMode"))
        {
            // ÁÖ»çÀ§
            case 0:
                dice.SetActive(true);
                firstPos = dice.transform.position;
                titleText.text = "ÁÖ»çÀ§ °ÔÀÓ";
                break;
            // ·ê·¿
            case 1:
                RoulettePanel.SetActive(true);
                GetItemText.text = "";
                titleText.text = "·ê·¿ °ÔÀÓ";
                pieceAngle = 360 / rouletteDate.Length;
                halfPieceAngle = pieceAngle * 0.5f;
                halfPieceAngleWithPaddings = halfPieceAngle - (halfPieceAngle * 0.25f);
                SpawnPieceLine();
                CalculateWaightIndicate();
                break;
            // ¹ÙÄªÄÚ
            case 2:
                titleText.text = "Çà¿îÀÇ 777";
                break;
            default:
                break;
        }
    }

    private void UpdateMode()
    {
        switch (PlayerPrefs.GetInt("GameMode"))
        {
            // ÁÖ»çÀ§
            case 0:
                if (playerTouch == ETouchState.Begin)
                {
                    if (canDice)
                    {
                        if (diceState == EDiceState.None) diceState = EDiceState.Begin;
                        else if (diceState == EDiceState.Begin) diceState = EDiceState.End;
                    }
                }

                if (playerTouch == ETouchState.End)
                {
                    DiceAction();
                }
                break;
            // ·ê·¿
            case 1:
                if(playerTouch == ETouchState.Begin)
                {
                    if (RouletteState == ERouletteState.None) RouletteState = ERouletteState.Begin;
                    else if (RouletteState == ERouletteState.Begin) RouletteState = ERouletteState.End;
                }

                if(playerTouch == ETouchState.End)
                {
                    if(RouletteState == ERouletteState.Begin)
                    {
                        GetItemText.text = "";
                        RouletteState = ERouletteState.Continue;
                        RuletteSpin(EndOfSpin);
                    }
                }
                break;
            // ¹ÙÄªÄÚ
            case 2:
                break;
            default:
                break;
        }
    }

    void TouchSetup()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.Begin; } }
        else if (Input.GetMouseButton(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.Move; } }
        else if (Input.GetMouseButtonUp(0)) { if (EventSystem.current.IsPointerOverGameObject() == false) { playerTouch = ETouchState.End; } }
        else playerTouch = ETouchState.None;
        touchPosition = Input.mousePosition;
        //Debug.Log(playerTouch);
#else
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId) == true) return;
            if (touch.phase == TouchPhase.Began) playerTouch = ETouchState.Begin;
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) playerTouch = ETouchState.Move;
            else if (touch.phase == TouchPhase.Ended) if (playerTouch != ETouchState.None) playerTouch = ETouchState.End;
            touchPosition = touch.position;
        }
        else playerTouch = ETouchState.None;
#endif
    }

    void DiceAction()
    {
        if (diceState == EDiceState.Begin)
        {
            StartCoroutine(DiceSpriteChange());
        }
        else if (diceState == EDiceState.End)
        {
            diceState = EDiceState.Continue;
            StartCoroutine(DiceEndSpriteChange());
            StartCoroutine(HideEffect());
            canDice = false;
        }
        else
        {
            return;
        }
    }

    void SpawnPieceLine()
    {
        for(int count = 0; count < rouletteDate.Length; ++count)
        {
            Transform piece = Instantiate(piecePrefab, pieceParent.position, Quaternion.identity, pieceParent);
            piece.GetComponent<RulettePiece>().Setup(rouletteDate[count]);
            piece.RotateAround(pieceParent.position, Vector3.back, (pieceAngle * count));

            Transform line = Instantiate(linePrefab, lineParent.position, Quaternion.identity, lineParent);
            line.RotateAround(lineParent.position, Vector3.back, (pieceAngle * count) + halfPieceAngle);
        }
    }

    void CalculateWaightIndicate()
    {
        for(int count = 0; count < rouletteDate.Length; ++count)
        {
            rouletteDate[count].index = count;

            if (rouletteDate[count].chance <= 0)
            {
                rouletteDate[count].chance = 1;
            }

            accumulateWeight += rouletteDate[count].chance;
            rouletteDate[count].weight = accumulateWeight;
        }
    }

    private int GetRandomIndex()
    {
        int weight = Random.Range(0, accumulateWeight);

        for(int count = 0; count < rouletteDate.Length; ++count)
        {
            if (rouletteDate[count].weight > weight)
            {
                return count;
            }
        }

        return 0;
    }

    public void RuletteSpin(UnityAction<RouletteDate> action = null)
    {
        if (isSpinning) return;

        selectedIndex = GetRandomIndex();

        float angle = pieceAngle * selectedIndex;

        float leftOffSet = (angle - halfPieceAngleWithPaddings) % 360;
        float rigthOffSet = (angle + halfPieceAngleWithPaddings) % 360;
        float randomAngle = Random.Range(leftOffSet, rigthOffSet);

        int rotateSpeed = 2;
        float targeAngle = (randomAngle + 360 * spinDuration * rotateSpeed);

        isSpinning = true;
        StartCoroutine(OnSpin(targeAngle, action));
    }

    private void EndOfSpin(RouletteDate selectedData)
    {
        RouletteState = ERouletteState.None;
        GetItemText.text = "<color=yellow> " + selectedData.description + "</color>¸¦ ¾òÀ¸¼Ì½À´Ï´Ù!!";
    }

    IEnumerator OnSpin(float end, UnityAction<RouletteDate> action)
    {
        float current = 0;
        float percent = 0;

        while(percent < 1)
        {
            current += Time.deltaTime;
            percent = current / spinDuration;

            float z = Mathf.Lerp(0, end, spinningCurve.Evaluate(percent));
            Rulette.transform.rotation = Quaternion.Euler(0, 0, z);

            yield return null;
        }

        isSpinning = false;

        if (action != null) action.Invoke(rouletteDate[selectedIndex]);
    }

    IEnumerator DiceSpriteChange()
    {
        dice.GetComponent<SpriteRenderer>().sprite = dices[diceCount];
        yield return new WaitForSeconds(changeTime);
        diceCount++;
        if (diceCount == 6) diceCount = 0;
        if(diceState != EDiceState.None) StartCoroutine(DiceSpriteChange());
    }

    IEnumerator DiceEndSpriteChange()
    {
        dice.transform.DOMove(new Vector2(firstPos.x + Random.Range(-0.3f, 0.3f), firstPos.y + Random.Range(-0.3f, 0.3f)), shackTime).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(shackTime);
        if (diceState != EDiceState.None) StartCoroutine(DiceEndSpriteChange());
    }

    IEnumerator HideEffect()
    {
        hideEffectPanel.GetComponent<Image>().DOFade(1, hideTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(hideTime + 1);
        StartCoroutine(OpenEffect());
    }

    IEnumerator OpenEffect()
    {
        diceState = EDiceState.None;
        StopCoroutine(DiceEndSpriteChange());
        dice.transform.position = firstPos;
        hideEffectPanel.GetComponent<Image>().DOFade(0, hideTime / 2).SetEase(Ease.Linear);
        yield return new WaitForSeconds(hideTime / 2);
        canDice = true;
    }
}
