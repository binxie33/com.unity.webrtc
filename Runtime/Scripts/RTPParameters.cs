using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Unity.WebRTC
{
    public class RTCRtpEncodingParameters
    {
        public bool active;
        public ulong? maxBitrate;
        public ulong? minBitrate;
        public uint? maxFramerate;
        public double? scaleResolutionDownBy;
        public string rid;

        internal RTCRtpEncodingParameters(RTCRtpEncodingParametersInternal parameter)
        {
            active = parameter.active;
            if (parameter.hasValueMaxBitrate)
                maxBitrate = parameter.maxBitrate;
            if (parameter.hasValueMinBitrate)
                minBitrate = parameter.minBitrate;
            if (parameter.hasValueMaxFramerate)
                maxFramerate = parameter.maxFramerate;
            if (parameter.hasValueScaleResolutionDownBy)
                scaleResolutionDownBy = parameter.scaleResolutionDownBy;
            if(parameter.rid != IntPtr.Zero)
                rid = parameter.rid.AsAnsiStringWithFreeMem();
        }

        internal void CopyInternal(ref RTCRtpEncodingParametersInternal instance)
        {
            instance.active = active;
            instance.hasValueMaxBitrate = maxBitrate.HasValue;
            if(maxBitrate.HasValue)
                instance.maxBitrate = maxBitrate.Value;
            instance.hasValueMinBitrate = minBitrate.HasValue;
            if (minBitrate.HasValue)
                instance.minBitrate = minBitrate.Value;
            instance.hasValueMaxFramerate = maxFramerate.HasValue;
            if (maxFramerate.HasValue)
                instance.maxFramerate = maxFramerate.Value;
            instance.hasValueScaleResolutionDownBy = scaleResolutionDownBy.HasValue;
            if (scaleResolutionDownBy.HasValue)
                instance.scaleResolutionDownBy = scaleResolutionDownBy.Value;
            instance.rid = string.IsNullOrEmpty(rid) ? IntPtr.Zero : Marshal.StringToCoTaskMemAnsi(rid);
        }
    }

    public class RTCRtpSendParameters
    {
        public string TransactionId => _transactionId;

        public RTCRtpEncodingParameters[] Encodings => _encodings;

        readonly RTCRtpEncodingParameters[] _encodings;
        readonly string _transactionId;

        internal RTCRtpSendParameters(RTCRtpSendParametersInternal parameters)
        {
            int length = parameters.encodingsLength;
            RTCRtpEncodingParametersInternal[] encodings =
                parameters.encodings.AsArray<RTCRtpEncodingParametersInternal>(length);
            _encodings = Array.ConvertAll(encodings, _ => new RTCRtpEncodingParameters(_));
            _transactionId = parameters.transactionId.AsAnsiStringWithFreeMem();
        }

        internal IntPtr CreatePtr()
        {
            RTCRtpEncodingParametersInternal[] encodings =
                new RTCRtpEncodingParametersInternal[_encodings.Length];
            for(int i = 0; i < _encodings.Length; i++)
            {
                _encodings[i].CopyInternal(ref encodings[i]);
            }
            RTCRtpSendParametersInternal instance = default;
            instance.encodingsLength = _encodings.Length;
            instance.encodings = IntPtrExtension.ToPtr(encodings);
            instance.transactionId = Marshal.StringToCoTaskMemAnsi(_transactionId);
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(instance));
            Marshal.StructureToPtr(instance, ptr, false);
            return ptr;
        }

        static internal void DeletePtr(IntPtr ptr)
        {
            var instance = Marshal.PtrToStructure<RTCRtpSendParametersInternal>(ptr);
            Marshal.FreeCoTaskMem(instance.encodings);
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCRtpSendParametersInternal
    {
        public int encodingsLength;
        public IntPtr encodings;
        public IntPtr transactionId;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RTCRtpEncodingParametersInternal
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool active;
        [MarshalAs(UnmanagedType.U1)]
        public bool hasValueMaxBitrate;
        public ulong maxBitrate;
        [MarshalAs(UnmanagedType.U1)]
        public bool hasValueMinBitrate;
        public ulong minBitrate;
        [MarshalAs(UnmanagedType.U1)]
        public bool hasValueMaxFramerate;
        public uint maxFramerate;
        [MarshalAs(UnmanagedType.U1)]
        public bool hasValueScaleResolutionDownBy;
        public double scaleResolutionDownBy;
        public IntPtr rid;
    }
}
