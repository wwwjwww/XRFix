
        public void SetGrabFlag(bool grabFlag)
        {
            _grabFlag = grabFlag;
        }

        protected virtual void OnDestroy()
        {
            UniqueIdentifier.Release(_id);
        }

    }
}
