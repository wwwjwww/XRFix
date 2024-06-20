
	public bool overlayEnabled
	{
		get
		{
			return _overlay && _overlay.enabled;
		}
		set
		{
			if (_overlay)
			{
				_overlay.enabled = value;
				_defaultMat.color = value ? Color.black : Color.white;
			}
		}
	}
}
