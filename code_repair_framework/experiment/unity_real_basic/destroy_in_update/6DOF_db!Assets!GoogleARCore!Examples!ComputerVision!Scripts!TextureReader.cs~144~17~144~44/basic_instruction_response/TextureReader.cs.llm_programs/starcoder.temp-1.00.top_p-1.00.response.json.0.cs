


















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





        private void Start()
        {
            if (_textureReaderApi == null)
            {
                _textureReaderApi = new TextureReaderApi();
                _command = CommandType.Create;
                _imageBufferIndex = -1;
            }
        }




        public void Apply()
        {
            _command = CommandType.Reset;
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
                // BUG: Destroy in Update() method
                // MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
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


        [SerializeField]
        private int _imageWidth = 1920;

        [SerializeField]
        private int _imageHeight = 1080;

        [SerializeField]
        private SampleMode _imageSampleMode = SampleMode.KeepAspectRatio;

        [SerializeField]
        private TextureReaderApi.ImageFormatType _imageFormat =
            TextureReaderApi.ImageFormatType.ImageFormatGrayscale;

        private TextureReaderApi _textureReaderApi = null;

        private CommandType _command = CommandType.None;

        private int _imageBufferIndex = -1;

        private void Update()
        {
            if (!enabled)
            {
                return;
            }

            switch (_command)
            {
                case CommandType.Create:
                {
                    _textureReaderApi.Create(
                        _imageFormat, _imageWidth, _imageHeight,
                        _imageSampleMode == SampleMode.KeepAspectRatio);
                    break;
                }

                case CommandType.Reset:
                {
                    _textureReaderApi.ReleaseFrame(_imageBufferIndex);
                    // BUG: Destroy in Update() method
                    // MESSAGE: Update() method is called each frame. It's inefficient to use Destroy() in Update() method.
                    _textureReaderApi.Destroy();
                    _textureReaderApi.Create(
                        _imageFormat, _imageWidth, _imageHeight,
                        _imageSampleMode == SampleMode.KeepAspectRatio);
                    _imageBufferIndex = -1;
                    break;
                }

                case CommandType.ReleasePreviousBuffer:
                {
                    _textureReaderApi.ReleaseFrame(_imageBufferIndex);
                    _imageBufferIndex = -1;
                    break;
                }

                case CommandType.ProcessNextFrame:
                {
                    if (_imageBufferIndex >= 0)
                    {
                        int bufferSize = 0;
                        IntPtr pixelBuffer = _textureReaderApi.AcquireFrame(
                            _imageBufferIndex, ref bufferSize);

                        if (pixelBuffer!= IntPtr.Zero && OnImageAvailableCallback!= null)
                        {
                            OnImageAvailableCallback(
                                _imageFormat, _imageWidth, _imageHeight, pixelBuffer, bufferSize);
                        }
                    }

                    break;
                }

                case CommandType.None:
                default:
                    break;
            }

            if (Frame.CameraImage.Texture!= null)
            {
                int textureId = Frame.CameraImage.Texture.GetNativeTexturePtr().ToInt32();
                _imageBufferIndex =
                    _textureReaderApi.SubmitFrame(textureId, k_ARCoreTextureWidth, k_ARCoreTextureHeight);
            }

            _command = CommandType.ProcessNextFrame;
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