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
    public int firstIndex = 0;
    public int shortIndex = 0;
    private float shortLenth = 100f;

    private void Start()
    {
        Initialized();
    }

    public void Initialized()
    {
        firstIndex = 0;
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
                    firstIndex = count;
                }
            }
        }

        if(currentState != rollState)
        {
            currentState = rollState;

            int selectCount = 0;
            foreach(GameObject child in slots)
            {
                if (shortLenth > Mathf.Abs(child.transform.position.y - getPoint.transform.position.y))
                {
                    shortIndex = selectCount;
                    shortLenth = Mathf.Abs(child.transform.position.y - getPoint.transform.position.y);
                    selectData = child.GetComponent<BachingkoData>();
                }

                selectCount++;
            }

            slots[shortIndex].transform.position = new Vector3(transform.position.x ,getPoint.transform.position.y, 0);

            for (int count = 0; count < slots.Count; ++count)
            {
                int tmpNum = (count + shortIndex) % slots.Count;
                if(slots[shortIndex].transform.position.y + (-1.05f * count) < endPoint.transform.position.y)
                {
                    slots[tmpNum].transform.position = new Vector3(transform.position.x, slots[shortIndex].transform.position.y + (-1.05f * count) - endPoint.transform.position.y + startPoint.transform.position.y, 0);
                }
                else
                {
                    slots[tmpNum].transform.position = new Vector3(transform.position.x, slots[shortIndex].transform.position.y + (-1.05f * count), 0);
                }
            }
        }
    }
}
