
            uint glTextureId;
            VideoStream videostream;
            CameraVideoStreamFrameHeader_t header;
        }

        #region Top level accessors.

        public static VideoStreamTexture Distorted(int deviceIndex = (int)OpenVR.k_unTrackedDeviceIndex_Hmd)
        {
            if (distorted == null)
                distorted = new VideoStreamTexture[OpenVR.k_unMaxTrackedDeviceCount];
            if (distorted[deviceIndex] == null)
                distorted[deviceIndex] = new VideoStreamTexture((uint)deviceIndex, false);
            return distorted[deviceIndex];
        }

        public static VideoStreamTexture Undistorted(int deviceIndex = (int)OpenVR.k_unTrackedDeviceIndex_Hmd)
        {
            if (undistorted == null)
                undistorted = new VideoStreamTexture[OpenVR.k_unMaxTrackedDeviceCount];
            if (undistorted[deviceIndex] == null)
                undistorted[deviceIndex] = new VideoStreamTexture((uint)deviceIndex, true);
            return undistorted[deviceIndex];
        }

        public static VideoStreamTexture Source(bool undistorted, int deviceIndex = (int)OpenVR.k_unTrackedDeviceIndex_Hmd)
        {
            return undistorted ? Undistorted(deviceIndex) : Distorted(deviceIndex);
        }

        private static VideoStreamTexture[] distorted, undistorted;

        #endregion

        #region Internal class to manage lifetime of video streams (per device).

        class VideoStream
        {
            public VideoStream(uint deviceIndex)
            {
                this.deviceIndex = deviceIndex;
                var trackedCamera = OpenVR.TrackedCamera;
                if (trackedCamera != null)
                    trackedCamera.HasCamera(deviceIndex, ref _hasCamera);
            }
            public uint deviceIndex { get; private set; }

            ulong _handle;
            public ulong handle { get { return _handle; } }

            bool _hasCamera;
            public bool hasCamera { get { return _hasCamera; } }

            ulong refCount;
            public ulong Acquire()
            {
                if (_handle == 0 && hasCamera)
                {
                    var trackedCamera = OpenVR.TrackedCamera;
                    if (trackedCamera != null)
                        trackedCamera.AcquireVideoStreamingService(deviceIndex, ref _handle);
                }
                return ++refCount;
            }
            public ulong Release()
            {
                if (refCount > 0 && --refCount == 0 && _handle != 0)
                {
                    var trackedCamera = OpenVR.TrackedCamera;
                    if (trackedCamera != null)
                        trackedCamera.ReleaseVideoStreamingService(_handle);
                    _handle = 0;
                }
                return refCount;
            }
        }

        static VideoStream Stream(uint deviceIndex)
        {
            if (videostreams == null)
                videostreams = new VideoStream[OpenVR.k_unMaxTrackedDeviceCount];
            if (videostreams[deviceIndex] == null)
                videostreams[deviceIndex] = new VideoStream(deviceIndex);
            return videostreams[deviceIndex];
        }

        static VideoStream[] videostreams;

        #endregion
    }
}