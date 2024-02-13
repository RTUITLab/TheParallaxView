using UnityEngine;
using System.Collections;

public class Bindable<T>
{
    public delegate void ValueChangedHandler(T value);
    public event ValueChangedHandler onValueChanged;

    private T _value;

    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (!object.Equals(value, _value))
            {
				_value = value;
                onValueChanged.Invoke(_value);
            }
        }
    }
}
