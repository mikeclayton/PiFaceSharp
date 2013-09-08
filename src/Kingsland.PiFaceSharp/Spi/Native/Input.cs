using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Kingsland.PiFaceSharp.Spi.Native
{

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="/usr/include/linux/input.h"/>
    public static class Input
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct input_event
        {
            public uint tv_sec;
            public uint tv_usec;
            public ushort type;
            public ushort code;
            public int value;
        }

        public const uint EV_VERSION = 0x010001;

        [StructLayout(LayoutKind.Sequential)]
        public struct input_id
        {
            public ushort bustype;
            public ushort vendor;
            public ushort product;
            public ushort version;
        };

        public static readonly uint EVIOCGVERSION = IoCtl.IOR('E', 0x01, sizeof(int)); /* get driver version */
        public static readonly uint EVIOCGID = IoCtl.IOR('E', 0x02, 4 * sizeof(short));
        //#define EVIOCGREP		_IOR('E', 0x03, unsigned int[2])	/* get repeat settings */
        //#define EVIOCSREP		_IOW('E', 0x03, unsigned int[2])	/* set repeat settings */

        //#define EVIOCGKEYCODE		_IOR('E', 0x04, unsigned int[2])        /* get keycode */
        //#define EVIOCGKEYCODE_V2	_IOR('E', 0x04, struct input_keymap_entry)
        //#define EVIOCSKEYCODE		_IOW('E', 0x04, unsigned int[2])        /* set keycode */
        //#define EVIOCSKEYCODE_V2	_IOW('E', 0x04, struct input_keymap_entry)

        public static uint EVIOCGNAME(uint len)
        {
            return IoCtl.IOC(IoCtl.IOC_READ, 'E', 0x06, len);		/* get device name */
        }

        //#define EVIOCGPHYS(len)		_IOC(_IOC_READ, 'E', 0x07, len)		/* get physical location */
        //#define EVIOCGUNIQ(len)		_IOC(_IOC_READ, 'E', 0x08, len)		/* get unique identifier */
        //#define EVIOCGPROP(len)		_IOC(_IOC_READ, 'E', 0x09, len)		/* get device properties */

        //#define EVIOCGKEY(len)		_IOC(_IOC_READ, 'E', 0x18, len)		/* get global key state */
        //#define EVIOCGLED(len)		_IOC(_IOC_READ, 'E', 0x19, len)		/* get all LEDs */
        //#define EVIOCGSND(len)		_IOC(_IOC_READ, 'E', 0x1a, len)		/* get all sounds status */
        //#define EVIOCGSW(len)		_IOC(_IOC_READ, 'E', 0x1b, len)		/* get all switch states */

        public static uint EVIOCGBIT(uint ev, uint len)
        {
            return IoCtl.IOC(IoCtl.IOC_READ, 'E', 0x20 + ev, len);	/* get event bits */
        }

        //#define EVIOCGABS(abs)		_IOR('E', 0x40 + (abs), struct input_absinfo)	/* get abs value/limits */
        //#define EVIOCSABS(abs)		_IOW('E', 0xc0 + (abs), struct input_absinfo)	/* set abs value/limits */

        //#define EVIOCSFF		_IOC(_IOC_WRITE, 'E', 0x80, sizeof(struct ff_effect))	/* send a force effect to a force feedback device */
        //#define EVIOCRMFF		_IOW('E', 0x81, int)			/* Erase a force effect */
        //#define EVIOCGEFFECTS		_IOR('E', 0x84, int)			/* Report number of effects playable at the same time */

        //#define EVIOCGRAB		_IOW('E', 0x90, int)			/* Grab/Release device */

        public const byte EV_SYN = 0x00;
        public const byte EV_KEY = 0x01;
        public const byte EV_REL = 0x02;
        public const byte EV_ABS = 0x03;
        public const byte EV_MSC = 0x04;
        public const byte EV_SW = 0x05;
        public const byte EV_LED = 0x11;
        public const byte EV_SND = 0x12;
        public const byte EV_REP = 0x14;
        public const byte EV_FF = 0x15;
        public const byte EV_PWR = 0x16;
        public const byte EV_FF_STATUS = 0x17;
        public const byte EV_MAX = 0x1f;
        public const byte EV_CNT = (byte)(EV_MAX + 1);

        public const ushort BTN_MISC = 0x100;
        public const ushort BTN_0 = 0x100;
        public const ushort BTN_1 = 0x101;
        public const ushort BTN_2 = 0x102;
        public const ushort BTN_3 = 0x103;
        public const ushort BTN_4 = 0x104;
        public const ushort BTN_5 = 0x105;
        public const ushort BTN_6 = 0x106;
        public const ushort BTN_7 = 0x107;
        public const ushort BTN_8 = 0x108;
        public const ushort BTN_9 = 0x109;

        public const ushort BTN_MOUSE = 0x110;
        public const ushort BTN_LEFT = 0x110;
        public const ushort BTN_RIGHT = 0x111;
        public const ushort BTN_MIDDLE = 0x112;
        public const ushort BTN_SIDE = 0x113;
        public const ushort BTN_EXTRA = 0x114;
        public const ushort BTN_FORWARD = 0x115;
        public const ushort BTN_BACK = 0x116;
        public const ushort BTN_TASK = 0x117;

        public const ushort BTN_JOYSTICK = 0x120;
        public const ushort BTN_TRIGGER = 0x120;
        public const ushort BTN_THUMB = 0x121;
        public const ushort BTN_THUMB2 = 0x122;
        public const ushort BTN_TOP = 0x123;
        public const ushort BTN_TOP2 = 0x124;
        public const ushort BTN_PINKIE = 0x125;
        public const ushort BTN_BASE = 0x126;
        public const ushort BTN_BASE2 = 0x127;
        public const ushort BTN_BASE3 = 0x128;
        public const ushort BTN_BASE4 = 0x129;
        public const ushort BTN_BASE5 = 0x12a;
        public const ushort BTN_BASE6 = 0x12b;
        public const ushort BTN_DEAD = 0x12f;

        public const ushort BTN_GAMEPAD = 0x130;
        public const ushort BTN_A = 0x130;
        public const ushort BTN_B = 0x131;
        public const ushort BTN_C = 0x132;
        public const ushort BTN_X = 0x133;
        public const ushort BTN_Y = 0x134;
        public const ushort BTN_Z = 0x135;
        public const ushort BTN_TL = 0x136;
        public const ushort BTN_TR = 0x137;
        public const ushort BTN_TL2 = 0x138;
        public const ushort BTN_TR2 = 0x139;
        public const ushort BTN_SELECT = 0x13a;
        public const ushort BTN_START = 0x13b;
        public const ushort BTN_MODE = 0x13c;
        public const ushort BTN_THUMBL = 0x13d;
        public const ushort BTN_THUMBR = 0x13e;

        public const ushort BTN_DIGI = 0x140;
        public const ushort BTN_TOOL_PEN = 0x140;
        public const ushort BTN_TOOL_RUBBER = 0x141;
        public const ushort BTN_TOOL_BRUSH = 0x142;
        public const ushort BTN_TOOL_PENCIL = 0x143;
        public const ushort BTN_TOOL_AIRBRUSH = 0x144;
        public const ushort BTN_TOOL_FINGER = 0x145;
        public const ushort BTN_TOOL_MOUSE = 0x146;
        public const ushort BTN_TOOL_LENS = 0x147;
        public const ushort BTN_TOOL_QUINTTAP = 0x148;	/* Five fingers on trackpad */
        public const ushort BTN_TOUCH = 0x14a;
        public const ushort BTN_STYLUS = 0x14b;
        public const ushort BTN_STYLUS2 = 0x14c;
        public const ushort BTN_TOOL_DOUBLETAP = 0x14d;
        public const ushort BTN_TOOL_TRIPLETAP = 0x14e;
        public const ushort BTN_TOOL_QUADTAP = 0x14f;	/* Four fingers on trackpad */

        public const ushort BTN_WHEEL = 0x150;
        public const ushort BTN_GEAR_DOWN = 0x150;
        public const ushort BTN_GEAR_UP = 0x151;

        //#define KEY_MIN_INTERESTING	KEY_MUTE
        public const ushort KEY_MAX = 0x2ff;
        //#define KEY_CNT			(KEY_MAX+1)

        public const ushort ABS_X = 0x00;
        public const ushort ABS_Y = 0x01;
        public const ushort ABS_Z = 0x02;
        public const ushort ABS_RX = 0x03;
        public const ushort ABS_RY = 0x04;
        public const ushort ABS_RZ = 0x05;
        public const ushort ABS_THROTTLE = 0x06;
        public const ushort ABS_RUDDER = 0x07;
        public const ushort ABS_WHEEL = 0x08;
        public const ushort ABS_GAS = 0x09;
        public const ushort ABS_BRAKE = 0x0a;
        public const ushort ABS_HAT0X = 0x10;
        public const ushort ABS_HAT0Y = 0x11;
        public const ushort ABS_HAT1X = 0x12;
        public const ushort ABS_HAT1Y = 0x13;
        public const ushort ABS_HAT2X = 0x14;
        public const ushort ABS_HAT2Y = 0x15;
        public const ushort ABS_HAT3X = 0x16;
        public const ushort ABS_HAT3Y = 0x17;
        public const ushort ABS_PRESSURE = 0x18;
        public const ushort ABS_DISTANCE = 0x19;
        public const ushort ABS_TILT_X = 0x1a;
        public const ushort ABS_TILT_Y = 0x1b;
        public const ushort ABS_TOOL_WIDTH = 0x1c;

    }

}
