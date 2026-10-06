using System;
using System.Diagnostics;
using System.Globalization;

public readonly struct BigInt : IComparable<BigInt>, IEquatable<BigInt>
{
    private readonly int _sign;
    private readonly uint[] _mag;

    private BigInt(int sign, uint[] mag)
    {
        if (mag.Length == 0) 
        { 
            _sign = 0; 
            _mag = null; 
        }
        else 
        { 
            _sign = sign; 
            _mag = mag; 
        }
    }

    public BigInt(long value)
    {
        if (value == 0) 
        { 
            _sign = 0; 
            _mag = null; 
            return; 
        }

        _sign = value < 0 ? -1 : 1;
        ulong m = value < 0 ? (ulong) (-(value + 1)) + 1UL : (ulong) value; // absolute value
        uint lo = (uint)(m & 0xFFFFFFFFUL);
        uint hi = (uint)(m >> 32);
        _mag = hi != 0 ? new[] { lo, hi } : new[] { lo };
    }

    public static BigInt FromUInt64(ulong value)
    {
        if (value == 0) return Zero;
        uint lo = (uint)(value & 0xFFFFFFFFUL);
        uint hi = (uint)(value >> 32);
        return new BigInt(1, hi != 0 ? new[] { lo, hi } : new[] { lo });
    }

    public static readonly BigInt Zero = default;
    public static readonly BigInt One = new BigInt(1L);
    public static readonly BigInt MinusOne = new BigInt(-1L);

    public int Sign => _sign;
    public bool IsZero => _sign == 0;
    public bool IsOne => _sign == 1 && _mag.Length == 1 && _mag[0] == 1;
    public bool IsEven => _sign == 0 || (_mag[0] & 1u) == 0;

    public long BitLength => _sign == 0 ? 0 : BigMag.BitLength(_mag);

    public int LimbCount => _sign == 0 ? 0 : _mag.Length;

    public static BigInt Negate(BigInt a) => a._sign == 0 ? a : new BigInt(-a._sign, a._mag);
    public static BigInt Abs(BigInt a) => a._sign < 0 ? new BigInt(1, a._mag) : a;

    public static BigInt Add(BigInt a, BigInt b)
    {
        if (a._sign == 0) return b;
        if (b._sign == 0) return a;
        if (a._sign == b._sign) return new BigInt(a._sign, BigMag.Add(a._mag, b._mag));

        int c = BigMag.Compare(a._mag, b._mag);
        if (c == 0) return Zero;
        return c > 0 ? new BigInt(a._sign, BigMag.Sub(a._mag, b._mag)) : new BigInt(b._sign, BigMag.Sub(b._mag, a._mag));
    }

    public static BigInt Sub(BigInt a, BigInt b) => Add(a, Negate(b));

    public static BigInt Mul(BigInt a, BigInt b)
    {
        if (a._sign == 0 || b._sign == 0) return Zero;
        return new BigInt(a._sign * b._sign, BigMag.Mul(a._mag, b._mag));
    }

    public static BigInt DivRem(BigInt a, BigInt b, out BigInt remainder)
    {
        if (b._sign == 0) throw new DivideByZeroException();
        if (a._sign == 0) 
        { 
            remainder = Zero; 
            return Zero; 
        }
        
        uint[] q, r;
        BigMag.DivRem(a._mag, b._mag, out q, out r);
        remainder = new BigInt(a._sign, r);
        return new BigInt(a._sign * b._sign, q);
    }

    public static BigInt Div(BigInt a, BigInt b) 
    { 
        BigInt r; 
        return DivRem(a, b, out r); 
    }

    public static BigInt Rem(BigInt a, BigInt b) 
    { 
        BigInt r; 
        DivRem(a, b, out r); 
        return r; 
    }

    public static BigInt Pow(BigInt x, int k)
    {
        if (k < 0) throw new ArgumentOutOfRangeException(nameof(k), "k < 0.");
        BigInt result = One, b = x;
        while (k > 0)
        {
            if ((k & 1) != 0) result = Mul(result, b);
            k >>= 1;
            if (k > 0) b = Mul(b, b);
        }
        return result;
    }

    public static BigInt ShiftLeft(BigInt a, int bits)
    {
        if (bits == int.MinValue) throw new ArgumentOutOfRangeException(nameof(bits));
        if (bits < 0) return ShiftRight(a, -bits);
        if (a._sign == 0 || bits == 0) return a;
        return new BigInt(a._sign, BigMag.ShiftLeft(a._mag, bits));
    }

    public static BigInt ShiftRight(BigInt a, int bits)
    {
        if (bits == int.MinValue) throw new ArgumentOutOfRangeException(nameof(bits));
        if (bits < 0) return ShiftLeft(a, -bits);
        if (a._sign == 0 || bits == 0) return a;
        uint[] r = BigMag.ShiftRight(a._mag, bits);
        if (a._sign < 0 && BigMag.AnyLowBits(a._mag, bits)) r = BigMag.Add(r, BigMag.OneMag);
        return new BigInt(a._sign, r);
    }

    public static BigInt operator +(BigInt a, BigInt b) => Add(a, b);
    public static BigInt operator -(BigInt a, BigInt b) => Sub(a, b);
    public static BigInt operator *(BigInt a, BigInt b) => Mul(a, b);
    public static BigInt operator /(BigInt a, BigInt b) => Div(a, b);
    public static BigInt operator %(BigInt a, BigInt b) => Rem(a, b);
    public static BigInt operator -(BigInt a) => Negate(a);
    public static BigInt operator <<(BigInt a, int n) => ShiftLeft(a, n);
    public static BigInt operator >>(BigInt a, int n) => ShiftRight(a, n);

    public static implicit operator BigInt(long v) => new BigInt(v);
    public static explicit operator long(BigInt v)
    {
        long r;
        if (!v.TryToInt64(out r)) throw new OverflowException("Overflow");
        return r;
    }
    public static explicit operator double(BigInt v) => v.ToDouble();

    public int CompareTo(BigInt other)
    {
        if (_sign != other._sign) return _sign < other._sign ? -1 : 1;
        if (_sign == 0) return 0;
        int c = BigMag.Compare(_mag, other._mag);
        return _sign > 0 ? c : -c;
    }

    public bool Equals(BigInt other) => CompareTo(other) == 0;
    public override bool Equals(object obj) => obj is BigInt b && Equals(b);

    public override int GetHashCode()
    {
        unchecked
        {
            int h = _sign;
            if (_mag != null) for (int i = 0; i < _mag.Length; i++) h = h * 31 + (int)_mag[i];
            return h;
        }
    }

    public static bool operator ==(BigInt a, BigInt b) => a.CompareTo(b) == 0;
    public static bool operator !=(BigInt a, BigInt b) => a.CompareTo(b) != 0;
    public static bool operator <(BigInt a, BigInt b) => a.CompareTo(b) < 0;
    public static bool operator >(BigInt a, BigInt b) => a.CompareTo(b) > 0;
    public static bool operator <=(BigInt a, BigInt b) => a.CompareTo(b) <= 0;
    public static bool operator >=(BigInt a, BigInt b) => a.CompareTo(b) >= 0;
// chưa hiểu
    public bool TryToInt64(out long value)
    {
        value = 0;
        if (_sign == 0) return true;
        if (_mag.Length > 2) return false;
        ulong m = _mag[0];
        if (_mag.Length == 2) m |= (ulong)_mag[1] << 32;
        if (_sign > 0)
        {
            if (m > (ulong)long.MaxValue) return false;
            value = (long)m;
            return true;
        }
        if (m > 9223372036854775808UL) return false;
        value = m == 9223372036854775808UL ? long.MinValue : -(long)m;
        return true;
    }

    public double ToDouble()
    {
        if (_sign == 0) return 0.0;
        if (BigMag.BitLength(_mag) > 1024) return _sign > 0 ? double.PositiveInfinity : double.NegativeInfinity;
        int shift; 
        bool sticky;
        ulong top = BigMag.Top64(_mag, out shift, out sticky);
        if (sticky) top |= 1UL;
        double d = (double)top;
        if (shift > 0) d *= Math.Pow(2.0, shift);
        return _sign > 0 ? d : -d;
    }

    public static BigInt FromDouble(double d)
    {
        if (double.IsNaN(d) || double.IsInfinity(d)) throw new OverflowException();
        d = Math.Truncate(d);
        if (d == 0) return Zero;
        long bits = BitConverter.DoubleToInt64Bits(d);
        bool neg = bits < 0;
        int exp = (int)((bits >> 52) & 0x7FF);
        long mant = (bits & 0xFFFFFFFFFFFFFL) | (1L << 52);
        int e = exp - 1075;
        BigInt r = new BigInt(mant);
        r = e >= 0 ? ShiftLeft(r, e) : ShiftRight(r, -e);
        return neg ? Negate(r) : r;
    }

    public ulong TopBits64(out int shift)
    {
        bool sticky;
        if (_sign == 0) { shift = 0; return 0UL; }
        return BigMag.Top64(_mag, out shift, out sticky);
    }

    public override string ToString()
    {
        if (_sign == 0) return "0";
        uint[] work = (uint[])_mag.Clone();
        int len = work.Length;
        uint[] chunks = new uint[len * 11 / 10 + 2];
        int count = 0;
        while (len > 0) chunks[count++] = BigMag.DivSmallInPlace(work, ref len, 1000000000u);

        char[] buf = new char[count * 9 + 1];
        int pos = buf.Length;
        for (int k = 0; k < count; k++)
        {
            uint v = chunks[k];
            if (k < count - 1) for (int d = 0; d < 9; d++) { buf[--pos] = (char)('0' + (int)(v % 10)); v /= 10; }
            else do { buf[--pos] = (char)('0' + (int)(v % 10)); v /= 10; } while (v != 0);
        }
        if (_sign < 0) buf[--pos] = '-';
        return new string(buf, pos, buf.Length - pos);
    }

    public static bool TryParse(string s, out BigInt result)
    {
        result = Zero;
        if (string.IsNullOrEmpty(s)) return false;
        int pos = 0, sign = 1;
        if (s[0] == '-') { sign = -1; pos = 1; }
        else if (s[0] == '+') pos = 1;

        int digits = s.Length - pos;
        if (digits <= 0) return false;
        for (int i = pos; i < s.Length; i++) if (s[i] < '0' || s[i] > '9') return false;

        int chunks = (digits + 8) / 9;
        uint[] work = new uint[chunks + 1];
        int len = 0;
        int first = digits - (chunks - 1) * 9;
        int p = pos;
        for (int c = 0; c < chunks; c++)
        {
            int n = c == 0 ? first : 9;
            uint v = 0;
            for (int k = 0; k < n; k++) v = v * 10 + (uint)(s[p++] - '0');
            BigMag.MulAddSmallInPlace(work, ref len, c == 0 ? 1u : 1000000000u, v);
        }
        uint[] mag = new uint[len];
        Array.Copy(work, mag, len);
        result = new BigInt(sign, mag);
        return true;
    }

    public static BigInt Parse(string s)
    {
        BigInt r;
        if (!TryParse(s, out r)) throw new FormatException("Chuỗi không phải số nguyên thập phân hợp lệ.");
        return r;
    }
}

internal static class BigMag
{
    private const ulong Mask = 0xFFFFFFFFUL;
    private const long MaskL = 0xFFFFFFFFL;

    public static readonly uint[] Empty = new uint[0];
    public static readonly uint[] OneMag = { 1u };

    public static int KaratsubaThreshold = 40;

    public static int Clz(uint x)
    {
        if (x == 0) return 32;
        int n = 0;
        if ((x & 0xFFFF0000u) == 0) 
        { 
            n += 16; 
            x <<= 16; 
        }

        if ((x & 0xFF000000u) == 0) 
        { 
            n += 8; 
            x <<= 8; 
        }

        if ((x & 0xF0000000u) == 0) 
        { 
            n += 4; 
            x <<= 4; 
        }

        if ((x & 0xC0000000u) == 0) 
        { 
            n += 2; 
            x <<= 2; 
        }

        if ((x & 0x80000000u) == 0) n += 1;
        return n;
    }

    public static int BitLength(uint[] a) => a.Length == 0 ? 0 : (a.Length - 1) * 32 + (32 - Clz(a[a.Length - 1]));

    public static uint[] Trim(uint[] r)
    {
        int n = r.Length;
        while (n > 0 && r[n - 1] == 0) n--;
        if (n == r.Length) return r;
        if (n == 0) return Empty;
        uint[] t = new uint[n];
        Array.Copy(r, t, n);
        return t;
    }

    public static int Compare(uint[] a, uint[] b)
    {
        if (a.Length != b.Length) return a.Length < b.Length ? -1 : 1;
        for (int i = a.Length - 1; i >= 0; i--) if (a[i] != b[i]) return a[i] < b[i] ? -1 : 1;
        return 0;
    }

    public static uint[] Add(uint[] a, uint[] b)
    {
        if (a.Length < b.Length) { uint[] t = a; a = b; b = t; }
        if (b.Length == 0) return a;

        bool mayCarry = b.Length < a.Length ? a[a.Length - 1] == uint.MaxValue : (ulong)a[a.Length - 1] + b[b.Length - 1] >= uint.MaxValue;

        uint[] r = new uint[mayCarry ? a.Length + 1 : a.Length];
        ulong carry = 0;
        int i = 0;
        for (; i < b.Length; i++)
        {
            ulong s = (ulong)a[i] + b[i] + carry;
            r[i] = (uint)(s & Mask);
            carry = s >> 32;
        }
        for (; i < a.Length; i++)
        {
            ulong s = (ulong)a[i] + carry;
            r[i] = (uint)(s & Mask);
            carry = s >> 32;
        }
        if (mayCarry) { r[i] = (uint)carry; return Trim(r); }
        Debug.Assert(carry == 0);
        return r;
    }

    public static uint[] Sub(uint[] a, uint[] b)
    {
        if (b.Length == 0) return a;
        uint[] r = new uint[a.Length];
        long borrow = 0;
        int i = 0;
        for (; i < b.Length; i++)
        {
            long d = (long)a[i] - b[i] - borrow;
            if (d < 0) { d += 0x100000000L; borrow = 1; } else borrow = 0;
            r[i] = (uint)d;
        }
        for (; i < a.Length; i++)
        {
            long d = (long)a[i] - borrow;
            if (d < 0) { d += 0x100000000L; borrow = 1; } else borrow = 0;
            r[i] = (uint)d;
        }
        Debug.Assert(borrow == 0, "Sub yêu cầu a >= b");
        return Trim(r);
    }

    public static uint[] Mul(uint[] a, uint[] b)
    {
        if (a.Length == 0 || b.Length == 0) return Empty;
        if (a.Length < b.Length) { uint[] t = a; a = b; b = t; }
        if (b.Length < KaratsubaThreshold) return MulSchool(a, b);
        return MulKaratsuba(a, b);
    }

    public static uint[] MulSchool(uint[] a, uint[] b)
    {
        uint[] r = new uint[a.Length + b.Length];
        for (int i = 0; i < a.Length; i++)
        {
            ulong ai = a[i];
            if (ai == 0) continue;
            ulong carry = 0;
            for (int j = 0; j < b.Length; j++)
            {
                ulong cur = r[i + j] + ai * b[j] + carry;
                r[i + j] = (uint)(cur & Mask);
                carry = cur >> 32;
            }
            r[i + b.Length] = (uint)carry;
        }
        return Trim(r);
    }

    private static uint[] MulKaratsuba(uint[] a, uint[] b)
    {
        int half = (a.Length + 1) >> 1;
        uint[] a0 = Low(a, half), a1 = High(a, half);
        uint[] r = new uint[a.Length + b.Length];

        if (b.Length <= half)
        {
            AddAt(r, 0, Mul(a0, b));
            AddAt(r, half, Mul(a1, b));
        }
        else
        {
            uint[] b0 = Low(b, half), b1 = High(b, half);
            uint[] z0 = Mul(a0, b0);
            uint[] z2 = Mul(a1, b1);
            uint[] z1 = Sub(Sub(Mul(Add(a0, a1), Add(b0, b1)), z0), z2);
            AddAt(r, 0, z0);
            AddAt(r, half, z1);
            AddAt(r, 2 * half, z2);
        }
        return Trim(r);
    }

    private static uint[] Low(uint[] a, int n)
    {
        if (a.Length <= n) return a;
        uint[] t = new uint[n];
        Array.Copy(a, t, n);
        return Trim(t);
    }

    private static uint[] High(uint[] a, int n)
    {
        if (a.Length <= n) return Empty;
        uint[] t = new uint[a.Length - n];
        Array.Copy(a, n, t, 0, t.Length);
        return t;
    }

    private static void AddAt(uint[] r, int offset, uint[] x)
    {
        ulong carry = 0;
        int i = 0;
        for (; i < x.Length; i++)
        {
            ulong s = (ulong)r[offset + i] + x[i] + carry;
            r[offset + i] = (uint)(s & Mask);
            carry = s >> 32;
        }
        for (int k = offset + i; carry != 0; k++)
        {
            ulong s = (ulong)r[k] + carry;
            r[k] = (uint)(s & Mask);
            carry = s >> 32;
        }
    }

    public static void DivRem(uint[] a, uint[] b, out uint[] q, out uint[] r)
    {
        if (Compare(a, b) < 0) { q = Empty; r = a; return; }
        if (b.Length == 1)
        {
            uint[] t = (uint[])a.Clone();
            int len = t.Length;
            uint rem = DivSmallInPlace(t, ref len, b[0]);
            q = Trim(t);
            r = rem == 0 ? Empty : new[] { rem };
            return;
        }
        DivRemKnuth(a, b, out q, out r);
    }

    public static uint DivSmallInPlace(uint[] a, ref int len, uint d)
    {
        ulong rem = 0;
        for (int i = len - 1; i >= 0; i--)
        {
            ulong cur = (rem << 32) | a[i];
            a[i] = (uint)(cur / d);
            rem = cur % d;
        }
        while (len > 0 && a[len - 1] == 0) len--;
        return (uint)rem;
    }

    public static void MulAddSmallInPlace(uint[] a, ref int len, uint mul, uint add)
    {
        ulong carry = add;
        for (int i = 0; i < len; i++)
        {
            ulong cur = (ulong)a[i] * mul + carry;
            a[i] = (uint)(cur & Mask);
            carry = cur >> 32;
        }
        if (carry != 0) a[len++] = (uint)carry;
    }

    private static void DivRemKnuth(uint[] u, uint[] v, out uint[] quotient, out uint[] remainder)
    {
        int m = u.Length, n = v.Length;
        int s = Clz(v[n - 1]);

        uint[] vn = new uint[n];
        uint[] un = new uint[m + 1];
        if (s == 0)
        {
            Array.Copy(v, vn, n);
            Array.Copy(u, un, m);
        }
        else
        {
            int rs = 32 - s;
            for (int i = n - 1; i > 0; i--) vn[i] = (v[i] << s) | (v[i - 1] >> rs);
            vn[0] = v[0] << s;
            un[m] = u[m - 1] >> rs;
            for (int i = m - 1; i > 0; i--) un[i] = (u[i] << s) | (u[i - 1] >> rs);
            un[0] = u[0] << s;
        }

        uint[] q = new uint[m - n + 1];
        ulong vTop = vn[n - 1];
        ulong vSecond = vn[n - 2];

        for (int j = m - n; j >= 0; j--)
        {
            ulong num = ((ulong)un[j + n] << 32) | un[j + n - 1];
            ulong qhat = num / vTop;
            ulong rhat = num - qhat * vTop;
            while (qhat > Mask || qhat * vSecond > ((rhat << 32) | un[j + n - 2]))
            {
                qhat--;
                rhat += vTop;
                if (rhat > Mask) break;
            }

            long k = 0;
            for (int i = 0; i < n; i++)
            {
                ulong p = qhat * vn[i];
                long t = (long)un[i + j] - k - (long)(p & Mask);
                un[i + j] = (uint)(t & MaskL);
                k = (long)(p >> 32) - (t >> 32);
            }
            long tt = (long)un[j + n] - k;
            un[j + n] = (uint)(tt & MaskL);

            q[j] = (uint)(qhat & Mask);
            if (tt < 0)
            {
                q[j]--;
                ulong c = 0;
                for (int i = 0; i < n; i++)
                {
                    ulong sum = (ulong)un[i + j] + vn[i] + c;
                    un[i + j] = (uint)(sum & Mask);
                    c = sum >> 32;
                }
                un[j + n] = (uint)((un[j + n] + c) & Mask);
            }
        }

        uint[] r = new uint[n];
        if (s == 0) Array.Copy(un, r, n);
        else
        {
            int rs = 32 - s;
            for (int i = 0; i < n - 1; i++) r[i] = (un[i] >> s) | (un[i + 1] << rs);
            r[n - 1] = un[n - 1] >> s;
        }
        quotient = Trim(q);
        remainder = Trim(r);
    }

    public static uint[] ShiftLeft(uint[] a, int bits)
    {
        if (a.Length == 0 || bits == 0) return a;
        int limbs = bits >> 5, rem = bits & 31;
        uint[] r = new uint[a.Length + limbs + 1];
        if (rem == 0) Array.Copy(a, 0, r, limbs, a.Length);
        else
        {
            uint carry = 0;
            for (int i = 0; i < a.Length; i++)
            {
                r[i + limbs] = (a[i] << rem) | carry;
                carry = a[i] >> (32 - rem);
            }
            r[a.Length + limbs] = carry;
        }
        return Trim(r);
    }

    public static uint[] ShiftRight(uint[] a, int bits)
    {
        if (a.Length == 0 || bits == 0) return a;
        int limbs = bits >> 5, rem = bits & 31;
        if (limbs >= a.Length) return Empty;
        int n = a.Length - limbs;
        uint[] r = new uint[n];
        if (rem == 0) Array.Copy(a, limbs, r, 0, n);
        else
        {
            for (int i = 0; i < n; i++)
            {
                uint lo = a[i + limbs] >> rem;
                uint hi = i + limbs + 1 < a.Length ? a[i + limbs + 1] << (32 - rem) : 0u;
                r[i] = lo | hi;
            }
        }
        return Trim(r);
    }

    public static bool AnyLowBits(uint[] a, int bits)
    {
        int limbs = bits >> 5, rem = bits & 31;
        int full = Math.Min(limbs, a.Length);
        for (int i = 0; i < full; i++) if (a[i] != 0) return true;
        if (limbs < a.Length && rem != 0 && (a[limbs] & ((1u << rem) - 1u)) != 0) return true;
        return false;
    }

    public static ulong Top64(uint[] a, out int shift, out bool sticky)
    {
        int bits = BitLength(a);
        if (bits <= 64)
        {
            ulong v = a[0];
            if (a.Length > 1) v |= (ulong)a[1] << 32;
            shift = 0; sticky = false;
            return v;
        }
        shift = bits - 64;
        sticky = AnyLowBits(a, shift);
        int idx = shift >> 5, o = shift & 31;
        ulong lo = a[idx];
        if (idx + 1 < a.Length) lo |= (ulong)a[idx + 1] << 32;
        if (o == 0) return lo;
        ulong hi = idx + 2 < a.Length ? a[idx + 2] : 0UL;
        return (lo >> o) | (hi << (64 - o));
    }
}