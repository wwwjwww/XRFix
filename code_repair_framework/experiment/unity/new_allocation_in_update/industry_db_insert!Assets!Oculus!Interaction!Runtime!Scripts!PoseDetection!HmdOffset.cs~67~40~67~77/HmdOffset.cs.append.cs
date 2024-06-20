
        #region Inject

        public void InjectAllHmdOffset(IHmd hmd)
        {
            InjectHmd(hmd);
        }

        public void InjectHmd(IHmd hmd)
        {
            _hmd = hmd as MonoBehaviour;
            Hmd = hmd;
        }

        public void InjectOptionalOffsetTranslation(Vector3 val)
        {
            _offsetTranslation = val;
        }

        public void InjectOptionalOffsetRotation(Vector3 val)
        {
            _offsetRotation = val;
        }

        public void InjectOptionalDisablePitchFromSource(bool val)
        {
            _disablePitchFromSource = val;
        }

        public void InjectOptionalDisableYawFromSource(bool val)
        {
            _disableYawFromSource = val;
        }

        public void InjectOptionalDisableRollFromSource(bool val)
        {
            _disableRollFromSource = val;
        }

        #endregion
    }
}
