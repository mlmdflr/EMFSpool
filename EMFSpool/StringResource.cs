using System.IO;


namespace EMFSpool
{
    public static class StringResource
    {
     
        public static char[] Get(BinaryReader reader)
        {
            long startPos = reader.BaseStream.Position;

            int size = 0;
            char nextChar = reader.ReadChar();
            while ((nextChar != 0) && (reader.BaseStream.Position <= reader.BaseStream.Length))
            {
                size++;
                nextChar = reader.ReadChar();
            }

            reader.BaseStream.Seek(startPos, SeekOrigin.Begin);
            char[] stringResource = reader.ReadChars(size);

            return stringResource;
        }
    }

}
