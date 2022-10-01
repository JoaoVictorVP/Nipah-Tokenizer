using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NipahTokenizer;

public unsafe struct DynValue
{
    public static readonly DynValue None = new();
    fixed byte dat[8];
    object _ref;
    DynType type;

    public DynType Type => type;

    #region From
    public static DynValue From<T>(T value)
    {
        if (value is null) return None;
        var dyn = new DynValue();
        dyn.Set(value);
        return dyn;
    }

    public static implicit operator DynValue(byte x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(bool x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(short x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(ushort x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(int x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(uint x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(long x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(ulong x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(float x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(double x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    public static implicit operator DynValue(string x)
    {
        var dyn = new DynValue();
        dyn.Set(x);
        return dyn;
    }
    #endregion

    #region To
    public static implicit operator byte(DynValue dyn) => dyn.Solve<byte>();
    public static implicit operator bool(DynValue dyn) => dyn.Solve<bool>();
    public static implicit operator short(DynValue dyn) => dyn.Solve<short>();
    public static implicit operator ushort(DynValue dyn) => dyn.Solve<ushort>();
    public static implicit operator int(DynValue dyn) => dyn.Solve<int>();
    public static implicit operator uint(DynValue dyn) => dyn.Solve<uint>();
    public static implicit operator long(DynValue dyn) => dyn.Solve<long>();
    public static implicit operator ulong(DynValue dyn) => dyn.Solve<ulong>();
    public static implicit operator float(DynValue dyn) => dyn.Solve<float>();
    public static implicit operator double(DynValue dyn) => dyn.Solve<double>();
    public static implicit operator string(DynValue dyn) => dyn.Solve<string>();
    #endregion

    public void Set<T>(T value)
    {
        switch (value)
        {
            case byte x:
            {
                dat[0] = x;
                type = DynType.U8;
                break;
            }
            case sbyte x:
            {
                dat[0] = (byte)x;
                type = DynType.I8;
                break;
            }
            case bool x:
            {
                dat[0] = (byte)(x is true ? 1 : 0);
                type = DynType.Bool;
                break;
            }
            case short x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.I16;
                break;
            }
            case ushort x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.U16;
                break;
            }
            case int x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.I32;
                break;
            }
            case uint x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.U32;
                break;
            }
            case long x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.I64;
                break;
            }
            case ulong x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.U64;
                break;
            }
            case float x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.F32;
                break;
            }
            case double x:
            {
                fixed (void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.F64;
                break;
            }
            case char x:
            {
                fixed(void* ptr = dat)
                {
                    Unsafe.Copy(ptr, ref x);
                }
                type = DynType.Char;
                break;
            }
            case ValueType x:
            {
                if (Unsafe.SizeOf<T>() <= 8)
                {
                    fixed (void* ptr = dat)
                    {
                        var tx = __refvalue(__makeref(x), T);
                        Unsafe.Copy(ptr, ref tx);
                    }
                    type = DynType.Other;
                }
                else
                {
                    _ref = x;
                    type = DynType.Ref;
                }
                break;
            }
            case string x:
            {
                _ref = x;
                type = DynType.String;
                break;
            }
            case object x:
            {
                _ref = x;
                type = DynType.Ref;
                break;
            }
            default:
            {
                type = DynType.Null;
                break;
            }
        }
    }

    T SolveBool<T>()
    {
        bool res = dat[0] is 1;
        return __refvalue(__makeref(res), T);
    }
    T SolveNum<TNum, T>() where TNum : unmanaged
    {
        fixed(void* ptr = dat)
        {
            var num = *(TNum*)ptr;
            return __refvalue(__makeref(num), T);
        }
    }
    T SolveOther<T>()
    {
        fixed(void* ptr = dat)
        {
            return Unsafe.AsRef<T>(ptr);
        }
    }
    public T Solve<T>()
    {
        return type switch
        {
            DynType.U8 => SolveNum<byte, T>(),
            DynType.I8 => SolveNum<sbyte, T>(),
            DynType.Bool => SolveBool<T>(),

            DynType.I16 => SolveNum<short, T>(),
            DynType.I32 => SolveNum<int, T>(),
            DynType.I64 => SolveNum<long, T>(),
            DynType.U16 => SolveNum<ushort, T>(),
            DynType.U32 => SolveNum<uint, T>(),
            DynType.U64 => SolveNum<ulong, T>(),
            DynType.F32 => SolveNum<float, T>(),
            DynType.F64 => SolveNum<double, T>(),

            DynType.Char => SolveNum<char, T>(),

            DynType.Other => SolveOther<T>(),

            DynType.String or DynType.Ref => (T)_ref,

            _ or DynType.Null => default
        };
    }
    public Result<T> TrySolve<T>()
    {
        var type = FromT<T>();
        if (this.type == type)
            return new(Solve<T>());
        return Result<T>.None;
    }
    DynType FromT<T>()
    {
        var t = typeof(T);
        if (t == typeof(byte)) return DynType.U8;
        else if (t == typeof(sbyte)) return DynType.I8;
        else if (t == typeof(bool)) return DynType.Bool;
        else if (t == typeof(short)) return DynType.I16;
        else if (t == typeof(ushort)) return DynType.U16;
        else if (t == typeof(int)) return DynType.I32;
        else if (t == typeof(uint)) return DynType.U32;
        else if (t == typeof(long)) return DynType.I64;
        else if (t == typeof(ulong)) return DynType.U64;
        else if (t == typeof(float)) return DynType.F32;
        else if (t == typeof(double)) return DynType.F64;
        else if (t == typeof(string)) return DynType.String;
        else if (t == typeof(char)) return DynType.Char;
        else if (t.IsSubclassOf(typeof(ValueType)) && Unsafe.SizeOf<T>() <= 8) return DynType.Other;
        else if (t.IsSubclassOf(typeof(object))) return DynType.Ref;
        else return DynType.Null;
    }
}
public enum DynType : byte
{
    Null,
    Bool,
    I8,
    I16,
    I32,
    I64,
    U8,
    U16,
    U32,
    U64,
    F32,
    F64,
    Char,
    Other,
    String,
    Ref
}
