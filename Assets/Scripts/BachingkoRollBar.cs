using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BachingkoRollBar : MonoBehaviour
{
    public GameObject startPoint;
    public GameObject endPoint;
    public GameObject getPoint;
    public List<GameObject> slots;

    public enum ERollState {Stop, Roll};
    public ERollState rollState = ERollState.Stop;
    public ERollState currentState = ERollState.Stop;

    public float rollSpeed;
    public BachingkoData selectData;
    private float shortLenth = 100f;

    private void Start()
    {
        shortLenth = 100f;
    }

    private void Update()
    {
        if (rollState == ERollState.Roll)
        {
            if (currentState != rollState) currentState = rollState;
            for (int count = 0; count < slots.Count; ++count)
            {
                slots[count].transform.Translate(new Vector2(0, -0.1f) * rollSpeed * Time.deltaTime);
                if (slots[count].transform.position.y < endPoint.transform.position.y)
                {
                    slots[count].transform.position = new Vector2(slots[count].transform.position.x, startPoint.transform.position.y);
                }
            }
        }

        if(currentState != rollState)
        {
            currentState = rollState;
            foreach(GameObject child in slots)
            {
                if (shortLenth > Mathf.Abs(child.transform.position.y - getPoint.transform.position.y))
                {
                    shortLenth = Mathf.Abs(child.transform.position.y - getPoint.transform.position.y);
                    selectData = child.GetComponent<BachingkoData>();
                }
            }
        }
    }
}
