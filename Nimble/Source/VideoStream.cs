using Nimble.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nimble
{
    public class VideoStream
    {
        private readonly IntPtr _handle;
        private readonly OptionalStreamSettings _optional;
        private readonly MandatoryStreamSettings _mandatory;
        private readonly OniNewFrameCallback _callback;
        private IntPtr _callbackHandle;
        private int _subscribers = 0;

        private event Action<VideoStream, Frame> _onNewFrame;

        internal VideoStream(Device device, OniSensorType sensorType)
        {
            var status = OpenNI2.oniDeviceCreateStream(device.Handle, sensorType, out _handle);
            status.ThrowIfFailed();
            _callback = new OniNewFrameCallback(NewFrameCallback);
            _optional = new OptionalStreamSettings(this);
            _mandatory = new MandatoryStreamSettings(this);
        }

        private void NewFrameCallback(IntPtr streamHandle, IntPtr cookie)
        {
            var tmp = _onNewFrame;
            using (var frame = ReadFrame())
            if (tmp != null) tmp(this, frame);
        }

        public event Action<VideoStream, Frame> NewFrame
        {
            add
            {
                _onNewFrame += value;
                if (_subscribers == 0)
                {
                    var status = OpenNI2.oniStreamRegisterNewFrameCallback(_handle, _callback, IntPtr.Zero, out _callbackHandle);
                    status.ThrowIfFailed();
                }
                _subscribers++;
            }
            remove
            {
                _onNewFrame -= value;
                _subscribers--;
                if (_subscribers == 0)
                {
                    OpenNI2.oniStreamUnregisterNewFrameCallback(_handle, _callbackHandle);
                }
            }
        }

        public void Start()
        {
            var status = OpenNI2.oniStreamStart(_handle);
            status.ThrowIfFailed();
        }

        public Frame ReadFrame()
        {
            IntPtr framePointer = IntPtr.Zero;
            var status = OpenNI2.oniStreamReadFrame(_handle, out framePointer);
            status.ThrowIfFailed();
            return Frame.From(framePointer);
        }

        public interface IStreamSettings
        {
            bool Mirroring { get; set; }
            bool AutoWhiteBalance { get; set; }
            bool AutoExposure { get; set; }
        }

        private abstract class StreamSettings : IStreamSettings
        {
            private readonly VideoStream _outer;
            protected StreamSettings(VideoStream outer) { _outer = outer; }
            
            protected int ReadIntProperty(int propertyId)
            {
                int n = sizeof(int);
                IntPtr buffer = Marshal.AllocHGlobal(n);
                var status = OpenNI2.oniStreamGetProperty(_outer._handle, propertyId, buffer, ref n);
                int result = Marshal.ReadInt32(buffer);
                Marshal.FreeHGlobal(buffer);
                Handle(status);
                return result;
            }

            protected void WritePropertyRaw(int propertyId, int value)
            {
                int n = sizeof(int);
                IntPtr buffer = Marshal.AllocHGlobal(n);
                Marshal.WriteInt32(buffer, value);
                var status = OpenNI2.oniStreamSetProperty(_outer._handle, propertyId, buffer, n);
                Marshal.FreeHGlobal(buffer);
                Handle(status);
            }

            public void WriteProperty<T>(int propertyId, T s) where T: struct
            {
                int n = Marshal.SizeOf(s);
                IntPtr buffer = Marshal.AllocHGlobal(n);
                Marshal.StructureToPtr(s, buffer, false);
                var status = OpenNI2.oniStreamSetProperty(_outer._handle, propertyId, buffer, n);
                Marshal.FreeHGlobal(buffer);
                Handle(status);
            }

            public T ReadProperty<T>(int propertyId) where T : struct
            {
                int n = Marshal.SizeOf(typeof(T));
                IntPtr buffer = Marshal.AllocHGlobal(n);
                var status = OpenNI2.oniStreamGetProperty(_outer._handle, propertyId, buffer, ref n);
                T result = (T)Marshal.PtrToStructure(buffer, typeof(T));
                Marshal.FreeHGlobal(buffer);
                Handle(status);
                return result;
            }

            protected void WriteProperty(int propertyId, bool value) { WriteProperty(propertyId, value ? 1 : 0); }
            protected abstract void Handle(Status status);

            public bool Mirroring
            {
                get { return ReadIntProperty(7) != 0; }
                set { WriteProperty(7, value); }
            }
            public bool AutoWhiteBalance
            {
                get { return ReadIntProperty(100) != 0; }
                set { WriteProperty(100, value); }
            }
            public bool AutoExposure
            {
                get { return ReadIntProperty(101) != 0; }
                set { WriteProperty(101, value); }
            }
        }

        private class OptionalStreamSettings: StreamSettings {
            public OptionalStreamSettings(VideoStream outer) : base(outer) { }
            protected override void Handle(Status status)
            {
                if (status == Status.NotSupported) return;
                status.ThrowIfFailed();
            }
        }

        private class MandatoryStreamSettings : StreamSettings
        {
            public MandatoryStreamSettings(VideoStream outer) : base(outer) { }
            protected override void Handle(Status status)
            {
                status.ThrowIfFailed();
            }
        }

        public IStreamSettings Optional { get { return _optional; } }
        public IStreamSettings Mandatory { get { return _mandatory; } }

        private VideoMode ToVideoMode(OniVideoMode videoMode)
        {
            return new VideoMode(videoMode.resolutionX, videoMode.resolutionY, videoMode.fps, videoMode.pixelFormat.ToNimblePixelFormat());
        }

        public IEnumerable<VideoMode> SupportedVideoModes
        {
            get
            {
                IntPtr rawSensorInfo = OpenNI2.oniStreamGetSensorInfo(_handle);
                OniSensorInfo sensorInfo = (OniSensorInfo)Marshal.PtrToStructure(rawSensorInfo, typeof(OniSensorInfo));
                return 
                    Marshaller.EnumerateStructArray<OniVideoMode>(sensorInfo.pSupportedVideoModes, sensorInfo.numSupportedVideoModes).
                    Select(ToVideoMode).ToList();
            }
        }

        public VideoMode VideoMode
        {
            get { return ToVideoMode(_mandatory.ReadProperty<OniVideoMode>(3)); }
            set
            {
                OniVideoMode videoMode = value.ToRaw();
                _mandatory.WriteProperty(3, videoMode);
            }
        }

        public void Stop()
        {
            OpenNI2.oniStreamStop(_handle);
        }

        public void Close()
        {
            OpenNI2.oniStreamDestroy(_handle);
        }
    }
}
