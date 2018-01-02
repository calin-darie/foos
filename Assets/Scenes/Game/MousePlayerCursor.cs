using System;
using UnityEngine;

public class MousePlayerCursor : MonoBehaviour
{
    int _mouseId;

    private ManyMouse _mouse;//This is what you'll grab input from.
    private Vector2 _cursorPosition;

    public void Init (int mouseId, Color color)
    {
        var renderer = GetComponent<Renderer>();
        renderer.material.color = color;
        Color = color;

	    _mouseId = mouseId;

        _mouse = ManyMouseWrapper.GetMouseByID( _mouseId );
        Debug.Log(gameObject.name + " connected to mouse: " + _mouse.DeviceName);
        
        _mouse.EventButtonDown += EventButtonDown;
	}

    private void EventButtonDown(ManyMouse mouse, int buttonId)
    {
        if (buttonId == 0)
        {
            OnClick();
        }
    }
    
	void Update ()
	{
	    Vector2 delta = _mouse.Delta * Time.deltaTime * 5;
	    transform.position += new Vector3(delta.y, 0, delta.x);
	    CursorPosition = new Vector2(transform.position.z, transform.position.x);
	}

    public Vector2 CursorPosition
    {
        get { return _cursorPosition; }
        set
        {
            if (_cursorPosition.Equals(value))
                return;
            _cursorPosition = value;
            OnCursorPositionChanged();
        }
    }

    public Color Color { get; private set; }

    public ManyMouse Mouse
    {
        get { return _mouse; }
    }

    public event Action Click;
    public event Action CursorPositionChanged;

    protected virtual void OnCursorPositionChanged()
    {
        var handler = CursorPositionChanged;
        if (handler != null) handler();
    }

    protected virtual void OnClick()
    {
        var handler = Click;
        if (handler != null) handler();
    }
}
