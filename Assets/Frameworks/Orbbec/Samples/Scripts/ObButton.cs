using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObButton : Button
{
    public Sprite buttonOff;
    public Sprite buttonOn;
    public GameObject board;

    private bool _isOn = false;

    public void OnOff(bool isOn)
    {
        _isOn = isOn;
        if (_isOn)
        {
            image.sprite = buttonOn;
            //GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
            var text = GetComponentInChildren<Text>();
            if(text != null)
            {
                text.color = new Color32(15, 113, 240, 255);
            }
        }
        else
        {
            image.sprite = buttonOff;
            //GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
            var text = GetComponentInChildren<Text>();
            if(text != null)
            {
                text.color = Color.white;
            }
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
		transform.localScale = new Vector2(1.2f, 1.2f);
        board.SetActive(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
		transform.localScale = new Vector2(1f, 1f);
        board.SetActive(false);
    }
}
