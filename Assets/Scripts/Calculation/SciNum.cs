using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

public readonly struct SciNum : IComparable<SciNum>, IEquatable<SciNum>
{
    private readonly double _m;
    private readonly long _e;

    private SciNum(double m, long e) { _m = m; _e = e; }

    public double Mantissa => _m;
    public long Exponent => _e;

    public static readonly SciNum Zero = default;
    public static readonly SciNum One = new SciNum(1.0, 0);

    private static readonly double[] P10 =
    {
        1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11,
        1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19, 1e20, 1e21, 1e22
    };
    private static readonly double[] N10 =
    {
        1e0, 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6, 1e-7, 1e-8, 1e-9, 1e-10,
        1e-11, 1e-12, 1e-13, 1e-14, 1e-15, 1e-16
    };
    private const double Log10Of2 = 0.30102999566398119521;
    private const double CancelEps = 1e-15;
    private const double SafeLong = 9.0e18;
    private const long MaxExpandExponent = 100000;

    public static SciNum FromDouble(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v)) throw new ArgumentOutOfRangeException(nameof(v));
        return Norm(v, 0);
    }

    public static SciNum FromMantissaExponent(double m, long e)
    {
        if (double.IsNaN(m) || double.IsInfinity(m)) throw new ArgumentOutOfRangeException(nameof(m));
        return Norm(m, e);
    }

    public static SciNum FromBigInteger(BigInteger v)
    {
        if (v.IsZero) return Zero;
        int sign = v.Sign;
        BigInteger abs = BigInteger.Abs(v);
        long bl = BitLen(abs);
        if (bl <= 63) return Norm((double)v, 0);

        int shift = (int)(bl - 64);
        double top = (double)(ulong)(abs >> shift);
        double lg = Math.Log10(top) + shift * Log10Of2;
        double fl = Math.Floor(lg);
        double mant = Math.Pow(10.0, lg - fl);
        long e = (long)fl;
        if (mant >= 10.0) { mant /= 10.0; e++; }
        return new SciNum(sign * mant, e);
    }

    public static implicit operator SciNum(double v) => FromDouble(v);
    public static implicit operator SciNum(long v) => FromDouble(v);

    public bool IsZero => _m == 0;
    public int Sign => _m > 0 ? 1 : _m < 0 ? -1 : 0;

    public static SciNum Add(SciNum a, SciNum b)
    {
        double am = a._m, bm = b._m;
        long ae = a._e, be = b._e;
        if (am == 0) return b;
        if (bm == 0) return a;
        if (ae < be) { double tm = am; am = bm; bm = tm; long te = ae; ae = be; be = te; }

        long d = ae - be;
        if (d > 16) return new SciNum(am, ae);

        double s = am + bm * N10[(int)d];
        if (Math.Abs(s) < Math.Abs(am) * CancelEps) return Zero;
        return Norm(s, ae);
    }

    public static SciNum Sub(SciNum a, SciNum b) => Add(a, Negate(b));

    public static SciNum Mul(SciNum a, SciNum b)
    {
        double m = a._m * b._m;
        if (m == 0) return Zero;
        long e = a._e + b._e;
        return Math.Abs(m) >= 10.0 ? new SciNum(m / 10.0, e + 1) : new SciNum(m, e);
    }

    public static SciNum Div(SciNum a, SciNum b)
    {
        if (b._m == 0) throw new DivideByZeroException();
        if (a._m == 0) return Zero;
        double m = a._m / b._m;
        long e = a._e - b._e;
        return Math.Abs(m) < 1.0 ? new SciNum(m * 10.0, e - 1) : new SciNum(m, e);
    }

    public static SciNum Pow(SciNum x, double n)
    {
        if (double.IsNaN(n) || double.IsInfinity(n)) throw new ArgumentOutOfRangeException(nameof(n));
        if (n == 0) return One;
        if (x._m == 0)
        {
            if (n < 0) throw new DivideByZeroException();
            return Zero;
        }

        bool neg = false;
        if (x._m < 0)
        {
            if (n != Math.Floor(n)) throw new ArgumentException("Cơ số âm chỉ hỗ trợ số mũ nguyên.");
            neg = Math.Abs(n) < 9007199254740992.0 && (((long)n) & 1) != 0;
        }

        double m = Math.Abs(x._m);
        double t = (double)x._e * n;
        if (!(Math.Abs(t) < SafeLong)) throw new OverflowException("Số mũ kết quả vượt giới hạn.");
        long ti = (long)t;
        double r = t - ti;

        if (r == 0)
        {
            double pm = Math.Pow(m, n);
            if (pm > 1e-300 && pm < 1e300) return Norm(neg ? -pm : pm, ti);
        }

        double lg = n * Math.Log10(m) + r;
        double fl = Math.Floor(lg);
        double total = (double)ti + fl;
        if (!(Math.Abs(total) < SafeLong)) throw new OverflowException("Số mũ kết quả vượt giới hạn.");
        double mant = Math.Pow(10.0, lg - fl);
        long e = ti + (long)fl;
        if (mant >= 10.0) { mant /= 10.0; e++; }
        return new SciNum(neg ? -mant : mant, e);
    }

    public static SciNum Negate(SciNum a) => a._m == 0 ? a : new SciNum(-a._m, a._e);

    public static SciNum operator +(SciNum a, SciNum b) => Add(a, b);
    public static SciNum operator -(SciNum a, SciNum b) => Sub(a, b);
    public static SciNum operator *(SciNum a, SciNum b) => Mul(a, b);
    public static SciNum operator /(SciNum a, SciNum b) => Div(a, b);
    public static SciNum operator -(SciNum a) => Negate(a);

    public int CompareTo(SciNum other)
    {
        int sa = Sign, sb = other.Sign;
        if (sa != sb) return sa < sb ? -1 : 1;
        if (sa == 0) return 0;
        if (_e != other._e)
        {
            int c = _e < other._e ? -1 : 1;
            return sa > 0 ? c : -c;
        }
        return _m.CompareTo(other._m);
    }

    public bool Equals(SciNum other) => CompareTo(other) == 0;
    public override bool Equals(object obj) => obj is SciNum o && Equals(o);
    public override int GetHashCode() => unchecked(_m.GetHashCode() * 31 + _e.GetHashCode());

    public static bool operator ==(SciNum a, SciNum b) => a.CompareTo(b) == 0;
    public static bool operator !=(SciNum a, SciNum b) => a.CompareTo(b) != 0;
    public static bool operator <(SciNum a, SciNum b) => a.CompareTo(b) < 0;
    public static bool operator >(SciNum a, SciNum b) => a.CompareTo(b) > 0;
    public static bool operator <=(SciNum a, SciNum b) => a.CompareTo(b) <= 0;
    public static bool operator >=(SciNum a, SciNum b) => a.CompareTo(b) >= 0;

    public double ToDouble()
    {
        if (_m == 0 || _e < -323) return 0.0;
        if (_e > 308) return _m > 0 ? double.PositiveInfinity : double.NegativeInfinity;
        return _m * Math.Pow(10.0, _e);
    }

    public BigInteger ToBigInteger()
    {
        if (_m == 0 || _e < 0) return BigInteger.Zero;
        BigInteger mi = new BigInteger(Math.Round(_m * 1e15));
        long sh = _e - 15;
        if (sh >= 0)
        {
            if (sh > MaxExpandExponent) throw new OverflowException("Số mũ quá lớn để đổi sang Exact.");
            return mi * BigInteger.Pow(10, (int)sh);
        }
        return mi / BigInteger.Pow(10, (int)(-sh));
    }

    public override string ToString() => _m == 0 ? "0e0" : _m.ToString("R", CultureInfo.InvariantCulture) + "e" + _e.ToString(CultureInfo.InvariantCulture);

    public static SciNum Parse(string s)
    {
        if (s == null) throw new ArgumentNullException(nameof(s));
        int i = s.IndexOf('e');
        if (i < 0) i = s.IndexOf('E');
        if (i < 0) return FromDouble(double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture));

        double m = double.Parse(s.Substring(0, i), NumberStyles.Float, CultureInfo.InvariantCulture);
        long e = long.Parse(s.Substring(i + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
        return FromMantissaExponent(m, e);
    }

    private static SciNum Norm(double m, long e)
    {
        double a = Math.Abs(m);
        if (a >= 1.0)
        {
            if (a < 10.0) return new SciNum(m, e);
            if (a < 100.0) return new SciNum(m / 10.0, e + 1);
        }
        else if (a >= 0.1) return new SciNum(m * 10.0, e - 1);
        else if (a == 0.0) return Zero;
        return NormSlow(m, e);
    }

    private static SciNum NormSlow(double m, long e)
    {
        double a = Math.Abs(m);
        if (a < 1e-300) { m *= 1e300; e -= 300; a = Math.Abs(m); }
        int k = (int)Math.Floor(Math.Log10(a));
        m = k >= 0 ? m / Pow10(k) : m * Pow10(-k);
        a = Math.Abs(m);
        if (a >= 10.0) { m /= 10.0; k++; }
        else if (a < 1.0) { m *= 10.0; k--; }
        return new SciNum(m, e + k);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double Pow10(int k) => (k >= 0 && k <= 22) ? P10[k] : Math.Pow(10.0, k);

    private static long BitLen(BigInteger v)
    {
#if NET5_0_OR_GREATER
        return v.GetBitLength();
#else
        return (long)v.ToByteArray().Length * 8;
#endif
    }
}