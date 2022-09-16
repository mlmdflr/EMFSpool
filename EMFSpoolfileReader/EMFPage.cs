using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;


namespace EMFSpool
{
    public class EMFPage
    {
        private Metafile pageImage;

        private Image thumbnail;

        private List<EMFRecord> records;

        private bool pageProcessed;

        private MemoryStream pageStream;


        /// <summary>
        /// Get the header record of the page
        /// </summary>
        public EMFPageHeader Header
        {
            // To get the header it is not necessary to process the page, the header is read from the
            get;
        }

        /// <summary>
        /// Obtains a full-size image of the page (1:1 scale)
        /// </summary>
        public Metafile PageImage
        {
            // Use lazy instantiation to avoid consuming resources when they are not needed
            get
            {
                if (!pageProcessed) ProcessPage();
                return pageImage;
            }
        }

        /// <summary>
        /// Get a reduced image of the page. The size of the thumbnail is defined by its scale
        /// </summary>
        public Image Thumbnail
        {
            // Use lazy instantiation to avoid consuming resources when they are not needed
            get
            {
                if (!pageProcessed) ProcessPage();
                return thumbnail;
            }
        }

        /// <summary>
        /// Obtains the EMF records contained in the page
        /// </summary>
        public List<EMFRecord> Records
        {
            // Use lazy instantiation to avoid consuming resources when they are not needed
            get
            {
                if (!pageProcessed) ProcessPage();
                return records;
            }
        }


        // Assemble the page from your stream, the stream must be positioned for reading
        private void ProcessPage()
        {
            // Get the page's metafile and create the thumbnail
            pageImage = new Metafile(pageStream);
            int scale = 20;
            thumbnail = pageImage.GetThumbnailImage(Header.Frame.Width / scale, Header.Frame.Height / scale, null, IntPtr.Zero);
            pageStream.Seek(0, SeekOrigin.Begin);

            records = new List<EMFRecord>();
            for (int record = 1; record <= Header.RecordCount; record++) // The first record is the header
            {
                EMFRecord emfRecord = new EMFRecord(pageStream);
                records.Add(emfRecord);
                pageStream.Seek(emfRecord.RecSeek + emfRecord.RecSize, SeekOrigin.Begin);
            }
            pageProcessed = true;
        }

        /// <summary>
        /// Constructor of the class. Stores the contents of the EMF record (saved to disk) in
        /// memory for further processing
        /// </summary>
        public EMFPage(BinaryReader fileReader)
        {

            // Marks the start position for reading the fileStream
            long startPos = fileReader.BaseStream.Position;

            // Reads the header and returns to the starting position
            Header = new EMFPageHeader(fileReader);
            fileReader.BaseStream.Seek(startPos, SeekOrigin.Begin);

            byte[] buffer = fileReader.ReadBytes(Header.FileSize);
            pageProcessed = false;
            pageStream = new MemoryStream();
            pageStream.Write(buffer, 0, buffer.Length);
            pageStream.Seek(0, SeekOrigin.Begin);
        }

    }

}
