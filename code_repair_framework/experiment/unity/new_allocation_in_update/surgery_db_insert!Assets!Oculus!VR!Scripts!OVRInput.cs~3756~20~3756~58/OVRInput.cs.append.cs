
		public override bool WasRecentered()
		{
			return (currentState.RRecenterCount != previousState.RRecenterCount);
		}

		public override byte GetRecenterCount()
		{
			return currentState.RRecenterCount;
		}

		public override byte GetBatteryPercentRemaining()
		{
			return currentState.RBatteryPercentRemaining;
		}
	}
}
