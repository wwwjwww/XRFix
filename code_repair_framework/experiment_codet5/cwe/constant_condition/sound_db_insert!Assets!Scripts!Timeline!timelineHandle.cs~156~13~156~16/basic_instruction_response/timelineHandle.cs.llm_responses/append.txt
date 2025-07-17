      } else highlight.SetActive(false);

    }
    if (curState == manipState.selected) {
      highlight.SetActive(true);
      highlightMat.SetFloat("_EmissionGain", .55f);
    }
    if (curState == manipState.grabbed) {
      _timelineEvent._componentInterface.killMultiselect();
      _timelineEvent.grabbed = true;
      _timelineEvent.toggleEdges(false);
      highlight.SetActive(true);

      if (stretchMode) manipOffset = _timelineEvent._componentInterface.worldPosToGridPos(manipulatorObj.position, true);
      else {
        manipOffset = _timelineEvent._componentInterface.worldPosToGridPos(manipulatorObj.position);
        startPos = _timelineEvent.transform.localPosition;
        startIn = _timelineEvent._componentInterface._gridParams.UnittoX(_timelineEvent.in_out.x);
        unitLength = _timelineEvent.in_out.y - _timelineEvent.in_out.x;
      }
      highlightMat.SetFloat("_EmissionGain", .75f);
    }
  }

  bool multiselected = false;
  public void OnMultiselect(bool on) {
    multiselected = on;
    highlight.SetActive(on);
    if (on) highlightMat.SetFloat("_EmissionGain", .9f);
  }
}

