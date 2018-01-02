using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

//This class spawns characters when you click the mouse. It also hands disconnects/reconnects.
public class RodSelectionController : MonoBehaviour {

	public Transform CursorPrefab;
    private List<Rod> _rods;

    void Start()
    {
        Cursor.visible = false;
        int numMice = ManyMouseWrapper.MouseCount;
        for (int i = 0; i < numMice; i++)
        {
            var cursor = SpawnRodSelectionCursorForMouseId(i);
            cursor.CursorPositionChanged += () => OnCursorPositionChanged(cursor);
            cursor.Click += () => OnClick(cursor);
        }
        
        _rods = GetComponentsInChildren<MouseControlledRod>()
            .Select(r => new Rod(r))
            .ToList();
    }

    private void OnClick(MousePlayerCursor playerCursor)
    {
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
        var ownedRods = _rods.Where(r => r.Owner == playerCursor);
        var rod = ClosestRod(playerCursor);
        foreach (var ownedRod in ownedRods.Where(r => r != rod))
           ownedRod.OwnerLeft();
        rod.AttemptSetOwner(playerCursor);
    }

    private Rod ClosestRod(MousePlayerCursor playerCursor)
    {
        return _rods.OrderBy(r => r.DistanceTo(playerCursor.CursorPosition)).First();
    }


    private class Rod
    {
        readonly MouseControlledRod _rod;
        private MousePlayerCursor _owner;
        private Color _oldRodColor;

        public Rod(MouseControlledRod rod)
        {
            _rod = rod;
        }

        public MousePlayerCursor Owner
        {
            get { return _owner; }
            private set
            {
                if (_owner == value) return;
                OnOwnerChanging();
                _owner = value;
                OnOwnerChanged();
            }
        }

        private void OnOwnerChanging()
        {
            _rod.Rod.GetComponent<Renderer>().material.SetColor("_SpecColor", _oldRodColor);
        }

        private void OnOwnerChanged()
        {
            if (Owner == null) return;
            _oldRodColor = _rod.Rod.GetComponent<Renderer>().material.GetColor("_SpecColor");
            _rod.Rod.GetComponent<Renderer>().material.SetColor("_SpecColor", Owner.Color);
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
        }

        public void AttemptSetOwner([NotNull] MousePlayerCursor playerCursor)
        {
            if (playerCursor == null) throw new ArgumentNullException("playerCursor");
            if (Owner == null)
                Owner = playerCursor;
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
