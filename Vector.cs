public struct Vector(float x, float y, float z)
{
    public readonly float X = x;

    public readonly float Y = y;

    public readonly float Z = z;

    public static bool operator ==(Vector a, Vector b)
    {
        if (a.X == b.X && a.Y == b.Y)
        {
            return a.Z == b.Z;
        }
        return false;
    }

    public static bool operator !=(Vector a, Vector b)
    {
        if (a.X == b.X && a.Y == b.Y)
        {
            return a.Z != b.Z;
        }
        return true;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Vector vector))
        {
            return false;
        }
        if (X == vector.X && Y == vector.Y)
        {
            return Z == vector.Z;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return ((391 + X.GetHashCode()) * 23 + Y.GetHashCode()) * 23 + Z.GetHashCode();
    }

    public override string ToString()
    {
        return "<" + X + ", " + Y + ", " + Z + ">";
    }
}
