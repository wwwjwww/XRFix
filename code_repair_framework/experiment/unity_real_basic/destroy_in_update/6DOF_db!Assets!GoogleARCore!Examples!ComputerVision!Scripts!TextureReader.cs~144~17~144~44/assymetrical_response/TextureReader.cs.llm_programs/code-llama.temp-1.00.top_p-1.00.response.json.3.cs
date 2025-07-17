


















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





        void Start()
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
            if (m_Command == CommandType.ProcessNextFrame)
            {
                // Retrieve the texture for the current frame.
                var frame = m_TextureReaderApi.GetFrame();

                // Get the image from the frame.
                var image = frame.Image;

                // Apply the image to the texture, using the specified parameters.
                //image.Apply(ImageFormat, ImageWidth, ImageHeight, ImageSampleMode);

                // Check if the image is available.
                if (image.DataAvailable)
                {
                    // Get the image data.
                    //var imageData = image.Data;

                    // Get the image buffer size.
                    //var imageBufferSize = image.BufferSize;

                    // Call the OnImageAvailable callback.
                    //OnImageAvailableCallback?.Invoke(ImageFormat, ImageWidth, ImageHeight, imageBufferSize);
                }
            }

            // Reset the command.
            m_Command = CommandType.None;
        }

        public void ReleasePreviousBuffer()
        {
            if (m_Command == CommandType.ReleasePreviousBuffer)
            {
                // Release the previous buffer.
                m_TextureReaderApi.ReleaseBuffer(m_ImageBufferIndex);
            }

            // Reset the command.
            m_Command = CommandType.None;
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