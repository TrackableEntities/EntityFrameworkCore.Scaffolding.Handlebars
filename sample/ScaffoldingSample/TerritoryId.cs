namespace ScaffoldingSample;

public struct TerritoryId
{
    private readonly System.String _value;

    private TerritoryId(System.String value)
    {
        _value = value;
    }

    public static explicit operator TerritoryId(System.String value)
    {
        return new TerritoryId(value);
    }

    public static explicit operator System.String(TerritoryId value)
    {
        return value._value;
    }
}