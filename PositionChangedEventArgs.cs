public class PositionChangedEventArgs : EventArgs
{
    public int newPosition { get; }

    public PositionChangedEventArgs(int num)
    {
        newPosition = num;
    }
}
