using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ObToggle : Toggle
{
    public Sprite toggleOff;
    public Sprite toggleOn;
	public Image checkmark;
	public GameObject hightLight;

    private bool _isOn = false;

    public void OnOff(bool isOn)
    {
        _isOn = isOn;
        if (_isOn)
        {
            checkmark.sprite = toggleOn;
        }
        else
        {
            checkmark.sprite = toggleOff;
        }
    }

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		hightLight.SetActive(true);
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		hightLight.SetActive(false);
	}
}
