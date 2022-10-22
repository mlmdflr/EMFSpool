# EMFSpool
C# support for .SHD and .SPL files on the win32

# Use

```c#
using System.Text;
using System.IO;
using System.Diagnostics;

using EMFSpool;

namespace spl2pdf
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream spoolFileStream = new FileStream(@"xx\xxx.SPL", FileMode.Open, FileAccess.Read);
            BinaryReader spoolReader = new BinaryReader(spoolFileStream, Encoding.Unicode);
            EMFSpoolFile spoolFile = new EMFSpoolFile(spoolReader);
            spoolFile.Pages[0].PageImage.Save(@"xx\xxx.png");
            spoolFile.Pages[0].Thumbnail.Save(@"xx\xxx.Thumbnail.png");
        }
    }
}

```


# Learning Sources

[renatosans@PrintJobAccounting](https://github.com/renatosans/PrintJobAccounting)

[Duncan Edwards Jones@EMF Printer Spool File Viewer](https://www.codeproject.com/Articles/10586/EMF-Printer-Spool-File-Viewer-2)
