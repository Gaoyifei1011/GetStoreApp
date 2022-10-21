namespace GetStoreApp.Extensions.DataType.Enum
{
    public enum SICHINTF : uint
    {
        SICHINT_DISPLAY = 0x00000000,
        SICHINT_ALLFIELDS = 0x80000000,
        SICHINT_CANONICAL = 0x10000000,
        SICHINT_TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
    }
}
