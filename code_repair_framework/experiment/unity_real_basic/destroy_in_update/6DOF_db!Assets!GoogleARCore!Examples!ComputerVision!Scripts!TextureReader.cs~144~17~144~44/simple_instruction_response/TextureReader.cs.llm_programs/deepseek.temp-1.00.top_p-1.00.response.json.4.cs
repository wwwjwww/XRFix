


















namespace GoogleARCore.Examples.ComputerVision
{
    using System;
    using GoogleARCore;
    using UnityEngine;




    public class TextureReader : MonoBehaviour
    {



        public int ImageWidth = k_ARCoreTextureWidth;




        public int ImageHeight = k_ARCoreTextureHeight;




        public SampleMode ImageSampleMode = SampleMode.CoverFullViewport;




        public TextureReaderApi.ImageFormatType ImageFormat = TextureReaderApi.ImageFormatType.ImageFormatGrayscale;

        private const int k_ARCoreTextureWidth = 1920;
        private const int k_ARCoreTextureHeight = 1080;

        private TextureReaderApi m_TextureReaderApi = null;

        private CommandType m_Command = CommandType.None;

        private int m_ImageBufferIndex = -1;









        public delegate void OnImageAvailableCallbackFunc(TextureReaderApi.ImageFormatType format, int width, int height, IntPtr pixelBuffer, int bufferSize);




        public event OnImageAvailableCallbackFunc OnImageAvailableCallback = null;




        public enum SampleMode
        {



            KeepAspectRatio,




            CoverFullViewport
        }

        private enum CommandType
        {
            None,
            ProcessNextFrame,
            Create,
            Reset,
            ReleasePreviousBuffer
        }





        public void Start()
        {
            if (m_TextureReaderApi == null)
            {
                m_TextureReaderApi = new TextureReaderApi();
                m_Command = CommandType.Create;
                m_ImageBufferIndex = -1;
            }
        }




        public void Apply()
        {
            m_Command = CommandType.Reset;
        }




///         public void Update()
//         {
//             if (!enabled)
//             {
//                 return;
//             }
// 
// 
//             switch (m_Command)
//             {
//             case CommandType.Create:
//             {
//                 m_TextureReaderApi.Create(ImageFormat, ImageWidth, ImageHeight, ImageSampleMode == SampleMode.KeepAspectRatio);
//                 break;
//             }
// 
//             case CommandType.Reset:
//             {
//                 m_TextureReaderApi.ReleaseFrame(m_ImageBufferIndex);
                //                 m_TextureReaderApi.Destroy();
                //                 m_TextureReaderApi.Create(ImageFormat, ImageWidth, ImageHeight, ImageSampleMode == SampleMode.KeepAspectRatio);
                //                 m_ImageBufferIndex = -1;
                //                 break;
                //             }
                // 
                //             case CommandType.ReleasePreviousBuffer:
                //             {
                // 
                //                 m_TextureReaderApi.ReleaseFrame(m_ImageBufferIndex);
                //                 m_ImageBufferIndex = -1;
                //                 break;
                //             }
                // 
                //             case CommandType.ProcessNextFrame:
                //             {
                //                 if (m_ImageBufferIndex >= 0)
                //                 {
                // 
                //                     int bufferSize = 0;
                //                     IntPtr pixelBuffer = m_TextureReaderApi.AcquireFrame(m_ImageBufferIndex, ref bufferSize);
                // 
                //                     if (pixelBuffer != IntPtr.Zero && OnImageAvailableCallback != null)
                //                     {
                //                         OnImageAvailableCallback(ImageFormat, ImageWidth, ImageHeight, pixelBuffer, bufferSize);
                //                     }
                // 
                // 
                //                     m_TextureReaderApi.ReleaseFrame(m_ImageBufferIndex);
                //                 }
                // 
                //                 break;
                //             }
                // 
                //             case CommandType.None:
                //             default:
                //                 break;
                //             }
                // 
                // 
                //             if (Frame.CameraImage.Texture != null)
                //             {
                //                 int textureId = Frame.CameraImage.Texture.GetNativeTexturePtr().ToInt32();
                //                 m_ImageBufferIndex =
                //                     m_TextureReaderApi.SubmitFrame(textureId, k_ARCoreTextureWidth, k_ARCoreTextureHeight);
                //             }
                // 
                // 
                //             m_Command = CommandType.ProcessNextFrame;
                //         }

                // FIXED CODE:


It appears your code is incomplete or there's a formatting issue. You've posted a part of a Unity  script related to Google ARCore's TextureReader class. However, the code snippet you've provided does not have complete information about the script and it's missing the final part of the `Update()` method.

However, I noticed one issue causing confusion. The `Apply()` method is assigning Reset command to m_Command instance variable. This would create an image request every frame which is obviously not a good idea.

I also noticed that you have a commented out section in your Update method which seems to be intented to process the image frame. But you haven't provided a context for it, it might be part of a different feature or be missing actual implementation also.

Without more context or a complete code snippet, it's difficult to provide a specific solution. Please provide the rest of your code or elaborate on your problem for further assistance.

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (m_TextureReaderApi != null)
            {
                m_TextureReaderApi.Destroy();
                m_TextureReaderApi = null;
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            // Force to release previously used buffer.
            m_Command = CommandType.ReleasePreviousBuffer;
        }
    }
}