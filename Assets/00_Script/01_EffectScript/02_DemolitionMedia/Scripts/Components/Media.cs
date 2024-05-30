﻿using System;
using static DemolitionStudios.DemolitionMedia.NativeDll;

#if UNITY_5_3_OR_NEWER
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Serialization;
    using Mathf = UnityEngine.Mathf;
#endif


namespace DemolitionStudios.DemolitionMedia
{
    /// <summary>
    /// Main class for working with a single media source
    /// </summary>
    public partial class Media : IDisposable
    {
        #region fields

        /// Media path
        public string mediaUrl;

        /// Whether to enable audio
#if UNITY_5_3_OR_NEWER
        [SerializeField, FormerlySerializedAs("enableAudio")]
#endif
        public bool openWithAudio = false;

        /// Whether to use native audio plugin
        public bool useNativeAudioPlugin = true;

        /// Whether to open the media on component start
        public bool openOnStart = true;

        /// Whether to play the media when it's opened
        public bool playOnOpen = false;

        /// Whether to preload file to the system memory
        public bool preloadToMemory = false;

        /// Enable framedrop after opening the video (not recommended) 
        /// To change framedrop setting during runtime use FramedropEnabled property
        public bool openWithFramedrop = false;

        /// Decryption key (cenc-aes-ctr scheme using ffmpeg)
        private string decryptionKey = "";

        /// Whether the input file has header magic protection enabled
        private bool enableHeaderMagicProtection = false;


        /// Events
        private MediaEvent _events = new MediaEvent();
        public MediaEvent Events
        {
            get { return _events; }
        }

        /// Unique media id. Generated by the native plugin code
        private int _mediaId = -1;
        public int MediaId
        {
            get { return _mediaId; }
        }

        /// Duration of the media stream in seconds
        public float DurationSeconds
        {
            get; private set;
        }

        /// Current playback position in seconds
        public float CurrentTime
        {
            get { return NativeDll.GetCurrentTime(_mediaId); }
        }

        /// Current state of the media
        public MediaState State
        {
            get { return NativeDll.GetMediaState(_mediaId); }
        }

        /// Whether the media is in valid opened state
        public bool IsOpened
        {
            get { return NativeDll.CanPlay(_mediaId)
                         && !IsOpening
                         && !IsClosing; }
        }

        /// Whether the media is currently opening
        public bool IsOpening { get; private set; } = false;

        /// Whether the media is currently closing
        public bool IsClosing { get; private set; } = false;

        /// Whether the media is currently playing
        public bool IsPlaying
        {
            get { return NativeDll.IsPlaying(_mediaId); }
        }

        /// Whether the media is currently has non-zero number of loops remaining
        public bool IsLooping
        {
            get { return NativeDll.IsLooping(_mediaId); }
        }

        /// Whether the media is finished playing
        public bool IsFinished
        {
            get { return NativeDll.IsFinished(_mediaId); }
        }

        /// Active segment for looping
        private int _startFrame = 0;
        public int StartFrame
        {
            get { return _startFrame; }
            set {
                _startFrame = value;
                _startTime = value * VideoFramerate;
                NativeDll.SetActiveSegmentFrames(_mediaId, _startFrame, _endFrame);
                UpdateVideoDecodeFramerateInterval();
            }
        }
        private int _endFrame = 0;
        public int EndFrame
        {
            get { return _endFrame; }
            set
            {
                _endFrame = value;
                _endTime = value * VideoFramerate;
                NativeDll.SetActiveSegmentFrames(_mediaId, _startFrame, _endFrame);
                UpdateVideoDecodeFramerateInterval();
            }
        }
        private float _startTime = 0.0f;
        public float StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                _startFrame = (int) (value / VideoFramerate);
                NativeDll.SetActiveSegment(_mediaId, _startTime, _endTime);
                UpdateVideoDecodeFramerateInterval();
            }
        }
        private float _endTime = 0.0f;
        public float EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                _endFrame = (int)(value / VideoFramerate);
                NativeDll.SetActiveSegment(_mediaId, _startTime, _endTime);
                UpdateVideoDecodeFramerateInterval();
            }
        }

        private void UpdateVideoDecodeFramerateInterval()
        {
            _videoDecodeFramerateInterval = Math.Min(1.0f, 0.5f * Math.Abs(_endTime - _startTime));
            _videoDecodeFramerateTime = 0.0f;
        }

        /// Framedrop: enabling is not recommended, as it may lead to not smooth playback
        public bool FramedropEnabled
        {
            get { return NativeDll.GetFramedropEnabled(_mediaId); }
            set { NativeDll.SetFramedropEnabled(_mediaId, value); }
        }

        /// Framedrop count
        public void GetFramedropCount(out int earlyDrops, out int lateDrops)
        {
            NativeDll.GetFramedropCount(_mediaId, out earlyDrops, out lateDrops);
        }

        /// Number of loops remaining for this media playback.
        /// Negative value means infinite
        public int Loops
        {
            get { return NativeDll.GetLoops(_mediaId); }
            set { NativeDll.SetLoops(_mediaId, value); }
        }

        /// Number of loops elapsed since playback started
        public int LoopsSinceStart
        {
            get
            {
                return NativeDll.GetNumberOfLoopsSinceStart(_mediaId);
            }
        }

        /// Sync mode
        public VideoTextureType VideoTextureType
        {
            get { return NativeDll.GetVideoTextureType(_mediaId); }
        }

        /// Sync mode
        public SyncMode SyncMode
        {
            get { return NativeDll.GetSyncMode(_mediaId); }
            set { NativeDll.SetSyncMode(_mediaId, value); }
        }

        /// Playback speed
        public float PlaybackSpeed
        {
            get { return NativeDll.GetPlaybackRate(_mediaId); }
            set
            {
                if (HasAudio && AudioEnabled && !Utilities.ApproximatelyEqual(Math.Abs(value), 1.0f))
                {
                    Utilities.LogError("[DemolitionMedia] Only playback speed +-1.0 supported with audio enabled." +
                                   "To set other playback speed, please disable the audio");
                    // Don't change the speed
                    return;
                }

                NativeDll.SetPlaybackRate(_mediaId, value);
            }
        }

        /// Video stream pixel format
        public PixelFormat VideoPixelFormat { get; private set; }
        /// Whether the pixel format requires color conversion
        public bool RequiresColorConversion { get; private set; }
        /// Video stream frame width
        public int VideoWidth { get; private set; }

        /// Video stream frame height
        public int VideoHeight { get; private set; }

        /// Video stream framerate
        public float VideoFramerate { get; private set; }

        /// Video stream frame aspect ratio
        public float VideoAspectRatio
        {
            get { return (float)VideoWidth / VideoHeight; }
        }

        /// Number of frames in the video stream
        public int VideoNumFrames
        {
            get; private set;
        }

        /// Current frame index in the video stream
        public int VideoCurrentFrame
        {
            get { return NativeDll.GetCurrentFrame(_mediaId); }
        }

        /// Whether the first video frame texture has been uploaded
        public bool FirstVideoFrameUploaded
        {
            get { return TexturesCreated() && NativeDll.NativeTexturesFirstFrameUploaded(_mediaId); }
        }

        /// Whether media has audio stream
        public bool HasAudio { get; private set; }

        /// Audio stream sample rate
        public int SourceAudioSampleRate
        {
            get
            {
                int sampleRate, channels;
                NativeDll.GetSourceAudioStreamParameters(_mediaId, out sampleRate, out channels);
                return sampleRate;
            }
        }

        /// Audio stream number of channels
        public int SourceAudioChannels
        {
            get
            {
                int sampleRate, channels;
                NativeDll.GetSourceAudioStreamParameters(_mediaId, out sampleRate, out channels);
                return channels;
            }
        }

        /// Default video frame queue memory limit for video is 300Mb (enough for two 16k Hap Q frames)
        /// Note: shouldn't be too high, since frames go directly to the GPU memory, which is often limited
        private int DefaultVideoFrameQueueMemoryLimitMb = 300;

        /// Default packet queues memory limit for video is 300Mb 
        private int DefaultPacketQueuesMemoryLimitMb = 300;

        /// Video decode framerate
        private int _videoDecodeFramerateFrameCount = 0;
		private float _videoDecodeFramerateInterval = 1.0f;
        private float _videoDecodeFramerateTime = 0.0f;
		private float _videoDecodeFramerate = 1000.0f;
		public float VideoDecodeFramerate
		{
			get { return Math.Min(_videoDecodeFramerate, VideoFramerate); }
		}

        /// Whether we need to restore framedrop enabled after disabling reverse playback
        private bool _restoreFramedropForNonReversePlayback;

        /// Whether we need to flip the video on the X axis for correct displaying
        public bool VideoNeedFlipX { get; private set; }

        /// Whether we need to flip the video on the Y axis for correct displaying
        public bool VideoNeedFlipY { get; private set; }

        /// Cached media state
        private MediaState _stateCached = MediaState.Closed;

        /// Sets the audio parameters
        public static void SetAudioParams(SampleFormat sampleFormat, int sampleRate, int bufferLength, int channels)
        {
            NativeDll.SetAudioParams(sampleFormat, sampleRate, bufferLength, channels);
        }

        // Fills audio buffer with data (SampleFormat.Float or SampleFormat.Float_Planar)
        // Returns the number of audio channels filled
        public int FillAudioBuffer(float[] buffer, int offset, int length, int maxChannels)
        {
            return NativeDll.FillAudioBuffer(_mediaId, buffer, offset, length, maxChannels);
        }
        #endregion

        #region media interface

        /// Open the media (not fully async)
        public bool Open(string url, SyncMode syncMode = SyncMode.SyncAudioMaster, int videoFrameQueueMemoryLimitMb = -1, int packetQueuesMemoryLimitMb = -1,
                         GraphicsInterfaceDeviceType deviceType = GraphicsInterfaceDeviceType.Unknown, IntPtr devicePtr = default)
        {
            // Close the old media if was opened previously
            Close();

            if (_mediaId < 0)
                _mediaId = NativeDll.CreateNewMediaId();
            else
                Utilities.Log("[DemolitionMedia] Using existing media id: " + _mediaId);

            // Prevent from crashing on a null string value
            if (url == null)
                url = "";

            var openingUrl = PreOpenImpl(url);
            int limitFramesMb = videoFrameQueueMemoryLimitMb > 0 ? videoFrameQueueMemoryLimitMb 
                                                                 : DefaultVideoFrameQueueMemoryLimitMb;
            int limitPacketsMb = packetQueuesMemoryLimitMb > 0 ? packetQueuesMemoryLimitMb 
                                                               : DefaultPacketQueuesMemoryLimitMb;

            var generalParams = new MediaGeneralOpenParams();
            generalParams.path = openingUrl;
            generalParams.decryptionKey = decryptionKey;
            generalParams.enableHeaderMagicProtection = enableHeaderMagicProtection;
            generalParams.preloadToMemory = preloadToMemory;
            generalParams.syncMode = syncMode;
            generalParams.videoFrameQueueMemoryLimitMb = limitFramesMb;
            generalParams.packetQueuesMemoryLimitMb = limitPacketsMb;

            var audioParams = new MediaAudioOpenParams();
            audioParams.enableAudio = openWithAudio;
            audioParams.useNativeAudioPlugin = useNativeAudioPlugin;

            bool useSrgbCompressedTextureFormats = false;
            bool fallbackToDynamicTexture = false;
            bool passRawTextureDataOutside = false;
            GetGraphicsParams(out fallbackToDynamicTexture, out passRawTextureDataOutside, out useSrgbCompressedTextureFormats);
#if UNITY_5_3_OR_NEWER
#if UNITY_2022_1_OR_NEWER
            if (!passRawTextureDataOutside)
            {
                Utilities.LogError("Unity 2022 or newer detected, but passRawTextureDataOutside=false");
                return false;
            }
#else
            if (fallbackToDynamicTexture)
            {
                Utilities.LogError("fallbackToDynamicTexture=true which may lead to memory leaks on open videos or even crashes");
                return false;
            }
            var gfxDevice = SystemInfo.graphicsDeviceType;
            if (!passRawTextureDataOutside && gfxDevice != GraphicsDeviceType.Direct3D11)
            {
                Utilities.LogError($"graphicsDeviceType {gfxDevice} is detected, but passRawTextureDataOutside=false");
                return false;
            }
    #endif
#endif

            var giParams = new MediaGraphicsInterfaceOpenParams();
            giParams.devicePtr = devicePtr;
            giParams.deviceType = deviceType;
            giParams.useSrgbCompressedTextureFormats = useSrgbCompressedTextureFormats;
            giParams.fallbackToDynamicTexture = fallbackToDynamicTexture;
            giParams.passRawTextureDataOutside = passRawTextureDataOutside;

            var openingStarted = NativeDll.Open(_mediaId, generalParams, audioParams, giParams);
            if (!openingStarted)
            {
                IsOpening = false;
                Utilities.LogError("[DemolitionMedia] Failed to start opening media from url: " + openingUrl);
                return false;
            }
            IsOpening = true;

            // Force the opening state
            //_stateCached = MediaState.Opening;
            InvokeEvent(MediaEvent.Type.OpeningStarted);
            if (preloadToMemory)
            {
                InvokeEvent(MediaEvent.Type.PreloadingToMemoryStarted);
            }

            //Utilities.Log("[DemolitionMedia] Started opening media from url: " + openingUrl);

            // Update the cached media url
            mediaUrl = url;

            return true;
        }

        public void Close()
        {
            if (_mediaId < 0)
                return;

            IsClosing = true;

            // Reset media id
            var mediaIdClosing = _mediaId;
            _mediaId = -1;

            InvokeEvent(MediaEvent.Type.ClosingStarted);

            // Clear native textures
            CloseImpl();

            // Note: will first close media instance and then destroy media id
            NativeDll.DestroyMediaId(mediaIdClosing);
            mediaUrl = "";

            // Force the closed media state
            _stateCached = MediaState.Closed;

            ResetCachedProperties();
            _videoDecodeFramerateInterval = 1.0f;

            InvokeEvent(MediaEvent.Type.Closed);

            IsClosing = false;
        }

        public void Play()
        {
            PrePlayImpl();
            _videoDecodeFramerateTime = 0.0f;
            NativeDll.Play(_mediaId);
        }

        public void Pause()
        {
            _videoDecodeFramerateTime = 0.0f;
            NativeDll.Pause(_mediaId);
        }

        public void TogglePause()
        {
            _videoDecodeFramerateTime = 0.0f;
            NativeDll.TogglePause(_mediaId);
        }

        public void StepForward()
        {
            NativeDll.StepForward(_mediaId);
        }

        public void StepBackward()
        {
            NativeDll.StepBackward(_mediaId);
        }

        public void SeekToTime(float seconds)
        {
            NativeDll.SeekToTime(_mediaId, seconds);
        }

        public void SeekToFrame(int frame)
        {
            NativeDll.SeekToFrame(_mediaId, frame);
        }

        public bool Muted
        {
            get { return NativeDll.GetAudioMuted(_mediaId); }
        }

        public void SetMuted(bool muted)
        {
            NativeDll.SetAudioMuted(_mediaId, muted);
        }

        public bool AudioEnabled
        {
            get { return NativeDll.GetAudioEnabled(_mediaId); }
        }

        public void DisableAudio()
        {
            NativeDll.DisableAudio(_mediaId);
        }
        #endregion

        #region internal

        private void OnOpened()
        {
            Utilities.Log("[DemolitionMedia] Opened media with url: " + mediaUrl);

            // Cache the media parameters
            int width, height;
            NativeDll.GetResolution(_mediaId, out width, out height);
            VideoWidth = width;
            VideoHeight = height;

            DurationSeconds = NativeDll.GetDuration(_mediaId);
            VideoNumFrames = NativeDll.GetNumFrames(_mediaId);
            VideoFramerate = NativeDll.GetFramerate(_mediaId);

            bool flipX, flipY;
            NativeDll.GetNeedFlipVideo(_mediaId, out flipX, out flipY);
            VideoNeedFlipX = flipX;
            VideoNeedFlipY = flipY;

            VideoPixelFormat = NativeDll.GetPixelFormat(_mediaId);
            RequiresColorConversion = VideoPixelFormat == PixelFormat.YCoCg || VideoPixelFormat == PixelFormat.YCoCgAndAlphaP;
                
            //Utilities.Log("[DemolitionMedia] video pixel format: " + VideoPixelFormat);

            HasAudio = NativeDll.HasAudioStream(_mediaId);
            // Note: audio params like sample rate/channels will be only available after 1st audio frame decoded

            // Check hardware acceleration ability
            bool hasHardwareAcceleration = NativeDll.IsDecodingHardwareAccelerated(_mediaId);
            //Utilities.Log("[DemolitionMedia] Hardware acceleration: " + hasHardwareAcceleration);

            // Execute host application-dependent code
            OnOpenedImpl();

            FramedropEnabled = openWithFramedrop;

            // Start the playback if needed
            if (playOnOpen)
                Play();
        }

        private void PopulateErrors()
        {
            MediaError error = NativeDll.GetError(_mediaId);
            if (error != MediaError.NoError)
            {
                if (IsOpening)
                {
                    ResetCachedProperties();

                    InvokeEvent(MediaEvent.Type.OpenFailed, error);
                    IsOpening = false;

                    Utilities.LogError("[DemolitionMedia] " + mediaUrl + " | OpenFailed: " + error.ToString());
#if UNITY_5_3_OR_NEWER
                    if (mediaUrl.Contains("SampleVideos"))
                    {
                        if (NativeDll.IsProVersion())
                        {
                            Utilities.LogError("[DemolitionMedia] Please check out README Pro Sync.txt for sample videos download link");
                        }
                        else
                        {
                            Utilities.LogError("[DemolitionMedia] Please check out README.txt for sample videos download link");
                        }
                    }
#endif
                }
                else
                {
                    InvokeEvent(MediaEvent.Type.PlaybackErrorOccured, error);

                    Utilities.LogError("[DemolitionMedia] PlaybackErrorOccured: " + error.ToString());
                }
            }
        }

        private void PopulateEvents()
        {
            MediaState state = NativeDll.GetMediaState(_mediaId);
            if (state == _stateCached)
            {
                if (state == MediaState.Playing)
                {
                    var newLoop = NativeDll.IsNewLoop(_mediaId);
                    if (newLoop)
                    {
                        InvokeEvent(MediaEvent.Type.PlaybackNewLoop);
                    }
                }

                // No state change has happened
                return;
            }

            //if (_stateCached == MediaState.Closed && state == MediaState.Opening)
            //    return;
            if ((_stateCached == MediaState.Closed ||
                 _stateCached == MediaState.Opening ||
                 _stateCached == MediaState.PreloadingToMemory) &&
                 state != MediaState.Error &&
                 state != MediaState.Opening &&
                 state != MediaState.PreloadingToMemory)
            {
                //Utilities.Log("Cached: " + _stateCached.ToString() + ", new: " + state.ToString());

                // Note: sometimes the state can change directly from PreloadingToMemory to Opened
                if (_stateCached == MediaState.PreloadingToMemory)
                    InvokeEvent(MediaEvent.Type.PreloadingToMemoryFinished);

                // Run needed on-open actions (properties caching, etc)
                OnOpened();

                // Note: actually the media has finished opening before calling OnOpened, but we want 
                //       to be sure that all the needed properties are cached after we set this flag to false
                IsOpening = false;
                InvokeEvent(MediaEvent.Type.Opened);

                if (state == MediaState.Playing)
                    InvokeEvent(MediaEvent.Type.PlaybackStarted);
            }
            else if (_stateCached == MediaState.PreloadingToMemory)
            {
                InvokeEvent(MediaEvent.Type.PreloadingToMemoryFinished);
            }
            else if (_stateCached == MediaState.Stopped && state == MediaState.Playing)
            {
                InvokeEvent(MediaEvent.Type.PlaybackStarted);
            }
            else if (_stateCached == MediaState.Playing && state == MediaState.Paused 
                     || _stateCached == MediaState.Stopped && state == MediaState.Paused)
            {
                InvokeEvent(MediaEvent.Type.PlaybackSuspended);
            }
            else if (_stateCached == MediaState.Paused && state == MediaState.Playing)
            {
                InvokeEvent(MediaEvent.Type.PlaybackResumed);
            }
            else if (_stateCached == MediaState.Playing && state == MediaState.Stopped)
            {
                var finished = NativeDll.IsFinished(_mediaId);
                if (finished)
                {
                    InvokeEvent(MediaEvent.Type.PlaybackEndReached);
                }
            }
            if ((_stateCached == MediaState.Playing ||
                 _stateCached == MediaState.Paused) && state == MediaState.Stopped)
            {
                InvokeEvent(MediaEvent.Type.PlaybackStopped);
            }
            else if (state == MediaState.Closed)
            {
                InvokeEvent(MediaEvent.Type.Closed);
            }

            // Update the cached state
            _stateCached = state;
        }

        private void ResetCachedProperties()
        {
            DurationSeconds = 0.0f;
            VideoNumFrames = 0;
            VideoWidth = 0;
            VideoHeight = 0;
            VideoFramerate = 0.0f;
            StartTime = 0.0f;
            EndTime = 0.0f;
            StartFrame = 0;
            EndFrame = 0;
            VideoPixelFormat = PixelFormat.None;
            HasAudio = false;
        }
        #endregion

        public void Dispose()
        {
            Close();
            DisposeImpl();
        }
    }
}