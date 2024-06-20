
  bool multiselected = false;
  public void OnMultiselect(bool on) {
    multiselected = on;
    highlight.SetActive(on);
    if (on) highlightMat.SetFloat("_EmissionGain", .9f);
  }
}

