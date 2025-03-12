


















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







Fixing the code...

        public void OnImageAvailable(TextureReaderApi.ImageFormatType format, int width, int height, IntPtr pixelBuffer, int bufferSize)
        {
            if (OnImageAvailableCallback != null)
                OnImageAvailableCallback(format, width, height, pixelBuffer, bufferSize);
        }

        private void ProcessNextFrame()
        {
            int width = ImageWidth;
            int height = ImageHeight;

            switch (ImageSampleMode)
            {
                case SampleMode.KeepAspectRatio:
                    width = Mathf.Min(width, (int)(height * width / height));
                    height = Mathf.Min(height, (int)(width * height / width));
                    break;
                case SampleMode.CoverFullViewport:
                    width = ImageWidth;
                    height = ImageHeight;
                    break;
            }

            m_TextureReaderApi.ProcessNextFrame(width, height, ImageFormat, m_ImageBufferIndex);

            if (m_Command == CommandType.ProcessNextFrame)
            {
                m_Command = CommandType.None;
            }
        }

        private void Create()
        {
            m_TextureReaderApi.Create(ImageWidth, ImageHeight, ImageFormat, 0);
            m_Command = CommandType.None;
        }

        private void Reset()
        {
            m_TextureReaderApi.Reset();
            m_Command = CommandType.None;
        }

        private void ReleasePreviousBuffer()
        {
            m_PreviousCommand = CommandType.None;
        }

        private void LateUpdate()
        {
            switch (m_Command)
            {
                case CommandType.ProcessNextFrame:
                    ProcessNextFrame();
                    break;
                case CommandType.Create:
                    Create();
                    break;
                case CommandType.Reset:
                    Reset();
                    break;
                case CommandType.ReleasePreviousBuffer:
                    ReleasePreviousBuffer();
                    break;
            }
        }

        private void OnApplicationQuit()
        {
            m_TextureReaderApi.Release();
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