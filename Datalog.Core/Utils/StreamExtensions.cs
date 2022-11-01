﻿using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.Core.Utils
{
    public static class StreamExtensions
    {
        public static TStruct ReadStruct<TStruct>(this Stream stream, int? offset = null, int? length = null, bool bigEndian = false) where TStruct : struct
        {
            var structSize = Marshal.SizeOf<TStruct>();
            var ptr = Marshal.AllocHGlobal(structSize);
            try
            {
                var buffer = new byte[length ?? structSize];
                stream.Read(buffer, offset ?? 0, (length ?? structSize) - (offset ?? 0));

                if (bigEndian)
                    buffer = buffer.Reverse().ToArray();

                Marshal.Copy(buffer, 0, ptr, structSize);

                return Marshal.PtrToStructure<TStruct>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static TStruct ReadBigEndianStruct<TStruct>(this Stream stream, int? offset = null, int? length = null) where TStruct : struct
        {
            return stream.ReadStruct<TStruct>(offset, length, true);
        }

        public static void WriteStruct<TStruct>(this Stream stream, TStruct @struct, int? offset = null, int? length = null) where TStruct : struct
        {
            var structSize = Marshal.SizeOf<TStruct>();
            var ptr = Marshal.AllocHGlobal(structSize);
            try
            {
                var buffer = new byte[length ?? structSize];
                Marshal.StructureToPtr(@struct, ptr, false);
                Marshal.Copy(ptr, buffer, offset ?? 0, structSize - (offset ?? 0));
                stream.Write(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}