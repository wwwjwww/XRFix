













using UnityEngine;
using System.Collections;

public class spectrumDisplay : MonoBehaviour {
  public AudioSource source;
  int texW = 256;
  int texH = 32;
  Texture2D tex;
  public Renderer texrend;
  Color32[] texpixels;

  bool active = false;

  float[] spectrum;

  protected Rigidbody rb10;


  void Start() {
    spectrum = new float[texW];

    tex = new Texture2D(texW, texH, TextureFormat.RGBA32, false);
    texpixels = new Color32[texW * texH];

    for (int i = 0; i < texpixels.Length; i++) texpixels[i] = new Color32(0, 0, 0, 255);
    tex.SetPixels32(texpixels);
    tex.Apply(false);

    texrend.material.mainTexture = tex;
    texrend.material.SetTexture(Shader.PropertyToID("_Illum"), tex);
    texrend.material.SetColor("_EmissionColor", Color.HSVToRGB(10 / 400f, 98 / 255f, 1f));
    texrend.material.SetFloat("_EmissionGain", .4f);
  }

  const float spectrumMult = 5;
  void GenerateTex() {
    for (int i = 0; i < texW; i++) {
      for (int i2 = 0; i2 < texH; i2++) {
        byte s = 0;
        if (spectrum[i] * spectrumMult * texH >= i2) s = 255;
        texpixels[i2 * texW + i] = new Color32(s, s, s, 255);
      }
    }
  }

  public void toggleActive(bool on) {
    active = on;
    if (!active) {
      for (int i = 0; i < texpixels.Length; i++) texpixels[i] = new Color32(0, 0, 0, 255);
      tex.SetPixels32(texpixels);
      tex.Apply(false);
    }
  }

//   void Update() {
    // BUG: Transform object of Rigidbody in Update() methods
    // MESSAGE: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //     rb10.transform.Rotate(0, 30, 0);
    // 
    //     if (!active) return;
    // 
    //     source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
    //     GenerateTex();
    //     tex.SetPixels32(texpixels);
    //     tex.Apply(false);
    //   }

    // FIXED CODE:

    }
using UnityEngine;
using UnityEngine.Audio;

public class spectrumDisplay : MonoBehaviour
{
    // Declare properties
    public AudioSource source;
    public Renderer textureRenderer;
    public int textureWidth = 256;
    public int textureHeight = 32;
    public int fftSize = 2048;
    public float spectrumMultiplier = 5f;
    public ColorSpectrum colorSpectrum;

    // Create a new texture
    Texture2D texture;
    Color32[] texturePixels;

    // Track whether the display is active
    bool displayActive = false;

    // Cache the transform of the rigidbody
    Rigidbody rb;

    void Start()
    {
        // Create a new texture
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
        texturePixels = new Color32[textureWidth * textureHeight];

        for (int i = 0; i < texturePixels.Length; i++)
        {
            texturePixels[i] = new Color32(0, 0, 0, 255);
        }

        // Set the pixels on the texture
        texture.SetPixels32(texturePixels);

        // Apply the changes to the texture
        texture.Apply(false);

        // Set the texture on the material
        textureRenderer.material.mainTexture = texture;

        // Cache the rigidbody transform
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // If the display is not active, do nothing
        if (!displayActive)
        {
            return;
        }

        // Get the audio spectrum data from the source
        float[] spectrum = new float[fftSize];
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Loop through the pixels in the texture and set their color based on the spectrum data
        for (int i = 0; i < textureWidth; i++)
        {
            for (int j = 0; j < textureHeight; j++)
            {
                float value = spectrum[i] * spectrumMultiplier * textureHeight;
                texturePixels[j * textureWidth + i] = colorSpectrum.GetColor(value);
            }
        }

        // Set the pixels on the texture
        texture.SetPixels32(texturePixels);

        // Apply the changes to the texture
        texture.Apply(false);
    }

    public void ToggleDisplay()
    {
        displayActive =!displayActive;
    }

    //BUG: transform object of rigidbody in update() methods
    //Message: Rigidbody needs to be transformed in FixedUpdate() methods to simulate real-world movement.
    //rb.transform.Rotate(0, 30, 0);

    //If not active then return
    if (!displayActive)
    {
        return;
    }

    // Get the audio spectrum data from the source
    float[] spectrum = new float[fftSize];
    source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

    // Loop through the pixels in the texture and set their color based
}
