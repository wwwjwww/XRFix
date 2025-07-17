#define TMP_PRESENT

using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace TMPro
{
    public interface ITextElement
    {
        Material sharedMaterial { get; }

        void Rebuild(CanvasUpdate update);
        int GetInstanceID();
    }

    public enum TextAlignmentOptions
    {
        TopLeft = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Top,
        Top = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Top,
        TopRight = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Top,
        TopJustified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Top,
        TopFlush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Top,
        TopGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Top,

        Left = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Middle,
        Center = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Middle,
        Right = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Middle,
        Justified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Middle,
        Flush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Middle,
        CenterGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Middle,

        BottomLeft = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Bottom,
        Bottom = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Bottom,
        BottomRight = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Bottom,
        BottomJustified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Bottom,
        BottomFlush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Bottom,
        BottomGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Bottom,

        BaselineLeft = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Baseline,
        Baseline = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Baseline,
        BaselineRight = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Baseline,
        BaselineJustified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Baseline,
        BaselineFlush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Baseline,
        BaselineGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Baseline,

        MidlineLeft = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Geometry,
        Midline = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Geometry,
        MidlineRight = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Geometry,
        MidlineJustified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Geometry,
        MidlineFlush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Geometry,
        MidlineGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Geometry,

        CaplineLeft = _HorizontalAlignmentOptions.Left | _VerticalAlignmentOptions.Capline,
        Capline = _HorizontalAlignmentOptions.Center | _VerticalAlignmentOptions.Capline,
        CaplineRight = _HorizontalAlignmentOptions.Right | _VerticalAlignmentOptions.Capline,
        CaplineJustified = _HorizontalAlignmentOptions.Justified | _VerticalAlignmentOptions.Capline,
        CaplineFlush = _HorizontalAlignmentOptions.Flush | _VerticalAlignmentOptions.Capline,
        CaplineGeoAligned = _HorizontalAlignmentOptions.Geometry | _VerticalAlignmentOptions.Capline
    };




    public enum _HorizontalAlignmentOptions
    {
        Left = 0x1, Center = 0x2, Right = 0x4, Justified = 0x8, Flush = 0x10, Geometry = 0x20
    }




    public enum _VerticalAlignmentOptions
    {
        Top = 0x100, Middle = 0x200, Bottom = 0x400, Baseline = 0x800, Geometry = 0x1000, Capline = 0x2000,
    }





    public enum TextRenderFlags
    {
        DontRender = 0x0,
        Render = 0xFF
    };

    public enum TMP_TextElementType { Character, Sprite };
    public enum MaskingTypes { MaskOff = 0, MaskHard = 1, MaskSoft = 2 }; //, MaskTex = 4 };
    public enum TextOverflowModes { Overflow = 0, Ellipsis = 1, Masking = 2, Truncate = 3, ScrollRect = 4, Page = 5, Linked = 6 };
    public enum MaskingOffsetMode { Percentage = 0, Pixel = 1 };
    public enum TextureMappingOptions { Character = 0, Line = 1, Paragraph = 2, MatchAspect = 3 };

    public enum FontStyles { Normal = 0x0, Bold = 0x1, Italic = 0x2, Underline = 0x4, LowerCase = 0x8, UpperCase = 0x10, SmallCaps = 0x20, Strikethrough = 0x40, Superscript = 0x80, Subscript = 0x100, Highlight = 0x200 };
    public enum FontWeight { Thin = 100, ExtraLight = 200, Light = 300, Regular = 400, Medium = 500, SemiBold = 600, Bold = 700, Heavy = 800, Black = 900 };




    public abstract class TMP_Text : MaskableGraphic
    {



        public string text
        {
            get { return m_text; }
            set { if (m_text == value) return; m_text = old_text = value; m_inputSource = TextInputSources.String; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        [TextArea(5, 10)]
        protected string m_text;





        public bool isRightToLeftText
        {
            get { return m_isRightToLeft; }
            set { if (m_isRightToLeft == value) return; m_isRightToLeft = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_isRightToLeft = false;





        public TMP_FontAsset font
        {
            get { return m_fontAsset; }
            set { if (m_fontAsset == value) return; m_fontAsset = value; LoadFontAsset(); m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected TMP_FontAsset m_fontAsset;
        protected TMP_FontAsset m_currentFontAsset;
        protected bool m_isSDFShader;





        public virtual Material fontSharedMaterial
        {
            get { return m_sharedMaterial; }
            set { if (m_sharedMaterial == value) return; SetSharedMaterial(value); m_havePropertiesChanged = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetMaterialDirty(); }
        }
        [SerializeField]
        protected Material m_sharedMaterial;
        protected Material m_currentMaterial;
        protected MaterialReference[] m_materialReferences = new MaterialReference[32];
        protected Dictionary<int, int> m_materialReferenceIndexLookup = new Dictionary<int, int>();

        protected TMP_RichTextTagStack<MaterialReference> m_materialReferenceStack = new TMP_RichTextTagStack<MaterialReference>(new MaterialReference[16]);
        protected int m_currentMaterialIndex;






        public virtual Material[] fontSharedMaterials
        {
            get { return GetSharedMaterials(); }
            set { SetSharedMaterials(value); m_havePropertiesChanged = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetMaterialDirty(); }
        }
        [SerializeField]
        protected Material[] m_fontSharedMaterials;





        public Material fontMaterial
        {

            get { return GetMaterial(m_sharedMaterial); }


            set
            {
                if (m_sharedMaterial != null && m_sharedMaterial.GetInstanceID() == value.GetInstanceID()) return;

                m_sharedMaterial = value;

                m_padding = GetPaddingForMaterial();
                m_havePropertiesChanged = true;
                m_isInputParsingRequired = true;

                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        [SerializeField]
        protected Material m_fontMaterial;





        public virtual Material[] fontMaterials
        {
            get { return GetMaterials(m_fontSharedMaterials); }

            set { SetSharedMaterials(value); m_havePropertiesChanged = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetMaterialDirty(); }
        }
        [SerializeField]
        protected Material[] m_fontMaterials;

        protected bool m_isMaterialDirty;





        public override Color color
        {
            get { return m_fontColor; }
            set { if (m_fontColor == value) return; m_havePropertiesChanged = true; m_fontColor = value; SetVerticesDirty(); }
        }

        [SerializeField]
        protected Color32 m_fontColor32 = Color.white;
        [SerializeField]
        protected Color m_fontColor = Color.white;
        protected static Color32 s_colorWhite = new Color32(255, 255, 255, 255);
        protected Color32 m_underlineColor = s_colorWhite;
        protected Color32 m_strikethroughColor = s_colorWhite;
        protected Color32 m_highlightColor = s_colorWhite;
        protected Vector4 m_highlightPadding = Vector4.zero;
        




        public float alpha
        {
            get { return m_fontColor.a; }
            set { if (m_fontColor.a == value) return; m_fontColor.a = value; m_havePropertiesChanged = true; SetVerticesDirty(); }
        }






        public bool enableVertexGradient
        {
            get { return m_enableVertexGradient; }
            set { if (m_enableVertexGradient == value) return; m_havePropertiesChanged = true; m_enableVertexGradient = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_enableVertexGradient;

        [SerializeField]
        protected ColorMode m_colorMode = ColorMode.FourCornersGradient;
        




        public VertexGradient colorGradient
        {
            get { return m_fontColorGradient; }
            set { m_havePropertiesChanged = true; m_fontColorGradient = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected VertexGradient m_fontColorGradient = new VertexGradient(Color.white);





        public TMP_ColorGradient colorGradientPreset
        {
            get { return m_fontColorGradientPreset; }
            set { m_havePropertiesChanged = true; m_fontColorGradientPreset = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected TMP_ColorGradient m_fontColorGradientPreset;





        public TMP_SpriteAsset spriteAsset
        {
            get { return m_spriteAsset; }
            set { m_spriteAsset = value; m_havePropertiesChanged = true; m_isInputParsingRequired = true; m_isCalculateSizeRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected TMP_SpriteAsset m_spriteAsset;





        public bool tintAllSprites
        {
            get { return m_tintAllSprites; }
            set { if (m_tintAllSprites == value) return; m_tintAllSprites = value; m_havePropertiesChanged = true; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_tintAllSprites;
        protected bool m_tintSprite;
        protected Color32 m_spriteColor;





        public bool overrideColorTags
        {
            get { return m_overrideHtmlColors; }
            set { if (m_overrideHtmlColors == value) return; m_havePropertiesChanged = true; m_overrideHtmlColors = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_overrideHtmlColors = false;





        public Color32 faceColor
        {
            get
            {
                if (m_sharedMaterial == null) return m_faceColor;

                m_faceColor = m_sharedMaterial.GetColor(ShaderUtilities.ID_FaceColor);
                return m_faceColor;
            }

            set { if (m_faceColor.Compare(value)) return; SetFaceColor(value); m_havePropertiesChanged = true; m_faceColor = value; SetVerticesDirty(); SetMaterialDirty(); }
        }
        [SerializeField]
        protected Color32 m_faceColor = Color.white;





        public Color32 outlineColor
        {
            get
            {
                if (m_sharedMaterial == null) return m_outlineColor;

                m_outlineColor = m_sharedMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
                return m_outlineColor;
            }

            set { if (m_outlineColor.Compare(value)) return; SetOutlineColor(value); m_havePropertiesChanged = true; m_outlineColor = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected Color32 m_outlineColor = Color.black;





        public float outlineWidth
        {
            get
            {
                if (m_sharedMaterial == null) return m_outlineWidth;

                m_outlineWidth = m_sharedMaterial.GetFloat(ShaderUtilities.ID_OutlineWidth);
                return m_outlineWidth;
            }
            set { if (m_outlineWidth == value) return; SetOutlineThickness(value); m_havePropertiesChanged = true; m_outlineWidth = value; SetVerticesDirty(); }
        }
        protected float m_outlineWidth = 0.0f;





        public float fontSize
        {
            get { return m_fontSize; }
            set { if (m_fontSize == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_fontSize = value; if (!m_enableAutoSizing) m_fontSizeBase = m_fontSize; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_fontSize = 36; // Font Size
        protected float m_currentFontSize; // Temporary Font Size affected by tags
        [SerializeField]
        protected float m_fontSizeBase = 36;
        protected TMP_RichTextTagStack<float> m_sizeStack = new TMP_RichTextTagStack<float>(16);





        public float fontScale
        {
            get { return m_fontScale; }
        }





        public FontWeight fontWeight
        {
            get { return m_fontWeight; }
            set { if (m_fontWeight == value) return; m_fontWeight = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected FontWeight m_fontWeight = FontWeight.Regular;
        protected FontWeight m_FontWeightInternal = FontWeight.Regular;
        protected TMP_RichTextTagStack<FontWeight> m_FontWeightStack = new TMP_RichTextTagStack<FontWeight>(8);




        public float pixelsPerUnit
        {
            get
            {
                var localCanvas = canvas;
                if (!localCanvas)
                    return 1;

                if (!font)
                    return localCanvas.scaleFactor;

                if (m_currentFontAsset == null || m_currentFontAsset.faceInfo.pointSize <= 0 || m_fontSize <= 0)
                    return 1;
                return m_fontSize / m_currentFontAsset.faceInfo.pointSize;
            }
        }





        public bool enableAutoSizing
        {
            get { return m_enableAutoSizing; }
            set { if (m_enableAutoSizing == value) return; m_enableAutoSizing = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_enableAutoSizing;
        protected float m_maxFontSize; // Used in conjunction with auto-sizing
        protected float m_minFontSize; // Used in conjunction with auto-sizing





        public float fontSizeMin
        {
            get { return m_fontSizeMin; }
            set { if (m_fontSizeMin == value) return; m_fontSizeMin = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_fontSizeMin = 0; // Text Auto Sizing Min Font Size.





        public float fontSizeMax
        {
            get { return m_fontSizeMax; }
            set { if (m_fontSizeMax == value) return; m_fontSizeMax = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_fontSizeMax = 0; // Text Auto Sizing Max Font Size.





        public FontStyles fontStyle
        {
            get { return m_fontStyle; }
            set { if (m_fontStyle == value) return; m_fontStyle = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected FontStyles m_fontStyle = FontStyles.Normal;
        protected FontStyles m_FontStyleInternal = FontStyles.Normal;
        protected TMP_FontStyleStack m_fontStyleStack;




        public bool isUsingBold { get { return m_isUsingBold; } }
        protected bool m_isUsingBold = false; // Used to ensure GetPadding & Ratios take into consideration bold characters.





        public TextAlignmentOptions alignment
        {
            get { return m_textAlignment; }
            set { if (m_textAlignment == value) return; m_havePropertiesChanged = true; m_textAlignment = value; SetVerticesDirty(); }
        }
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("m_lineJustification")]
        protected TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;
        protected TextAlignmentOptions m_lineJustification;
        protected TMP_RichTextTagStack<TextAlignmentOptions> m_lineJustificationStack = new TMP_RichTextTagStack<TextAlignmentOptions>(new TextAlignmentOptions[16]);
        protected Vector3[] m_textContainerLocalCorners = new Vector3[4];
















        public float characterSpacing
        {
            get { return m_characterSpacing; }
            set { if (m_characterSpacing == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true;  m_characterSpacing = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_characterSpacing = 0;
        protected float m_cSpacing = 0;
        protected float m_monoSpacing = 0;




        public float wordSpacing
        {
            get { return m_wordSpacing; }
            set { if (m_wordSpacing == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_wordSpacing = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_wordSpacing = 0;




        public float lineSpacing
        {
            get { return m_lineSpacing; }
            set { if (m_lineSpacing == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_lineSpacing = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_lineSpacing = 0;
        protected float m_lineSpacingDelta = 0; // Used with Text Auto Sizing feature
        protected float m_lineHeight = TMP_Math.FLOAT_UNSET; // Used with the <line-height=xx.x> tag.





        public float lineSpacingAdjustment
        {
            get { return m_lineSpacingMax; }
            set { if (m_lineSpacingMax == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_lineSpacingMax = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_lineSpacingMax = 0; // Text Auto Sizing Max Line spacing reduction.





        public float paragraphSpacing
        {
            get { return m_paragraphSpacing; }
            set { if (m_paragraphSpacing == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_paragraphSpacing = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_paragraphSpacing = 0;





        public float characterWidthAdjustment
        {
            get { return m_charWidthMaxAdj; }
            set { if (m_charWidthMaxAdj == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_charWidthMaxAdj = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_charWidthMaxAdj = 0f; // Text Auto Sizing Max Character Width reduction.
        protected float m_charWidthAdjDelta = 0;





        public bool enableWordWrapping
        {
            get { return m_enableWordWrapping; }
            set { if (m_enableWordWrapping == value) return; m_havePropertiesChanged = true; m_isInputParsingRequired = true; m_isCalculateSizeRequired = true; m_enableWordWrapping = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_enableWordWrapping = false;
        protected bool m_isCharacterWrappingEnabled = false;
        protected bool m_isNonBreakingSpace = false;
        protected bool m_isIgnoringAlignment;




        public float wordWrappingRatios
        {
            get { return m_wordWrappingRatios; }
            set { if (m_wordWrappingRatios == value) return; m_wordWrappingRatios = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected float m_wordWrappingRatios = 0.4f; // Controls word wrapping ratios between word or characters.


















        public TextOverflowModes overflowMode
        {
            get { return m_overflowMode; }
            set { if (m_overflowMode == value) return; m_overflowMode = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected TextOverflowModes m_overflowMode = TextOverflowModes.Overflow;





        public bool isTextOverflowing
        {
            get { if (m_firstOverflowCharacterIndex != -1) return true; return false; }
        }





        public int firstOverflowCharacterIndex
        {
            get { return m_firstOverflowCharacterIndex; }
        }
        [SerializeField]
        protected int m_firstOverflowCharacterIndex = -1;





        public TMP_Text linkedTextComponent
        {
            get { return m_linkedTextComponent; }

            set
            {
                if (m_linkedTextComponent != value)
                {

                    if (m_linkedTextComponent != null)
                    {
                        m_linkedTextComponent.overflowMode = TextOverflowModes.Overflow;
                        m_linkedTextComponent.linkedTextComponent = null;
                        m_linkedTextComponent.isLinkedTextComponent = false;
                    }

                    m_linkedTextComponent = value;

                    if (m_linkedTextComponent != null)
                        m_linkedTextComponent.isLinkedTextComponent = true;
                }

                m_havePropertiesChanged = true;
                m_isCalculateSizeRequired = true;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
        [SerializeField]
        protected TMP_Text m_linkedTextComponent;





        public bool isLinkedTextComponent
        {
            get { return m_isLinkedTextComponent; }

            set
            {
                m_isLinkedTextComponent = value;

                if (m_isLinkedTextComponent == false)
                    m_firstVisibleCharacter = 0;

                m_havePropertiesChanged = true;
                m_isCalculateSizeRequired = true;
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
        [SerializeField]
        protected bool m_isLinkedTextComponent;





        public bool isTextTruncated { get { return m_isTextTruncated; } }
        [SerializeField]
        protected bool m_isTextTruncated;





        public bool enableKerning
        {
            get { return m_enableKerning; }
            set { if (m_enableKerning == value) return; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_enableKerning = value; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_enableKerning;





        public bool extraPadding
        {
            get { return m_enableExtraPadding; }
            set { if (m_enableExtraPadding == value) return; m_havePropertiesChanged = true; m_enableExtraPadding = value; UpdateMeshPadding(); /* m_isCalculateSizeRequired = true;*/ SetVerticesDirty(); /* SetLayoutDirty();*/ }
        }
        [SerializeField]
        protected bool m_enableExtraPadding = false;
        [SerializeField]
        protected bool checkPaddingRequired;





        public bool richText
        {
            get { return m_isRichText; }
            set { if (m_isRichText == value) return; m_isRichText = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_isRichText = true; // Used to enable or disable Rich Text.





        public bool parseCtrlCharacters
        {
            get { return m_parseCtrlCharacters; }
            set { if (m_parseCtrlCharacters == value) return; m_parseCtrlCharacters = value; m_havePropertiesChanged = true; m_isCalculateSizeRequired = true; m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_parseCtrlCharacters = true;





        public bool isOverlay
        {
            get { return m_isOverlay; }
            set { if (m_isOverlay == value) return; m_isOverlay = value; SetShaderDepth(); m_havePropertiesChanged = true; SetVerticesDirty(); }
        }
        protected bool m_isOverlay = false;





        public bool isOrthographic
        {
            get { return m_isOrthographic; }
            set { if (m_isOrthographic == value) return; m_havePropertiesChanged = true; m_isOrthographic = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_isOrthographic = false;





        public bool enableCulling
        {
            get { return m_isCullingEnabled; }
            set { if (m_isCullingEnabled == value) return; m_isCullingEnabled = value; SetCulling(); m_havePropertiesChanged = true; }
        }
        [SerializeField]
        protected bool m_isCullingEnabled = false;




        public bool ignoreRectMaskCulling
        {
            get { return m_ignoreRectMaskCulling; }
            set { if (m_ignoreRectMaskCulling == value) return; m_ignoreRectMaskCulling = value; m_havePropertiesChanged = true; }
        }
        [SerializeField]
        protected bool m_ignoreRectMaskCulling;





        public bool ignoreVisibility
        {
            get { return m_ignoreCulling; }
            set { if (m_ignoreCulling == value) return; m_havePropertiesChanged = true; m_ignoreCulling = value; }
        }
        [SerializeField]
        protected bool m_ignoreCulling = true; // Not implemented yet.





        public TextureMappingOptions horizontalMapping
        {
            get { return m_horizontalMapping; }
            set { if (m_horizontalMapping == value) return; m_havePropertiesChanged = true; m_horizontalMapping = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected TextureMappingOptions m_horizontalMapping = TextureMappingOptions.Character;





        public TextureMappingOptions verticalMapping
        {
            get { return m_verticalMapping; }
            set { if (m_verticalMapping == value) return; m_havePropertiesChanged = true; m_verticalMapping = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected TextureMappingOptions m_verticalMapping = TextureMappingOptions.Character;

















        public float mappingUvLineOffset
        {
            get { return m_uvLineOffset; }
            set { if (m_uvLineOffset == value) return; m_havePropertiesChanged = true; m_uvLineOffset = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected float m_uvLineOffset = 0.0f; // Used for UV line offset per line





        public TextRenderFlags renderMode
        {
            get { return m_renderMode; }
            set { if (m_renderMode == value) return; m_renderMode = value; m_havePropertiesChanged = true; }
        }
        protected TextRenderFlags m_renderMode = TextRenderFlags.Render;





        public VertexSortingOrder geometrySortingOrder
        {
            get { return m_geometrySortingOrder; }

            set { m_geometrySortingOrder = value; m_havePropertiesChanged = true; SetVerticesDirty(); }

        }
        [SerializeField]
        protected VertexSortingOrder m_geometrySortingOrder;





        public bool vertexBufferAutoSizeReduction
        {
            get { return m_VertexBufferAutoSizeReduction; }
            set { m_VertexBufferAutoSizeReduction = value; m_havePropertiesChanged = true; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_VertexBufferAutoSizeReduction = true;




        public int firstVisibleCharacter
        {
            get { return m_firstVisibleCharacter; }
            set { if (m_firstVisibleCharacter == value) return; m_havePropertiesChanged = true; m_firstVisibleCharacter = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected int m_firstVisibleCharacter;




        public int maxVisibleCharacters
        {
            get { return m_maxVisibleCharacters; }
            set { if (m_maxVisibleCharacters == value) return; m_havePropertiesChanged = true; m_maxVisibleCharacters = value; SetVerticesDirty(); }
        }
        protected int m_maxVisibleCharacters = 99999;





        public int maxVisibleWords
        {
            get { return m_maxVisibleWords; }
            set { if (m_maxVisibleWords == value) return; m_havePropertiesChanged = true; m_maxVisibleWords = value; SetVerticesDirty(); }
        }
        protected int m_maxVisibleWords = 99999;





        public int maxVisibleLines
        {
            get { return m_maxVisibleLines; }
            set { if (m_maxVisibleLines == value) return; m_havePropertiesChanged = true; m_isInputParsingRequired = true; m_maxVisibleLines = value; SetVerticesDirty(); }
        }
        protected int m_maxVisibleLines = 99999;





        public bool useMaxVisibleDescender
        {
            get { return m_useMaxVisibleDescender; }
            set { if (m_useMaxVisibleDescender == value) return; m_havePropertiesChanged = true; m_isInputParsingRequired = true; SetVerticesDirty(); }
        }
        [SerializeField]
        protected bool m_useMaxVisibleDescender = true;





        public int pageToDisplay
        {
            get { return m_pageToDisplay; }
            set { if (m_pageToDisplay == value) return; m_havePropertiesChanged = true; m_pageToDisplay = value; SetVerticesDirty(); }
        }
        [SerializeField]
        protected int m_pageToDisplay = 1;
        protected bool m_isNewPage = false;




        public virtual Vector4 margin
        {
            get { return m_margin; }
            set { if (m_margin == value) return; m_margin = value; ComputeMarginSize(); m_havePropertiesChanged = true; SetVerticesDirty(); }
        }
        [SerializeField]
        protected Vector4 m_margin = new Vector4(0, 0, 0, 0);
        protected float m_marginLeft;
        protected float m_marginRight;
        protected float m_marginWidth;  // Width of the RectTransform minus left and right margins.
        protected float m_marginHeight; // Height of the RectTransform minus top and bottom margins.
        protected float m_width = -1;





        public TMP_TextInfo textInfo
        {
            get { return m_textInfo; }
        }
        [SerializeField]
        protected TMP_TextInfo m_textInfo; // Class which holds information about the Text object such as characters, lines, mesh data as well as metrics. 




        public bool havePropertiesChanged
        {
            get { return m_havePropertiesChanged; }
            set { if (m_havePropertiesChanged == value) return; m_havePropertiesChanged = value; m_isInputParsingRequired = true; SetAllDirty(); }
        }

        protected bool m_havePropertiesChanged;  // Used to track when properties of the text object have changed.





        public bool isUsingLegacyAnimationComponent
        {
            get { return m_isUsingLegacyAnimationComponent; }
            set { m_isUsingLegacyAnimationComponent = value; }
        }
        [SerializeField]
        protected bool m_isUsingLegacyAnimationComponent;





        public new Transform transform
        {
            get
            {
                if (m_transform == null)
                    m_transform = GetComponent<Transform>();
                return m_transform;
            }
        }
        protected Transform m_transform;





        public new RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null)
                    m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }
        protected RectTransform m_rectTransform;





        public virtual bool autoSizeTextContainer
        {
            get;
            set;
        }
        protected bool m_autoSizeTextContainer;





        public virtual Mesh mesh
        {
            get { return m_mesh; }
        }
        protected Mesh m_mesh;





        public bool isVolumetricText
        {
            get { return m_isVolumetricText; }
            set { if (m_isVolumetricText == value) return; m_havePropertiesChanged = value; m_textInfo.ResetVertexLayout(value); m_isInputParsingRequired = true; SetVerticesDirty(); SetLayoutDirty(); }
        }
        [SerializeField]
        protected bool m_isVolumetricText;




        public Bounds bounds
        {
            get
            {
                if (m_mesh == null) return new Bounds();

                return GetCompoundBounds();
            }
        }




        public Bounds textBounds
        {
            get
            {
                if (m_textInfo == null) return new Bounds();

                return GetTextBounds();
            }
        }













































        protected TMP_SpriteAnimator spriteAnimator
        {
            get
            {
                if (m_spriteAnimator == null)
                {
                    m_spriteAnimator = GetComponent<TMP_SpriteAnimator>();
                    if (m_spriteAnimator == null) m_spriteAnimator = gameObject.AddComponent<TMP_SpriteAnimator>();
                }

                return m_spriteAnimator;
            }

        }
        [SerializeField]
        protected TMP_SpriteAnimator m_spriteAnimator;






















        public float flexibleHeight { get { return m_flexibleHeight; } }
        protected float m_flexibleHeight = -1f;




        public float flexibleWidth { get { return m_flexibleWidth; } }
        protected float m_flexibleWidth = -1f;




        public float minWidth { get { return m_minWidth; } }
        protected float m_minWidth;




        public float minHeight { get { return m_minHeight; } }
        protected float m_minHeight;




        public float maxWidth { get { return m_maxWidth; } }
        protected float m_maxWidth;




        public float maxHeight { get { return m_maxHeight; } }
        protected float m_maxHeight;




        protected LayoutElement layoutElement
        {
            get
            {
                if (m_LayoutElement == null)
                {
                    m_LayoutElement = GetComponent<LayoutElement>();
                }

                return m_LayoutElement;
            }
        }
        protected LayoutElement m_LayoutElement;




        public virtual float preferredWidth { get { if (!m_isPreferredWidthDirty) return m_preferredWidth; m_preferredWidth = GetPreferredWidth(); return m_preferredWidth; } }
        protected float m_preferredWidth;
        protected float m_renderedWidth;
        protected bool m_isPreferredWidthDirty;




        public virtual float preferredHeight { get { if (!m_isPreferredHeightDirty) return m_preferredHeight; m_preferredHeight = GetPreferredHeight(); return m_preferredHeight; } }
        protected float m_preferredHeight;
        protected float m_renderedHeight;
        protected bool m_isPreferredHeightDirty;

        protected bool m_isCalculatingPreferredValues;
        private int m_recursiveCount;




        public virtual float renderedWidth { get { return GetRenderedWidth(); } }





        public virtual float renderedHeight { get { return GetRenderedHeight(); } }





        public int layoutPriority { get { return m_layoutPriority; } }
        protected int m_layoutPriority = 0;

        protected bool m_isCalculateSizeRequired = false;
        protected bool m_isLayoutDirty;

        protected bool m_verticesAlreadyDirty;
        protected bool m_layoutAlreadyDirty;

        protected bool m_isAwake;
        internal bool m_isWaitingOnResourceLoad;

        internal bool m_isInputParsingRequired = false; // Used to determine if the input text needs to be re-parsed.


        internal enum TextInputSources { Text = 0, SetText = 1, SetCharArray = 2, String = 3 };

        internal TextInputSources m_inputSource;
        protected string old_text; // Used by SetText to determine if the text has changed.



        protected float m_fontScale; // Scaling of the font based on Atlas true Font Size and Rendered Font Size.  
        protected float m_fontScaleMultiplier; // Used for handling of superscript and subscript.

        protected char[] m_htmlTag = new char[128]; // Maximum length of rich text tag. This is preallocated to avoid GC.
        protected RichTextTagAttribute[] m_xmlAttribute = new RichTextTagAttribute[8];

        protected float[] m_attributeParameterValues = new float[16];

        protected float tag_LineIndent = 0;
        protected float tag_Indent = 0;
        protected TMP_RichTextTagStack<float> m_indentStack = new TMP_RichTextTagStack<float>(new float[16]);
        protected bool tag_NoParsing;


        protected bool m_isParsingText;
        protected Matrix4x4 m_FXMatrix;
        protected bool m_isFXMatrixSet;


        protected UnicodeChar[] m_TextParsingBuffer; // This array holds the characters to be processed by GenerateMesh();

        protected struct UnicodeChar
        {
            public int unicode;
            public int stringIndex;
            public int length;
        }


        private TMP_CharacterInfo[] m_internalCharacterInfo; // Used by functions to calculate preferred values.
        protected char[] m_input_CharArray = new char[256]; // This array hold the characters from the SetText();
        private int m_charArray_Length = 0;
        protected int m_totalCharacterCount;


        protected WordWrapState m_SavedWordWrapState = new WordWrapState();
        protected WordWrapState m_SavedLineState = new WordWrapState();




        protected int m_characterCount;


        protected int m_firstCharacterOfLine;
        protected int m_firstVisibleCharacterOfLine;
        protected int m_lastCharacterOfLine;
        protected int m_lastVisibleCharacterOfLine;
        protected int m_lineNumber;
        protected int m_lineVisibleCharacterCount;
        protected int m_pageNumber;
        protected float m_maxAscender;
        protected float m_maxCapHeight;
        protected float m_maxDescender;
        protected float m_maxLineAscender;
        protected float m_maxLineDescender;
        protected float m_startOfLineAscender;

        protected float m_lineOffset;
        protected Extents m_meshExtents;



        protected Color32 m_htmlColor = new Color(255, 255, 255, 128);
        protected TMP_RichTextTagStack<Color32> m_colorStack = new TMP_RichTextTagStack<Color32>(new Color32[16]);
        protected TMP_RichTextTagStack<Color32> m_underlineColorStack = new TMP_RichTextTagStack<Color32>(new Color32[16]);
        protected TMP_RichTextTagStack<Color32> m_strikethroughColorStack = new TMP_RichTextTagStack<Color32>(new Color32[16]);
        protected TMP_RichTextTagStack<Color32> m_highlightColorStack = new TMP_RichTextTagStack<Color32>(new Color32[16]);

        protected TMP_ColorGradient m_colorGradientPreset;
        protected TMP_RichTextTagStack<TMP_ColorGradient> m_colorGradientStack = new TMP_RichTextTagStack<TMP_ColorGradient>(new TMP_ColorGradient[16]);

        protected float m_tabSpacing = 0;
        protected float m_spacing = 0;






        protected TMP_RichTextTagStack<int> m_styleStack = new TMP_RichTextTagStack<int>(new int[16]);
        protected TMP_RichTextTagStack<int> m_actionStack = new TMP_RichTextTagStack<int>(new int[16]);

        protected float m_padding = 0;
        protected float m_baselineOffset; // Used for superscript and subscript.
        protected TMP_RichTextTagStack<float> m_baselineOffsetStack = new TMP_RichTextTagStack<float>(new float[16]);
        protected float m_xAdvance; // Tracks x advancement from character to character.

        protected TMP_TextElementType m_textElementType;
        protected TMP_TextElement m_cached_TextElement; // Glyph / Character information is cached into this variable which is faster than having to fetch from the Dictionary multiple times.
        protected TMP_Character m_cached_Underline_Character; // Same as above but for the underline character which is used for Underline.
        protected TMP_Character m_cached_Ellipsis_Character;

        protected TMP_SpriteAsset m_defaultSpriteAsset;
        protected TMP_SpriteAsset m_currentSpriteAsset;
        protected int m_spriteCount = 0;
        protected int m_spriteIndex;
        protected int m_spriteAnimationID;






        protected virtual void LoadFontAsset() { }





        protected virtual void SetSharedMaterial(Material mat) { }




        protected virtual Material GetMaterial(Material mat) { return null; }





        protected virtual void SetFontBaseMaterial(Material mat) { }





        protected virtual Material[] GetSharedMaterials() { return null; }




        protected virtual void SetSharedMaterials(Material[] materials) { }





        protected virtual Material[] GetMaterials(Material[] mats) { return null; }












        protected virtual Material CreateMaterialInstance(Material source)
        {
            Material mat = new Material(source);
            mat.shaderKeywords = source.shaderKeywords;
            mat.name += " (Instance)";

            return mat;
        }

        protected void SetVertexColorGradient(TMP_ColorGradient gradient)
        {
            if (gradient == null) return;

            m_fontColorGradient.bottomLeft = gradient.bottomLeft;
            m_fontColorGradient.bottomRight = gradient.bottomRight;
            m_fontColorGradient.topLeft = gradient.topLeft;
            m_fontColorGradient.topRight = gradient.topRight;

            SetVerticesDirty();
        }




        protected void SetTextSortingOrder(VertexSortingOrder order)
        {
            
        }





        protected void SetTextSortingOrder(int[] order)
        {

        }





        protected virtual void SetFaceColor(Color32 color) { }





        protected virtual void SetOutlineColor(Color32 color) { }





        protected virtual void SetOutlineThickness(float thickness) { }




        protected virtual void SetShaderDepth() { }




        protected virtual void SetCulling() { }





        protected virtual float GetPaddingForMaterial() { return 0; }






        protected virtual float GetPaddingForMaterial(Material mat) { return 0; }






        protected virtual Vector3[] GetTextContainerLocalCorners() { return null; }



        protected bool m_ignoreActiveState;



        public virtual void ForceMeshUpdate() { }













        public virtual void ForceMeshUpdate(bool ignoreActiveState) { }





        internal void SetTextInternal(string text)
        {
            m_text = text;
            m_renderMode = TextRenderFlags.DontRender;
            m_isInputParsingRequired = true;
            ForceMeshUpdate();
            m_renderMode = TextRenderFlags.Render;
        }













        public virtual void UpdateGeometry(Mesh mesh, int index) { }





        public virtual void UpdateVertexData(TMP_VertexDataUpdateFlags flags) { }





        public virtual void UpdateVertexData() { }






        public virtual void SetVertices(Vector3[] vertices) { }





        public virtual void UpdateMeshPadding() { }















        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
        {
            base.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
            InternalCrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
        }








        public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            base.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
            InternalCrossFadeAlpha(alpha, duration, ignoreTimeScale);
        }










        protected virtual void InternalCrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha) { }








        protected virtual void InternalCrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale) { }





        protected void ParseInputText()
        {



            m_isInputParsingRequired = false;

            switch (m_inputSource)
            {
                case TextInputSources.String:
                case TextInputSources.Text:
                    StringToCharArray(m_text, ref m_TextParsingBuffer);
                    break;
                case TextInputSources.SetText:
                    SetTextArrayToCharArray(m_input_CharArray, ref m_TextParsingBuffer);
                    break;
                case TextInputSources.SetCharArray:
                    break;
            }

            SetArraySizes(m_TextParsingBuffer);

        }






        public void SetText(string text)
        {
            SetText(text, true);
        }






        public void SetText(string text, bool syncTextInputBox)
        {




            m_inputSource = TextInputSources.SetCharArray;

            StringToCharArray(text, ref m_TextParsingBuffer);

            #if UNITY_EDITOR


            if (syncTextInputBox)
                m_text = text;
            #endif

            m_isInputParsingRequired = true;
            m_havePropertiesChanged = true;
            m_isCalculateSizeRequired = true;

            SetVerticesDirty();
            SetLayoutDirty();
        }









        public void SetText(string text, float arg0)
        {
            SetText(text, arg0, 255, 255);
        }









        public void SetText(string text, float arg0, float arg1)
        {
            SetText(text, arg0, arg1, 255);
        }










