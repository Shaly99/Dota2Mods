using System.Runtime.InteropServices;

namespace SKYNET.Interop.Windows;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ERROR
{
    public const int SUCCESS = 0;

    public const int FILE_NOT_FOUND = 2;

    public const int NO_MORE_FILES = 18;

    public const int INSUFFICIENT_BUFFER = 122;

    public const int PROC_NOT_FOUND = 127;

    public const int ALREADY_EXISTS = 183;

    public const int MORE_DATA = 234;

    public const int BADDB = 1009;

    public const int BADKEY = 1010;

    public const int CANTOPEN = 1011;

    public const int CANTREAD = 1012;

    public const int CANTWRITE = 1013;

    public const int REGISTRY_RECOVERED = 1014;

    public const int REGISTRY_CORRUPT = 1015;

    public const int REGISTRY_IO_FAILED = 1016;

    public const int NOT_REGISTRY_FILE = 1017;

    public const int KEY_DELETED = 1018;

    public const int NO_LOG_SPACE = 1019;

    public const int KEY_HAS_CHILDREN = 1020;

    public const int CHILD_MUST_BE_VOLATILE = 1021;

    public const int NOTIFY_ENUM_DIR = 1022;

    public const int NO_ASSOCIATION = 1155;

    public const int UNABLE_TO_REMOVE_REPLACED = 1175;

    public const int UNABLE_TO_MOVE_REPLACEMENT = 1176;

    public const int UNABLE_TO_MOVE_REPLACEMENT_2 = 1177;

    public const int BAD_DEVICE = 1200;

    public const int NO_NET_OR_BAD_PATH = 1203;

    public const int HOTKEY_ALREADY_REGISTERED = 1409;

    public const int CONNECTION_RESET = 10054;

    public const int CONNECTION_REFUSED = 10061;

    public const int NOT_CONNECTED = 2250;
}
