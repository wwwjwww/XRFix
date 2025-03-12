


















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

        private const int k_ARCoreTextureHeight = 1080;
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







        public void Update()
        {
            m_Command = CommandType.None;

            if (m_Command == CommandType.ProcessNextFrame)
            {
                m_TextureReaderApi.ProcessNextFrame();

                TextureReaderApi.ImageFormatType format = m_TextureReaderApi.GetImageFormat();
                int width = m_TextureReaderApi.GetImageWidth();
                int height = m_TextureReaderApi.GetImageHeight();
                IntPtr pixelBuffer = m_TextureReaderApi.GetImageBuffer();
                int bufferSize = m_TextureReaderApi.GetImageBufferSize();

                OnImageAvailableCallback(format, width, height, pixelBuffer, bufferSize);

                m_Command = CommandType.ReleasePreviousBuffer;
            }

            else if (m_Command == CommandType.Create)
            {
                m_TextureReaderApi.Create(ImageWidth, ImageHeight, ImageFormat, ImageSampleMode);
                m_Command = CommandType.None;
            }

            else if (m_Command == CommandType.Reset)
            {
                m_TextureReaderApi.Reset();
                m_Command = CommandType.None;
            }

            else if (m_Command == CommandType.ReleasePreviousBuffer)
            {
                m_TextureReaderApi.ReleasePreviousBuffer();
                m_Command = CommandType.None;
            }
        }



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