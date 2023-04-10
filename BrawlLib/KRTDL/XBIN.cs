using BrawlLib.Internal;
using System.Runtime.InteropServices;

namespace BrawlLib.SSBB.Types
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct XBIN // based on PLT0v1
    {
        public const uint Tag = 0x4E494258; // "XBIN", endian reversed
        public BinTag _tag;                 // XBIN
        public bshort _magic;               // 1234
        public bshort _version;             // 0200
        public bint   _size;                // unpadded size (in file usually padded to nearest 32 I think)

        private XBIN* Address
        {
            get
            {
                fixed (XBIN* ptr = &this)
                {
                    return ptr;
                }
            }
        }

        public int Size => (int)_size;
        // public int PaddedSize => ((RawSize + 31) / 32) * 32; // left commented out, in case needed later
    }
}