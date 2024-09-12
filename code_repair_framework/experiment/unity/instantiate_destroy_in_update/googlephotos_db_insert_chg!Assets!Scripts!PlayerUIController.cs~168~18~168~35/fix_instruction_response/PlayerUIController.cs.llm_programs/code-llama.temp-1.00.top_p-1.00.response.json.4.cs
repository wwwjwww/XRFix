namespace PlayerUIController
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerUIController : MonoBehaviour
    {
        // FIXED CODE:
        public Image progressBar;
        private void SetProgressBar(float value)
        {
            progressBar.fillAmount = value;
        }

        private void Update()
        {
            // FIXED CODE: Use a UnityEngine.UI.Slider to track the progress of the player's health.
            healthSlider.value = Player.Instance.CurrentHealth / Player.Instance.MaxHealth;

            // FIXED CODE: Use a UnityEngine.UI.Slider to track the progress of the player's hunger.
            hungerSlider.value = Player.Instance.CurrentHunger / Player.Instance.MaxHunger;

            // FIXED CODE: Use a UnityEngine.UI.Slider to track the progress of the player's thirst.
            thirstSlider.value = Player.Instance.CurrentThirst / Player.Instance.MaxThirst;

            // FIXED CODE: Use a UnityEngine.UI.Slider to track the progress of the player's fatigue.
            fatigueSlider.value = Player.Instance.CurrentFatigue / Player.Instance.MaxFatigue;
        }
    }
}

    public void ReleaseInstance(GameObject instance5)
    {
        Destroy(instance5);
    }

    void OnHidePressed() {
        if(content.activeSelf && isDisplayingPhotos && photosUI.photoDisplayer.CurrentMediaItem != null)
        {
            content.SetActive(false);
        } else
        {
            content.SetActive(true);
        }
    }

    void OnEnable()
    {
        hideUIbutton.Enable();
    }
    void OnDisable()
    {
        hideUIbutton.Disable();
    }
}
