using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.DataType.Methods
{
    /// <summary>
    /// BinaryReader 类的扩展方法
    /// </summary>
    public static class BinaryReaderExtension
    {
        public static void ExpectUInt16(this BinaryReader reader, ushort expectedValue)
        {
            if (reader.ReadUInt16() != expectedValue)
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static void ExpectUInt32(this BinaryReader reader, uint expectedValue)
        {
            if (reader.ReadUInt32() != expectedValue)
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static void ExpectString(this BinaryReader reader, string s)
        {
            if (new string(reader.ReadChars(s.Length)) != s)
            {
                throw new InvalidDataException("Unexpected value read.");
            }
        }

        public static string ReadString(this BinaryReader reader, Encoding encoding, int length)
        {
            using BinaryReader r = new(reader.BaseStream, encoding, true);
            return new string(r.ReadChars(length));
        }

        public static string ReadNullTerminatedString(this BinaryReader reader, Encoding encoding)
        {
            using BinaryReader binaryReader = new(reader.BaseStream, encoding, true);
            List<char> charList = [];
            char c;
            while ((c = binaryReader.ReadChar()) is not '\0')
            {
                charList.Add(c);
            }
            return new string([.. charList]);
        }
    }
}
