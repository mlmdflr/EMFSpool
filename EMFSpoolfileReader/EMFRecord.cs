using System;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;


namespace EMFSpool
{
    public class EMFRecord
    {
        private readonly int type;

        public long RecSeek { get; }

        public EmfPlusRecordType RecType
        {
            get { return (EmfPlusRecordType)type; }
        }

        public int RecSize { get; }

        public byte[] Data { get; }

        /// <summary>
        /// Build the EMF record from a file (via fileReader)
        /// </summary>
        public EMFRecord(BinaryReader fileReader)
        {
            RecSeek = fileReader.BaseStream.Position;
            type = fileReader.ReadInt32();
            RecSize = fileReader.ReadInt32();
            Data = fileReader.ReadBytes(RecSize - 8);
        }

        /// <summary>
        /// Constructs the EMF register from its memory representation (gets pointer)
        /// </summary>
        public EMFRecord(IntPtr memoryAddress)
        {
            RecSeek = memoryAddress.ToInt64();

            int[] buffer1 = new int[1];
            Marshal.Copy(memoryAddress, buffer1, 0, 1);
            type = buffer1[0];

            int[] buffer2 = new int[1];
            Marshal.Copy(new IntPtr(memoryAddress.ToInt64() + 4), buffer2, 0, 1);
            RecSize = buffer2[0];

            Data = new byte[RecSize - 8];
            Marshal.Copy(new IntPtr(memoryAddress.ToInt64() + 8), Data, 0, RecSize - 8);
        }

        /// <summary>
        /// Constroi o registro EMF a partir de um stream, o stream deve estar posicionado para leitura
        /// </summary>
        public EMFRecord(Stream contentStream)
        {
            RecSeek = contentStream.Position;
            byte[] buffer = new byte[8];
            contentStream.Read(buffer, 0, 8);
            type = BitConverter.ToInt32(buffer, 0);
            RecSize = BitConverter.ToInt32(buffer, 4);
            Data = new byte[RecSize - 8];
            contentStream.Read(Data, 0, RecSize - 8);
        }
    }

}
