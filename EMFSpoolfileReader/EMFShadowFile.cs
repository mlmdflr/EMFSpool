using System;
using System.IO;
using System.Management;

namespace EMFSpool
{
    public class JobShadowFile
    {
        private readonly SHDFileFormatEnum fileFormat;
        private readonly int systemType = 32; // by default considers as being a 32-bit operational system

        private readonly int headerSize;
        private readonly int priority;

        private readonly int offsetUserName;
        private readonly int offsetNotifyName;
        private readonly int offsetDocumentName;
        private readonly int offsetPort;
        private readonly int offsetPrinterName;
        private readonly int offsetDriverName;
        private readonly int offsetDevMode;
        private readonly int offsetPrintProcessor;
        private readonly int offsetDataType;

        private readonly short year;
        private readonly short month;
        private readonly short dayOfWeek;
        private readonly short day;
        private readonly short hour;
        private readonly short minute;
        private readonly short second;
        private readonly short millisecond;

        private readonly int startTime;
        private readonly int endTime;


        public string DataType { get; }


        public string DocumentName { get; }

        public int PageCount { get; }

        public string DriverName { get; }


        public int JobId { get; }

        public string NotifyName { get; }

        public string Port { get; }

        public string PrinterName { get; }

        public string PrintProcessor { get; }

        public DateTime Submitted
        {
            get { return new DateTime(year, month, day, hour, minute, second); }
        }

        public string UserName { get; }

        public int SpoolFileSize { get; }

        private int ReadInteger(BinaryReader fileReader)
        {
            if (systemType == 64) // if the operating system is 64-bit it makes one more reading
                fileReader.ReadInt32();
            return fileReader.ReadInt32();
        }

        private string ReadString(BinaryReader fileReader, int offset)
        {
            // If the offset is not correctly informed returns "null".
            if (offset < 1) return null;

            fileReader.BaseStream.Seek(offset, SeekOrigin.Begin);
            return new string(StringResource.Get(fileReader));
        }

        private bool Is64BitOperatingSystem()
        {
            try
            {
                ManagementObjectSearcher osSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectCollection operatingSystemCollection = osSearcher.Get();
                foreach (ManagementObject operatingSystem in operatingSystemCollection)
                {
                    string osArchitecture = (string)operatingSystem.GetPropertyValue("OSArchitecture");
                    if (osArchitecture.Contains("64")) return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public JobShadowFile(BinaryReader fileReader)
        {
            fileFormat = (SHDFileFormatEnum)fileReader.ReadInt32();
            if (Is64BitOperatingSystem()) systemType = 64; // 64-bit operating system
            // In Windows 2000 and Windows 2003 shadow files there is the "HeaderSize"
            if ((fileFormat == SHDFileFormatEnum.SHD_SIGNATURE_WIN2K) ||
                  (fileFormat == SHDFileFormatEnum.SHD_SIGNATURE_WIN2003))
            {
                headerSize = fileReader.ReadInt32();
            }
            // Unknown attribute, occupies 16 bits (2 bytes) in the file
            short unknown = fileReader.ReadInt16();
            JobId = fileReader.ReadInt32();
            priority = fileReader.ReadInt32();

            offsetUserName = ReadInteger(fileReader);
            offsetNotifyName = ReadInteger(fileReader);
            offsetDocumentName = ReadInteger(fileReader);
            offsetPort = ReadInteger(fileReader);
            offsetPrinterName = ReadInteger(fileReader);
            offsetDriverName = ReadInteger(fileReader);
            offsetDevMode = ReadInteger(fileReader);
            offsetPrintProcessor = ReadInteger(fileReader);
            offsetDataType = ReadInteger(fileReader);

            long offsetSubmitted = 4;
            if (systemType == 64) offsetSubmitted = 12;
            fileReader.BaseStream.Seek(offsetSubmitted, SeekOrigin.Current);
            year = fileReader.ReadInt16();
            month = fileReader.ReadInt16();
            dayOfWeek = fileReader.ReadInt16();
            day = fileReader.ReadInt16();
            hour = fileReader.ReadInt16();
            minute = fileReader.ReadInt16();
            second = fileReader.ReadInt16();
            millisecond = fileReader.ReadInt16();

            startTime = fileReader.ReadInt32();
            endTime = fileReader.ReadInt32();

            SpoolFileSize = fileReader.ReadInt32();
            PageCount = fileReader.ReadInt32();

            fileReader.BaseStream.Seek(offsetDevMode, SeekOrigin.Begin);

            UserName = ReadString(fileReader, offsetUserName);
            NotifyName = ReadString(fileReader, offsetNotifyName);
            DocumentName = ReadString(fileReader, offsetDocumentName);
            Port = ReadString(fileReader, offsetPort);
            PrinterName = ReadString(fileReader, offsetPrinterName);
            DriverName = ReadString(fileReader, offsetDriverName);
            PrintProcessor = ReadString(fileReader, offsetPrintProcessor);
            DataType = ReadString(fileReader, offsetDataType);
        }
    }

}
