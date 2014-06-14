namespace Kingsland.PiFaceSharp
{

    // register addresses
    // see https://github.com/piface/pifacecommon/blob/master/pifacecommon/mcp23s17.py
    internal enum PiFaceRegisterAddress : byte
    {

        /// <summary>
        /// I/O direction A
        /// </summary>
        IODIRA = 0x00,

        /// <summary>
        /// I/O direction B
        /// </summary>
        IODIRB = 0x01,

        /// <summary>
        /// I/O polarity A
        /// </summary>
        IPOLA = 0x02,

        /// <summary>
        /// I/O polarity B
        /// </summary>
        IPOLB = 0x03,

        /// <summary>
        /// Interrupt enable A
        /// </summary>
        GPINTENA = 0x04,

        /// <summary>
        /// Interrupt enable B
        /// </summary>
        GPINTENB = 0x05,

        /// <summary>
        /// Register default value A (interrupts)
        /// </summary>
        DEFVALA = 0x06,

        /// <summary>
        /// Register default value B (interrupts)
        /// </summary>
        DEFVALB = 0x07,

        /// <summary>
        /// Interrupt control A
        /// </summary>
        INTCONA = 0x08,

        /// <summary>
        /// Interrupt control B
        /// </summary>
        INTCONB = 0x09,

        /// <summary>
        /// I/O config (also 0xB)
        /// </summary>
        IOCON = 0x0A,

        /// <summary>
        /// Port A pullups
        /// </summary>
        GPPUA = 0x0C,

        /// <summary>
        /// Port B pullups
        /// </summary>
        GPPUB = 0x0D,

        /// <summary>
        /// Interrupt flag A (where the interrupt came from)
        /// </summary>
        INTFA = 0x0E,

        /// <summary>
        /// Interrupt flag B
        /// </summary>
        INTFB = 0x0F,

        /// <summary>
        /// Interrupt capture A (value at interrupt is saved here)
        /// </summary>
        INTCAPA = 0x10,

        /// <summary>
        /// Interrupt capture B
        /// </summary>
        INTCAPB = 0x11,

        /// <summary>
        /// Port A
        /// </summary>
        GPIOA = 0x12,

        /// <summary>
        /// Port B
        /// </summary>
        GPIOB = 0x13,

        /// <summary>
        /// Output latch A
        /// </summary>
        OLATA = 0x14,

        /// <summary>
        /// Output latch B
        /// </summary>
        OLATB = 0x15

    }

}
