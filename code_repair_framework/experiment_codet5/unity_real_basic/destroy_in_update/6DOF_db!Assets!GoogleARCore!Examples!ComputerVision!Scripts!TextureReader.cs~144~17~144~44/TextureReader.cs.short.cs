
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

                // you can try to build an object pool before Update() method has been called.
                // FIXED CODE:
