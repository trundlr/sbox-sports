namespace Sports;

public struct MinMax<T> where T : unmanaged
{
    public T Min;
    public T Max;

    public MinMax( T min, T max )
    {
        Min = min;
        Max = max;
    }
}
