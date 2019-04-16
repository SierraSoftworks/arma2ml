using System;
using System.Text;

namespace SierraLib.ByteManipulation
{
    public static class CreationFunctions
    {
        public static int AddByte(ref byte[] ChunkData, byte value)
        {
            byte[] temp = new byte[ChunkData.Length + 1];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            temp[temp.Length - 1] = value;

            ChunkData = temp;
            return 1;
        }

        public static int AddInt16(ref byte[] ChunkData, Int16 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte[] temp = new byte[ChunkData.Length + 2];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            Array.Copy(bytes, 0, temp, ChunkData.Length, 2);

            ChunkData = temp;
            return 2;
        }

        public static int AddInt32LE(ref byte[] ChunkData, Int32 value)
        {
            UInt32 uvalue = (UInt32)value;

            UInt32 swapped = (
            (0x000000FF) & (uvalue >> 24)

            | (0x0000FF00) & (uvalue >> 8)

            | (0x00FF0000) & (uvalue << 8)

            | (0xFF000000) & (uvalue << 24)
);

            byte[] bytes = BitConverter.GetBytes(swapped);
            byte[] temp = new byte[ChunkData.Length + 4];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            Array.Copy(bytes, 0, temp, ChunkData.Length, 4);

            ChunkData = temp;
            return 4;
        }

        public static int AddInt32(ref byte[] ChunkData, Int32 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte[] temp = new byte[ChunkData.Length + 4];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            Array.Copy(bytes, 0, temp, ChunkData.Length, 4);

            ChunkData = temp;
            return 4;
        }

        public static int AddInt64(ref byte[] ChunkData, Int64 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            byte[] temp = new byte[ChunkData.Length + 8];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            Array.Copy(bytes, 0, temp, ChunkData.Length, 8);

            ChunkData = temp;
            return 8;
        }

        public static int AddInt64LE(ref byte[] ChunkData, Int64 value)
        {
            UInt64 uvalue = (UInt64)value;

            UInt64 swapped = ((0x00000000000000FF) & (uvalue >> 56)

            | (0x000000000000FF00) & (uvalue >> 40)

            | (0x0000000000FF0000) & (uvalue >> 24)

            | (0x00000000FF000000) & (uvalue >> 8)

            | (0x000000FF00000000) & (uvalue << 8)

            | (0x0000FF0000000000) & (uvalue << 24)

            | (0x00FF000000000000) & (uvalue << 40)

            | (0xFF00000000000000) & (uvalue << 56));

            byte[] bytes = BitConverter.GetBytes(swapped);

            byte[] temp = new byte[ChunkData.Length + 8];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            Array.Copy(bytes, 0, temp, ChunkData.Length, 8);

            ChunkData = temp;
            return 8;
        }

        public static int AddData(ref byte[] ChunkData, byte[] Data)
        {
            byte[] temp = new byte[ChunkData.Length + Data.Length];
            for (int i = 0; i < ChunkData.Length; i++)
            {
                temp[i] = ChunkData[i];
            }

            for (int i = ChunkData.Length, i2 = 0; i < temp.Length; i++, i2++)
            {
                temp[i] = Data[i2];
            }

            ChunkData = temp;
            return Data.Length;
        }

        public static int AddUTF8String(ref byte[] ChunkData, string Value, int FieldLength)
        {
            byte[] val = new byte[0];

            if (Value.Length > FieldLength)
                Value = Value.Substring(0, FieldLength);

            //Prevent null strings from being processed
            if (!String.IsNullOrEmpty(Value))
                val = Encoding.UTF8.GetBytes(Value);

            int length = 0;
            length += AddInt16(ref ChunkData, (short)val.Length);

            //Don't add null data
            if (val.Length > 0)
                length += AddData(ref ChunkData, val);

            if (val.Length < FieldLength)
                length += AddData(ref ChunkData, new byte[FieldLength - val.Length]);

            return length;
        }
    }
}