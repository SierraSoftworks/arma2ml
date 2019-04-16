using System;
using System.Text;

namespace SierraLib.ByteManipulation
{
    /// <summary>
    /// Functions for extracting bytes from popular data types.
    /// </summary>
    public static class ParsingFunctions
    {
        /// <summary>
        /// Extracts a 1-Byte data chunk from a
        /// <see cref="byte"/> array and outputs
        /// it to a <see cref="byte"/> object.
        /// </summary>
        /// <param name="ChunkData">
        /// The <see cref="byte"/> array containing
        /// the byte to be extracted.
        /// </param>
        /// <param name="Position">
        /// The position at which the byte will be
        /// extracted.
        /// </param>
        /// <param name="Output">
        /// The <see cref="byte"/> object that will recieve
        /// the value.
        /// </param>
        /// <returns>
        /// Returns an <see cref="int"/> that indicates the
        /// length of data that was read from the array.
        /// </returns>
        public static int ExtractByte(byte[] ChunkData, int Position, out byte Output)
        {
            Output = ChunkData[Position];
            return sizeof(byte);
        }

        public static int ExtractInt16(byte[] ChunkData, int Position, out Int16 Output)
        {
            Int16 value = 0;
            value = (Int16)(value << 8);
            value |= ChunkData[Position];
            value = (Int16)(value << 8);
            value |= ChunkData[Position + 1];

            Output = value;

            return sizeof(Int16);
        }

        public static int ExtractInt16LE(byte[] ChunkData, int Position, out Int16 Output)
        {
            Int16 value = 0;
            value = (Int16)(value << 8);
            value |= ChunkData[Position];
            value = (Int16)(value << 8);
            value |= ChunkData[Position + 1];

            UInt16 swapped = (UInt16)((0x00FF) & (value >> 8) | (0xFF00) & (value << 8));

            Output = (Int16)swapped;

            return sizeof(Int16);
        }

        public static int ExtractInt32(byte[] ChunkData, int Position, out Int32 Output)
        {
            Int32 value = 0;
            value = value << 8;
            value |= ChunkData[Position];
            value = value << 8;
            value |= ChunkData[Position + 1];
            value = value << 8;
            value |= ChunkData[Position + 2];
            value = value << 8;
            value |= ChunkData[Position + 3];

            Output = value;
            return sizeof(Int32);
        }

        public static int ExtractInt64(byte[] ChunkData, int Position, out Int64 Output)
        {
            if (Position + 7 >= ChunkData.Length)
            {
                throw new ArgumentOutOfRangeException("Position", "The position should allow enough space in the array for a 64 bit integer to exist");
            }
            Int64 value = 0;
            value = value << 8;
            value |= ChunkData[Position];
            value = value << 8;
            value |= ChunkData[Position + 1];
            value = value << 8;
            value |= ChunkData[Position + 2];
            value = value << 8;
            value |= ChunkData[Position + 3];
            value = value << 8;
            value |= ChunkData[Position + 4];
            value = value << 8;
            value |= ChunkData[Position + 5];
            value = value << 8;
            value |= ChunkData[Position + 6];
            value = value << 8;
            value |= ChunkData[Position + 7];

            Output = value;

            return sizeof(Int64);
        }

        public static int ExtractData(byte[] ChunkData, int Position, int DataLength, out byte[] Output)
        {
            byte[] temp = new byte[DataLength];

            for (int i = Position, i2 = 0; i < Position + DataLength; i++, i2++)
            {
                temp[i2] = ChunkData[i];
            }

            Output = temp;
            return DataLength;
        }

        public static int ExtractUTF8String(byte[] ChunkData, int Position, out string Output)
        {
            int pos = Position;
            Int16 length;
            pos += ExtractInt16LE(ChunkData, pos, out length);

            byte[] temp = null;
            pos += ExtractData(ChunkData, pos, length, out temp);

            Output = Encoding.UTF8.GetString(temp);

            return pos - Position;
        }
    }
}