namespace EMFSpool
{
    public enum SPLRecordTypeEnum
    {
        SRT_EOF           = 0x00,         // int32 zero
        SRT_RESERVED_1    = 0x01,         // 1
        SRT_FONTDATA      = 0x02,         // 2 Font Data
        SRT_DEVMODE       = 0x03,         // 3 DevMode
        SRT_FONT2         = 0x04,         // 4 Font Data
        SRT_RESERVED_5    = 0x05,         // 5 
        SRT_FONT_MM       = 0x06,         // 6 Font Data (Multiple Master)
        SRT_FONT_SUB1     = 0x07,         // 7 Font Data (SubsetFont 1)
        SRT_FONT_SUB2     = 0x08,         // 8 Font Data (SubsetFont 2)
        SRT_RESERVED_9    = 0x09,         // 9
        SRT_RESERVED_A    = 0x0A,         // 10
        SRT_RESERVED_B    = 0x0B,         // 11
        SRT_PAGE          = 0x0C,         // 12  Enhanced Meta File (EMF)
        SRT_EOPAGE1       = 0x0D,         // 13  EndOfPage
        SRT_EOPAGE2       = 0x0E,         // 14  EndOfPage
        SRT_EXT_FONT      = 0x0F,         // 15  Ext Font Data
        SRT_EXT_FONT2     = 0x10,         // 16  Ext Font Data
        SRT_EXT_FONT_MM   = 0x11,         // 17  Ext Font Data (Multiple Master)
        SRT_EXT_FONT_SUB1 = 0x12,         // 18  Ext Font Data (SubsetFont 1)
        SRT_EXT_FONT_SUB2 = 0x13,         // 19  Ext Font Data (SubsetFont 2)
        SRT_EXT_PAGE      = 0x14,         // 20  
        SRT_JOB_INFO      = 0x10000       // int length, wchar jobDescription
    }

}
