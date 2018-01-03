using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

//This class spawns characters when you click the mouse. It also hands disconnects/reconnects.
public class RodSelectionController : MonoBehaviour {

	public Transform CursorPrefab;
    List<Rod> _rods;
    readonly List<MousePlayerCursor> _renderers = new List<MousePlayerCursor>();

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        int numMice = ManyMouseWrapper.MouseCount;
        for (int i = 0; i < numMice; i++)
        {
            var cursor = SpawnRodSelectionCursorForMouseId(i);
            cursor.CursorPositionChanged += () => OnCursorPositionChanged(cursor);
            cursor.Click += () => OnClick(cursor);
            _renderers.Add(cursor);
        }
        
        _rods = GetComponentsInChildren<MouseControlledRod>()
            .Select(r => new Rod(r))
            .ToList();

        IsActive = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Toggle();
    }

    private void OnClick(MousePlayerCursor playerCursor)
    {
        if (!IsActive) return;
        ToggleControlOfOwnedRod(playerCursor);
    }

    private void ToggleControlOfOwnedRod(MousePlayerCursor playerCursor)
    {
        var rod = ClosestRod(playerCursor);
        if (rod == null || (rod.Owner != null && rod.Owner != playerCursor)) return;
        rod.ToggleControl();
    }

    private void OnCursorPositionChanged(MousePlayerCursor playerCursor)
    {
        if (!IsActive) return;
        var ownedRods = _rods.Where(r => r.Owner == playerCursor);
        var rod = ClosestRod(playerCursor);
        foreach (var ownedRod in ownedRods.Where(r => r != rod))
           ownedRod.OwnerLeft();
        rod.AttemptSetOwner(playerCursor);
    }

    private bool IsActive { get; set; }

    Rod ClosestRod(MousePlayerCursor playerCursor)
    {
        return _rods.OrderBy(r => r.DistanceTo(playerCursor.CursorPosition)).First();
    }

    void Toggle()
    {
        _renderers.ForEach(r => r.gameObject.SetActive(!r.gameObject.activeSelf));
        _rods.ForEach(r => r.ToggleHighlight());
        IsActive = !IsActive;
    }

    private class Rod
    {
        readonly MouseControlledRod _rod;
        private MousePlayerCursor _owner;
        Color? _oldRodColor;
        private bool _highlight;

        public Rod(MouseControlledRod rod)
        {
            _rod = rod;
            Highlight = true;
        }

        private bool Highlight
        {
            get { return _highlight; }
            set
            {
                if (_highlight == value) return;
                _highlight = value;
                OnHighlightChanged();
            }
        }

        private void OnHighlightChanged()
        {
            if (Highlight)
                HighlightWithOwnerColor(selectedRod:false);
            else 
                RestoreRodColor();
        }

        public MousePlayerCursor Owner
        {
            get { return _owner; }
            private set
            {
                if (_owner == value)
                    return;
                OnOwnerChanging();
                _owner = value;
                OnOwnerChanged();
            }
        }

        private void OnOwnerReturned()
        {
            HighlightWithOwnerColor(selectedRod: true);
        }

        public Color OldRodColor
        {
            get
            {
                if (!_oldRodColor.HasValue)
                {
                    _oldRodColor = _rod.Rod.GetComponent<Renderer>().material.GetColor("_SpecColor");
                }
                return _oldRodColor.Value;
            }
        }

        private void OnOwnerChanging()
        {
            RestoreRodColor();
        }

        private void RestoreRodColor()
        {
            _rod.Rod.GetComponent<Renderer>().material.SetColor("_SpecColor", OldRodColor);
        }

        private void OnOwnerChanged()
        {
            if (Highlight)
                HighlightWithOwnerColor(selectedRod:true);
        }

        private void HighlightWithOwnerColor(bool selectedRod)
        {
            if (Owner == null) return;
            var ownerColor = Owner.Color;
            float h, s, v;
            Color.RGBToHSV(ownerColor, out h, out s, out v);
            _rod.Rod.GetComponent<Renderer>().material.SetColor("_SpecColor", Color.HSVToRGB(h, selectedRod? s/2 : s, v));
        }

        public float DistanceTo(Vector2 point)
        {
            return Mathf.Abs(point.x - _rod.gameObject.transform.position.z);
        }

        public void ToggleControl()
        {
            if (Owner == null) return;
            _rod.Mouse = _rod.Mouse == null ? Owner.Mouse : null;
        }
        

        public void OwnerLeft()
        {
            if (_rod.Mouse == null)
                Owner = null;
            else
                HighlightWithOwnerColor(selectedRod: false);
        }

        public void AttemptSetOwner([NotNull] MousePlayerCursor playerCursor)
        {
            if (playerCursor == null) throw new ArgumentNullException("playerCursor");
            if (Owner == null)
                Owner = playerCursor;
            else if (Owner == playerCursor)
                OnOwnerReturned();
        }

        public void ToggleHighlight()
        {
            Highlight = !Highlight;
        }
    }


    private MousePlayerCursor SpawnRodSelectionCursorForMouseId(int mouseId)
	{
	    GameObject newPlayer = Instantiate(CursorPrefab.gameObject);
	    MousePlayerCursor newPlayerCursor = newPlayer.GetComponent<MousePlayerCursor>();
        newPlayerCursor.Init(mouseId, Color.HSVToRGB(H: 1f / MaxMice * mouseId, S: 0.7f, V: 1f));
	    return newPlayerCursor;

	}

    public const int MaxMice = 8;

    //todo remove rod selection playerCursor for disconnected player. also reset game?
    private void EventMouseDisconnected(ManyMouse mouse)
    {
        //keep details of disconnected mouse... then keep looking for it with an init?
        Debug.Log("Mouse Disconnected: " + mouse.DeviceName);
    }

}
