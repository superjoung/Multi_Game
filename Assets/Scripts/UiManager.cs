using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UiManager : MonoBehaviour
{
    // Scene 1
    public GameObject hidePanel;
    public GameObject titleText;
    public GameObject startButton;
    public float fadeTime;
    public float moveTime;

    // Scene 2
    public GameObject infoPanel;
    public GameObject gameSelectPanel;
    public List<GameObject> printObjects = new List<GameObject>();
    public GameObject printView;

    public void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            StartCoroutine(Initialized());
        }
    }

    IEnumerator Initialized()
    {
        hidePanel.GetComponent<Image>().DOFade(0, fadeTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(fadeTime);
        RectTransform currentObj = titleText.GetComponent<RectTransform>();
        currentObj.DOAnchorPosY(-1 * currentObj.anchoredPosition.y, moveTime).SetEase(Ease.OutBounce);
        currentObj = startButton.GetComponent<RectTransform>();
        currentObj.DOAnchorPosY(-1 * currentObj.anchoredPosition.y, moveTime).SetEase(Ease.OutBounce);
    }

    public void StartButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void GameModeButtonClick()
    {
        if (!gameSelectPanel.activeSelf)
        {
            gameSelectPanel.SetActive(true);
        }
        else
        {
            gameSelectPanel.SetActive(false);
        }
    }

    public void GameSelect(int index)
    {
        // 0 ¡÷ªÁ¿ß, 1 ∑Í∑ø, 2 777
        PlayerPrefs.SetInt("GameMode", index);
        SceneManager.LoadScene(2);
    }

    public void InfomationButtonClick()
    {
        if (!infoPanel.activeSelf)
        {
            infoPanel.SetActive(true);
        }
        else
        {
            infoPanel.SetActive(false);
        }
    }

    public void HooneJaButtonClick()
    {

    }

    public void HeadInfoButtonClick(int index)
    {
        foreach(GameObject child in printObjects) child.SetActive(false);
        printObjects[index].SetActive(true);
        printView.GetComponent<ScrollRect>().content = printObjects[index].GetComponent<RectTransform>();
    }
}
