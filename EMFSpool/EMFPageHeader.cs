using System.IO;
using System.Text;
using System.Drawing;

namespace EMFSpool
{
    public class EMFPageHeader
    {
        private readonly int type;
        private readonly int boundsLeft;
        private readonly int boundsTop;
        private readonly int boundsRight;
        private readonly int boundsBottom;

        private readonly int frameLeft;
        private readonly int frameTop;
        private readonly int frameRight;
        private readonly int frameBottom;

        private readonly byte signature1; // space
        private readonly byte signature2; // E
        private readonly byte signature3; // M
        private readonly byte signature4; // F

        private readonly uint version;
        private readonly short handleCount;
        private readonly short reserved;
        private readonly int descriptionLength;
        private readonly int descriptionOffset;
        private readonly int palEntries;
        private readonly int deviceWidth;
        private readonly int deviceHeight;
        private readonly int deviceWidthMilimeters;
        private readonly int deviceHeightMilimeters;
        private readonly int pixelFormatSize;
        private readonly int pixelFormatOffset;
        private readonly bool openGL;
        private readonly int deviceWidthMicrometers;
        private readonly int deviceHeightMicrometers;

        public int Size { get; }

        /// <summary>
        /// Gets the page limits (paper dimensions) 
        /// </summary>
        public Rectangle Bounds
        {
            get { return new Rectangle(boundsLeft, boundsTop, boundsRight, boundsBottom); }
        }

        /// <summary>
        /// You get the frame containing print elements. This can be smaller than the paper, because
        /// many printers have a non-printable margin at the edges.
        /// </summary>
        public Rectangle Frame
        {
            get { return new Rectangle(frameLeft, frameTop, frameRight, frameBottom); }
        }

        /// <summary>
        /// Returns the size of the metafile device in pixels
        /// </summary>
        public Size DevicePixelDimensions
        {
            get { return new Size(deviceWidth, deviceHeight); }
        }

        /// <summary>
        /// Returns the size of the metafile device in millimetres
        /// </summary>
        public Size DeviceMilimeterDimensions
        {
            get { return new Size(deviceWidthMilimeters, deviceHeightMilimeters); }
        }

        /// <summary>
        /// Returns the size of the metafile device in micrometers
        /// </summary>
        public Size DeviceMicrometerDimensions
        {
            get { return new Size(deviceWidthMicrometers, deviceHeightMicrometers); }
        }

        public int FileSize { get; }

        /// <summary>
        /// Obtain the number of records (EMF Records). These describe text elements
        /// and graphic elements that are part of the page.
        /// </summary>
        public int RecordCount { get; }

        /// <summary>
        /// Description containing the name of the application where the metafile was generated and the name of the figure
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Signature of the file ( " EMF " in case it has been correctly generated)
        /// </summary>
        public string Signature { get; }

        public EMFPageHeader(BinaryReader fileReader)
        {
            // Marks the start position for reading the fileStream
            long startPos = fileReader.BaseStream.Position;

            type = fileReader.ReadInt32();
            Size = fileReader.ReadInt32();

            boundsLeft = fileReader.ReadInt32();
            boundsTop = fileReader.ReadInt32();
            boundsRight = fileReader.ReadInt32();
            boundsBottom = fileReader.ReadInt32();

            frameLeft = fileReader.ReadInt32();
            frameTop = fileReader.ReadInt32();
            frameRight = fileReader.ReadInt32();
            frameBottom = fileReader.ReadInt32();

            signature1 = fileReader.ReadByte();
            signature2 = fileReader.ReadByte();
            signature3 = fileReader.ReadByte();
            signature4 = fileReader.ReadByte();

            version = fileReader.ReadUInt32();
            FileSize = fileReader.ReadInt32();
            RecordCount = fileReader.ReadInt32();
            handleCount = fileReader.ReadInt16();
            reserved = fileReader.ReadInt16();
            descriptionLength = fileReader.ReadInt32();
            descriptionOffset = fileReader.ReadInt32();
            palEntries = fileReader.ReadInt32();

            deviceWidth = fileReader.ReadInt32();
            deviceHeight = fileReader.ReadInt32();
            deviceWidthMilimeters = fileReader.ReadInt32();
            deviceHeightMilimeters = fileReader.ReadInt32();

            if (Size > 88)
            {
                pixelFormatSize = fileReader.ReadInt32();
                pixelFormatOffset = fileReader.ReadInt32();
                openGL = fileReader.ReadInt32() != 0;
            }

            if (Size > 100)
            {
                deviceWidthMicrometers = fileReader.ReadInt32();
                deviceHeightMicrometers = fileReader.ReadInt32();
            }

            if (descriptionLength > 0)
            {
                fileReader.BaseStream.Seek(startPos + descriptionOffset, SeekOrigin.Begin);
                Description = new string(StringResource.Get(fileReader));

                if (Description.Length + 2 < descriptionLength) // Checks if there is another string
                {
                    fileReader.ReadChars(1);
                    Description = Description + "#" + new string(StringResource.Get(fileReader));
                }
            }

            Signature = Encoding.UTF8.GetString(new byte[] { signature1, signature2, signature3, signature4 });
        }
    }

}
