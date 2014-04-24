using Nimble.Native;
using System;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace Nimble
{
    public class VideoStream
    {
        private readonly IntPtr _handle;        
        private readonly StreamSettings _optional;
        private readonly StreamSettings _mandatory;
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
            var frame = ReadFrame();
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
            var frame = (OniFrame)Marshal.PtrToStructure(framePointer, typeof(OniFrame));
            return Frame.From(frame);
        }

        public interface IStreamSettings
        {
            bool Mirroring { get; set; }
        }

        private abstract class StreamSettings : IStreamSettings
        {
            private readonly VideoStream _outer;
            protected StreamSettings(VideoStream outer) { _outer = outer; }
            
            protected int ReadProperty(int propertyId)
            {
                int n = sizeof(int);
                IntPtr buffer = Marshal.AllocHGlobal(n);
                var status = OpenNI2.oniStreamGetProperty(_outer._handle, 7, buffer, ref n);
                int result = Marshal.ReadInt32(buffer);
                Marshal.FreeHGlobal(buffer);
                status.ThrowIfFailed();
                return result;
            }

            protected Status WritePropertyRaw(int propertyId, int value)
            {
                int n = sizeof(int);
                IntPtr buffer = Marshal.AllocHGlobal(n);
                Marshal.WriteInt32(buffer, value);
                var status = OpenNI2.oniStreamSetProperty(_outer._handle, 7, buffer, n);
                Marshal.FreeHGlobal(buffer);
                return status;
            }

            protected abstract void WriteProperty(int propertyId, int value);

            public bool Mirroring
            {
                get { return ReadProperty(7) != 0; }
                set { WriteProperty(7, value ? 1 : 0); }
            }
        }

        private class OptionalStreamSettings: StreamSettings {
            public OptionalStreamSettings(VideoStream outer) : base(outer) { }
            protected override void WriteProperty(int propertyId, int value)
            {
                var status = WritePropertyRaw(propertyId, value);
                if (status != Status.NotSupported) status.ThrowIfFailed();
            }
        }

        private class MandatoryStreamSettings : StreamSettings
        {
            public MandatoryStreamSettings(VideoStream outer) : base(outer) { }
            protected override void WriteProperty(int propertyId, int value)
            {
                WritePropertyRaw(propertyId, value).ThrowIfFailed();
            }
        }

        public IStreamSettings Optional { get { return _optional; } }
        public IStreamSettings Mandatory { get { return _mandatory; } }

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
