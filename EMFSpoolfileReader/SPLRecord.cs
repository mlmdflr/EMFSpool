using System.IO;


namespace EMFSpool
{
    public class SPLRecord
    {
        private readonly int type;

        public long RecSeek { get; }

        public SPLRecordTypeEnum RecType
        {
            get { return (SPLRecordTypeEnum)type; }
        }

        public int RecSize { get; }


        public SPLRecord(BinaryReader fileReader)
        {
            RecSeek = fileReader.BaseStream.Position;
            try
            {
                type = fileReader.ReadInt32();
            }
            catch (EndOfStreamException)
            {
                type = (int)SPLRecordTypeEnum.SRT_EOF;
                return;
            }
            RecSize = fileReader.ReadInt32();
        }
    }

}
