using System.IO;
using System.Collections.Generic;

namespace EMFSpool
{
    public class EMFSpoolFile
    {

        protected long startPos;

        public List<EMFPage> Pages = new List<EMFPage>();

        public int FileSize;

        public bool MalformedFile { get; }

        public string JobDescription { get; private set; }


        public EMFSpoolFile(BinaryReader fileReader)
        {

            if (!GetJobInfo(fileReader))
            {
                MalformedFile = true;
                return;
            }

            SPLRecord nextRecord = new SPLRecord(fileReader);
            while (nextRecord.RecType != SPLRecordTypeEnum.SRT_EOF)
            {
                ProcessSPLRecord(nextRecord, fileReader);
                nextRecord = new SPLRecord(fileReader);
            }
        }

        private bool GetJobInfo(BinaryReader fileReader)
        {
            SPLRecord record = new SPLRecord(fileReader);
            long recSeek = record.RecSeek;
            if (record.RecType != SPLRecordTypeEnum.SRT_JOB_INFO)
                return false;

            fileReader.ReadBytes(8);
            char[] jobDescriptionArray = StringResource.Get(fileReader);
            JobDescription = new string(jobDescriptionArray);
            fileReader.BaseStream.Seek(recSeek, SeekOrigin.Begin);
            if (string.IsNullOrEmpty(JobDescription))
                return false;

            return true;
        }

        private void ProcessSPLRecord(SPLRecord record, BinaryReader fileReader)
        {
            long recSeek = record.RecSeek;
            int recSize = record.RecSize;
            if (recSize <= 0) recSize = 8;

            switch (record.RecType)
            {
                case SPLRecordTypeEnum.SRT_JOB_INFO:
                    fileReader.BaseStream.Seek(recSeek + recSize, SeekOrigin.Begin);
                    break;
                case SPLRecordTypeEnum.SRT_EOF:
                    break;
                case SPLRecordTypeEnum.SRT_DEVMODE:
                    fileReader.BaseStream.Seek(recSeek + 8 + recSize, SeekOrigin.Begin);
                    break;
                case SPLRecordTypeEnum.SRT_PAGE:
                case SPLRecordTypeEnum.SRT_EXT_PAGE:
                    ProcessEMFPage(record, fileReader);
                    break;
                case SPLRecordTypeEnum.SRT_EOPAGE1:
                case SPLRecordTypeEnum.SRT_EOPAGE2:
                    byte[] bytes = fileReader.ReadBytes(recSize);
                    if (recSize == 0x08)
                        fileReader.BaseStream.Seek(recSeek + recSize + 8, SeekOrigin.Begin);
                    break;
                case SPLRecordTypeEnum.SRT_EXT_FONT2:
                    fileReader.BaseStream.Seek(recSeek + 4, SeekOrigin.Begin);
                    break;
                default:
                    fileReader.BaseStream.Seek(recSeek + recSize, SeekOrigin.Begin);
                    break;
            }
        }

        private void ProcessEMFPage(SPLRecord record, BinaryReader fileReader)
        {
            long nextRecordStart = record.RecSeek + 8;
            fileReader.BaseStream.Seek(nextRecordStart, SeekOrigin.Begin);

            EMFPage emfPage = new EMFPage(fileReader);
            Pages.Add(emfPage);

            nextRecordStart = nextRecordStart + emfPage.Header.FileSize;
            fileReader.BaseStream.Seek(nextRecordStart, SeekOrigin.Begin);
        }
    }

}
