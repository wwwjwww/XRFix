            manip.toggleTips(on);
        }
        else if (halfSelected[1])
        {
            if (deleteOn)
            {
                manip.DeleteSelection(on);
                halfSprites[2].material.SetColor("_TintColor", on ? onColor : offColor);
                halfSprites[2].material.SetFloat("_EmissionGain", on ? .5f : 0);
            }
            else if (multiselectOn)
            {
                manip.MultiselectSelection(on);
                halfSprites[3].material.SetColor("_TintColor", on ? onColor : offColor);
                halfSprites[3].material.SetFloat("_EmissionGain", on ? .5f : 0);
            }
            else
            {
                manip.SetCopy(on);

                halfSprites[1].material.SetColor("_TintColor", on ? onColor : offColor);
                halfSprites[1].material.SetFloat("_EmissionGain", on ? .5f : 0);
            }
          
        }
    }

    public void setQuestionMark(bool on)
    {
        halfSprites[0].material.SetColor("_TintColor", on ? onColor : offColor);
        halfSprites[0].material.SetFloat("_EmissionGain", on ? .5f : 0);
    }
}
