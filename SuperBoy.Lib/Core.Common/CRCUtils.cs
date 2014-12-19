using System.Text;

namespace Core.Common
{
    /// <summary>
    /// CRC–£—È∏®÷˙¿‡
    /// </summary>
    public sealed class CrcUtils
    {
        private static ushort[] _crc16Table = null;
        private static uint[] _crc32Table = null;

        private static void MakeCrc16Table()
        {
            if (_crc16Table != null) return;
            _crc16Table = new ushort[256];
            for (ushort i = 0; i < 256; i++)
            {
                var vCrc = i;
                for (var j = 0; j < 8; j++)
                    if (vCrc % 2 == 0)
                        vCrc = (ushort)(vCrc >> 1);
                    else vCrc = (ushort)((vCrc >> 1) ^ 0x8408);
                _crc16Table[i] = vCrc;
            }
        }

        private static void MakeCrc32Table()
        {
            if (_crc32Table != null) return;
            _crc32Table = new uint[256];
            for (uint i = 0; i < 256; i++)
            {
                var vCrc = i;
                for (var j = 0; j < 8; j++)
                    if (vCrc % 2 == 0)
                        vCrc = (uint)(vCrc >> 1);
                    else vCrc = (uint)((vCrc >> 1) ^ 0xEDB88320);
                _crc32Table[i] = vCrc;
            }
        }

        private static ushort UpdateCrc16(byte aByte, ushort aSeed)
        {
            return (ushort)(_crc16Table[(aSeed & 0x000000FF) ^ aByte] ^ (aSeed >> 8));
        }

        private static uint UpdateCrc32(byte aByte, uint aSeed)
        {
            return (uint)(_crc32Table[(aSeed & 0x000000FF) ^ aByte] ^ (aSeed >> 8));
        }

        public static ushort CRC16(byte[] aBytes)
        {
            MakeCrc16Table();
            ushort result = 0xFFFF;
            foreach (var vByte in aBytes)
                result = UpdateCrc16(vByte, result);
            return (ushort)(~result);
        }

        public static ushort CRC16(string aString, Encoding aEncoding)
        {
            return CRC16(aEncoding.GetBytes(aString));
        }

        public static ushort CRC16(string aString)
        {
            return CRC16(aString, Encoding.UTF8);
        }

        public static uint CRC32(byte[] aBytes)
        {
            MakeCrc32Table();
            var result = 0xFFFFFFFF;
            foreach (var vByte in aBytes)
                result = UpdateCrc32(vByte, result);
            return (uint)(~result);
        }

        public static uint CRC32(string aString, Encoding aEncoding)
        {
            return CRC32(aEncoding.GetBytes(aString));
        }

        public static uint CRC32(string aString)
        {
            return CRC32(aString, Encoding.UTF8);
        }
    }

}
