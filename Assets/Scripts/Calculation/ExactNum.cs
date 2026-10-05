using System;
using System.Globalization;
using System.Numerics;

public readonly struct ExactNum : IComparable<ExactNum>, IEquatable<ExactNum>
{
    public static long MaxBits = 1 << 16;

    public readonly BigInteger Value;

    public ExactNum(BigInteger value) { Value = value; }
    public ExactNum(long value) { Value = new BigInteger(value); }

    public static readonly ExactNum Zero = new ExactNum(BigInteger.Zero);
    public static readonly ExactNum One = new ExactNum(BigInteger.One);

    public bool IsZero => Value.IsZero;
    public int Sign => Value.Sign;

    public static implicit operator ExactNum(long v) => new ExactNum(v);
    public static implicit operator ExactNum(BigInteger v) => new ExactNum(v);

    public static ExactNum Add(ExactNum a, ExactNum b) => new ExactNum(a.Value + b.Value);
    public static ExactNum Sub(ExactNum a, ExactNum b) => new ExactNum(a.Value - b.Value);

    public static ExactNum Mul(ExactNum a, ExactNum b)
    {
        if (BitLen(a.Value) + BitLen(b.Value) > MaxBits) throw new OverflowException("Số Exact vượt MaxBits.");
        return new ExactNum(a.Value * b.Value);
    }

    public static ExactNum Div(ExactNum a, ExactNum b)
    {
        if (b.Value.IsZero) throw new DivideByZeroException();
        return new ExactNum(BigInteger.Divide(a.Value, b.Value));
    }

    public static ExactNum Pow(ExactNum x, int k)
    {
        if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), "Exact chỉ hỗ trợ số mũ nguyên không âm.");
        if (k == 0) return One;
        if (BitLen(x.Value) * k > MaxBits) throw new OverflowException("Số Exact vượt MaxBits.");
        return new ExactNum(BigInteger.Pow(x.Value, k));
    }

    public static ExactNum Negate(ExactNum a) => new ExactNum(-a.Value);

    public static ExactNum operator +(ExactNum a, ExactNum b) => Add(a, b);
    public static ExactNum operator -(ExactNum a, ExactNum b) => Sub(a, b);
    public static ExactNum operator *(ExactNum a, ExactNum b) => Mul(a, b);
    public static ExactNum operator /(ExactNum a, ExactNum b) => Div(a, b);
    public static ExactNum operator -(ExactNum a) => Negate(a);

    public int CompareTo(ExactNum other) => Value.CompareTo(other.Value);
    public bool Equals(ExactNum other) => Value.Equals(other.Value);
    public override bool Equals(object obj) => obj is ExactNum o && Equals(o);
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(ExactNum a, ExactNum b) => a.Value == b.Value;
    public static bool operator !=(ExactNum a, ExactNum b) => a.Value != b.Value;
    public static bool operator <(ExactNum a, ExactNum b) => a.Value < b.Value;
    public static bool operator >(ExactNum a, ExactNum b) => a.Value > b.Value;
    public static bool operator <=(ExactNum a, ExactNum b) => a.Value <= b.Value;
    public static bool operator >=(ExactNum a, ExactNum b) => a.Value >= b.Value;

    public double ToDouble() => (double)Value;

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public static ExactNum Parse(string s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        return new ExactNum(BigInteger.Parse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture));
    }

    private static long BitLen(BigInteger v)
    {
#if NET5_0_OR_GREATER
        return v.GetBitLength();
#else
        return (long)v.ToByteArray().Length * 8;
#endif
    }
}