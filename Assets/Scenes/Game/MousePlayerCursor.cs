using System;
using UnityEngine;

/*
 todo
 rotate table so that x,z are aligned with mouse x, y. currently mouse x,y corresponds to screen z,x, which is confusing
*/
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
        MinX = MaxX = transform.position.x + mouseId * renderer.bounds.size.x;
        transform.position = new Vector3(MinX, transform.position.y, MinZ + MaxZ / 2);

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
	    Vector2 transformDelta = _mouse.Delta / 4;
	    transform.position = new Vector3(
            ConstrainToInterval(transform.position.x + transformDelta.y, MinX, MaxX), 
            transform.position.y,
            ConstrainToInterval(transform.position.z + transformDelta.x, MinZ, MaxZ));
	    CursorPosition = new Vector2(transform.position.z, transform.position.x);
	}
    
    public float MinX;
    public float MaxX;
    public float MinZ = -50;
    public float MaxZ = 0;

    private float ConstrainToInterval(float number, float min, float max)
    {
        var constrainedToMin = Mathf.Max(number, min);
        var constrainedToMinAndMax = Mathf.Min(constrainedToMin, max);
        return constrainedToMinAndMax;
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
