namespace ScaffoldingSample;

public struct EmployeeId
{
    private readonly System.Int32 _value;

    private EmployeeId(System.Int32 value)
    {
        _value = value;
    }

    public static explicit operator EmployeeId(System.Int32 value)
    {
        return new EmployeeId(value);
    }

    public static explicit operator System.Int32(EmployeeId value)
    {
        return value._value;
    }
}