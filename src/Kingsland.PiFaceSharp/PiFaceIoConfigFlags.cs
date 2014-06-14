using System;

namespace Kingsland.PiFaceSharp
{

    // i/o config
    // from https://github.com/piface/pifacecommon/blob/master/pifacecommon/mcp23s17.py
    [Flags]
    internal enum PiFaceIoConfigFlags : byte
    {

        /// <summary>
        /// Addressing mode
        /// </summary>
        BANK_OFF = 0x00,

        /// <summary>
        /// Addressing mode
        /// </summary>
        BANK_ON = 0x80,

        /// <summary>
        /// Interrupt mirror (INTa|INTb)
        /// </summary>
        INT_MIRROR_ON = 0x40,

        /// <summary>
        /// Interrupt mirror (INTa|INTb)
        /// </summary>
        INT_MIRROR_OFF = 0x00,

        /// <summary>
        /// Incrementing address pointer
        /// </summary>
        SEQOP_OFF = 0x20,

        /// <summary>
        /// Incrementing address pointer
        /// </summary>
        SEQOP_ON = 0x00,

        /// <summary>
        /// Slew rate
        /// </summary>
        DISSLW_ON = 0x10,

        /// <summary>
        /// Slew rate
        /// </summary>
        DISSLW_OFF = 0x00,

        /// <summary>
        /// Hardware addressing
        /// </summary>
        HAEN_ON = 0x08,

        /// <summary>
        /// Hardware addressing
        /// </summary>
        HAEN_OFF = 0x00,

        /// <summary>
        /// Open drain for interupts
        /// </summary>
        ODR_ON = 0x04,

        /// <summary>
        /// Open drain for interupts
        /// </summary>
        ODR_OFF = 0x00,

        /// <summary>
        /// Interrupt polarity
        /// </summary>
        INTPOL_HIGH = 0x02,

        /// <summary>
        /// Interrupt polarity
        /// </summary>
        INTPOL_LOW = 0x00
    
    }

}
