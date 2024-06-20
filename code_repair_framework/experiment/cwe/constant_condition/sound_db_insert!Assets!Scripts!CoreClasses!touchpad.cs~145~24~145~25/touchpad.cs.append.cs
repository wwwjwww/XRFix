
    public void setQuestionMark(bool on)
    {
        halfSprites[0].material.SetColor("_TintColor", on ? onColor : offColor);
        halfSprites[0].material.SetFloat("_EmissionGain", on ? .5f : 0);
    }
}
