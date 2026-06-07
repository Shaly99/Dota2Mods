namespace SKYNET.Interop.Windows;

public struct RECT
{
    public int left;

    public int top;

    public int right;

    public int bottom;

    public int Width => right - left;

    public int Height => bottom - top;

    public static implicit operator Rectangle(RECT rect)
    {
        return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
    }

    public static implicit operator RECT(Rectangle rectangle)
    {
        return new RECT
        {
            left = rectangle.Left,
            top = rectangle.Top,
            right = rectangle.Right,
            bottom = rectangle.Bottom
        };
    }
}
