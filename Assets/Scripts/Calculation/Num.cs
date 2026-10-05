using System;
using System.Globalization;

public static class NumHub
{
    public static bool UseExact { get; private set; } = true;
    public static bool DemoteOnOverflow = false;
    public static event Action<bool> ModeChanged;

    public static void SetUseExact(bool useExact)
    {
        if (UseExact == useExact) return;
        UseExact = useExact;
        ModeChanged?.Invoke(useExact);
    }

    public static void Toggle() => SetUseExact(!UseExact);

    public static long MaxExactBits
    {
        get => ExactNum.MaxBits;
        set => ExactNum.MaxBits = value;
    }
}

public readonly struct Num : IComparable<Num>, IEquatable<Num>
{
    private readonly bool _exact;
    private readonly ExactNum _x;
    private readonly SciNum _s;

    private Num(ExactNum x) { _exact = true; _x = x; _s = default; }
    private Num(SciNum s) { _exact = false; _x = default; _s = s; }

    public bool IsExact => _exact;
    public bool IsZero => _exact ? _x.IsZero : _s.IsZero;
    public int Sign => _exact ? _x.Sign : _s.Sign;

    public static Num From(long v) => NumHub.UseExact ? new Num(new ExactNum(v)) : new Num(SciNum.FromDouble(v));

    public static Num From(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v)) throw new ArgumentOutOfRangeException(nameof(v));
        return NumHub.UseExact ? new Num(new ExactNum(new System.Numerics.BigInteger(v))) : new Num(SciNum.FromDouble(v));
    }

    public static Num FromMantissaExponent(double m, long e) => NumHub.UseExact ? new Num(new ExactNum(SciNum.FromMantissaExponent(m, e).ToBigInteger())) : new Num(SciNum.FromMantissaExponent(m, e));

    public static Num Parse(string s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        bool sci = s.IndexOf('e') >= 0 || s.IndexOf('E') >= 0;
        Num n = sci ? new Num(SciNum.Parse(s)) : new Num(ExactNum.Parse(s));
        return n.ConvertTo(NumHub.UseExact);
    }

    public static implicit operator Num(long v) => From(v);
    public static implicit operator Num(double v) => From(v);

    public Num ConvertTo(bool exact)
    {
        if (_exact == exact) return this;
        return exact ? new Num(new ExactNum(_s.ToBigInteger())) : new Num(SciNum.FromBigInteger(_x.Value));
    }

    public Num ToCurrentMode() => ConvertTo(NumHub.UseExact);
    public ExactNum AsExact() => _exact ? _x : new ExactNum(_s.ToBigInteger());
    public SciNum AsSci() => _exact ? SciNum.FromBigInteger(_x.Value) : _s;

    private static void Unify(ref Num a, ref Num b)
    {
        if (a._exact == b._exact) return;
        bool target = NumHub.UseExact;
        a = a.ConvertTo(target);
        b = b.ConvertTo(target);
    }

    public static Num Add(Num a, Num b)
    {
        Unify(ref a, ref b);
        return a._exact ? new Num(a._x + b._x) : new Num(a._s + b._s);
    }

    public static Num Sub(Num a, Num b)
    {
        Unify(ref a, ref b);
        return a._exact ? new Num(a._x - b._x) : new Num(a._s - b._s);
    }

    public static Num Mul(Num a, Num b)
    {
        Unify(ref a, ref b);
        if (!a._exact) return new Num(a._s * b._s);
        try { return new Num(a._x * b._x); }
        catch (OverflowException) when (NumHub.DemoteOnOverflow)
        {
            return new Num(SciNum.FromBigInteger(a._x.Value) * SciNum.FromBigInteger(b._x.Value));
        }
    }

    public static Num Div(Num a, Num b)
    {
        Unify(ref a, ref b);
        return a._exact ? new Num(a._x / b._x) : new Num(a._s / b._s);
    }

    public static Num Pow(Num x, double n)
    {
        if (!x._exact) return new Num(SciNum.Pow(x._s, n));

        if (n < 0 || n > int.MaxValue || n != Math.Floor(n)) throw new ArgumentException("Chế độ Exact chỉ hỗ trợ số mũ nguyên không âm; hãy đổi sang SciNum.");
        try { return new Num(ExactNum.Pow(x._x, (int)n)); }
        catch (OverflowException) when (NumHub.DemoteOnOverflow)
        {
            return new Num(SciNum.Pow(SciNum.FromBigInteger(x._x.Value), n));
        }
    }

    public static Num Negate(Num a) => a._exact ? new Num(-a._x) : new Num(-a._s);

    public static Num operator +(Num a, Num b) => Add(a, b);
    public static Num operator -(Num a, Num b) => Sub(a, b);
    public static Num operator *(Num a, Num b) => Mul(a, b);
    public static Num operator /(Num a, Num b) => Div(a, b);
    public static Num operator -(Num a) => Negate(a);

    public int CompareTo(Num other)
    {
        Num a = this, b = other;
        Unify(ref a, ref b);
        return a._exact ? a._x.CompareTo(b._x) : a._s.CompareTo(b._s);
    }

    public bool Equals(Num other) => CompareTo(other) == 0;
    public override bool Equals(object obj) => obj is Num n && Equals(n);
    public override int GetHashCode() => AsSci().GetHashCode();

    public static bool operator ==(Num a, Num b) => a.CompareTo(b) == 0;
    public static bool operator !=(Num a, Num b) => a.CompareTo(b) != 0;
    public static bool operator <(Num a, Num b) => a.CompareTo(b) < 0;
    public static bool operator >(Num a, Num b) => a.CompareTo(b) > 0;
    public static bool operator <=(Num a, Num b) => a.CompareTo(b) <= 0;
    public static bool operator >=(Num a, Num b) => a.CompareTo(b) >= 0;

    public double ToDouble() => _exact ? _x.ToDouble() : _s.ToDouble();
    public override string ToString() => _exact ? _x.ToString() : _s.ToString();
}