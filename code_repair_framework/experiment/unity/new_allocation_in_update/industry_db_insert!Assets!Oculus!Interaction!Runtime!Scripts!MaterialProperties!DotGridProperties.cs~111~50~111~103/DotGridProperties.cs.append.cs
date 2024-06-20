
        protected virtual void OnValidate()
        {
            _change = true;
        }

        #region Inject

        public void InjectAllDotGridProperties(MaterialPropertyBlockEditor editor)
        {
            InjectMaterialPropertyBlockEditor(editor);
        }

        public void InjectMaterialPropertyBlockEditor(MaterialPropertyBlockEditor editor)
        {
            _materialPropertyBlockEditor = editor;
        }

        #endregion
    }
}
