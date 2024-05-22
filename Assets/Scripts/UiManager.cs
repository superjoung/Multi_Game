using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
}
