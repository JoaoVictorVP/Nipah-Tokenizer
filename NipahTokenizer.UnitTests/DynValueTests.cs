using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NipahTokenizer.UnitTests;

public class DynValueTests
{
    [Fact]
    public void From_ByteValue_ReturnsDynValue()
    {
        // Arrange
        byte value = 5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.U8);
    }

    [Fact]
    public void From_BooleanValue_ReturnsDynValue()
    {
        // Arrange
        bool value = true;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.Bool);
    }

    [Fact]
    public void From_ShortValue_ReturnsDynValue()
    {
        // Arrange
        short value = -5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.I16);
    }

    [Fact]
    public void From_UShortValue_ReturnsDynValue()
    {
        // Arrange
        ushort value = 5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.U16);
    }

    [Fact]
    public void From_IntValue_ReturnsDynValue()
    {
        // Arrange
        int value = -5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.I32);
    }

    [Fact]
    public void From_UIntValue_ReturnsDynValue()
    {
        // Arrange
        uint value = 5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.U32);
    }

    [Fact]
    public void From_LongValue_ReturnsDynValue()
    {
        // Arrange
        long value = -5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.I64);
    }

    [Fact]
    public void From_ULongValue_ReturnsDynValue()
    {
        // Arrange
        ulong value = 5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.U64);
    }

    [Fact]
    public void From_FloatValue_ReturnsDynValue()
    {
        // Arrange
        float value = 5.5f;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.F32);
    }

    [Fact]
    public void From_DoubleValue_ReturnsDynValue()
    {
        // Arrange
        double value = 5.5;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.F64);
    }

    [Fact]
    public void From_StringValue_ReturnsDynValue()
    {
        // Arrange
        string value = "hello";

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Type.Should().Be(DynType.String);
    }

    [Fact]
    public void From_NullValue_ReturnsNone()
    {
        // Arrange
        string? value = null;

        // Act
        DynValue result = DynValue.From(value);

        // Assert
        result.Should().Be(DynValue.None);
    }

    [Fact]
    public void ImplicitConversion_DynValueToByte_ReturnsByteValue()
    {
        // Arrange
        DynValue value = DynValue.From((byte)5);

        // Act
        byte result = value;

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToBoolean_ReturnsBooleanValue()
    {
        // Arrange
        DynValue value = DynValue.From(true);

        // Act
        bool result = value;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_DynValueToShort_ReturnsShortValue()
    {
        // Arrange
        DynValue value = DynValue.From((short)-5);

        // Act
        short result = value;

        // Assert
        result.Should().Be(-5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToUShort_ReturnsUShortValue()
    {
        // Arrange
        DynValue value = DynValue.From((ushort)5);

        // Act
        ushort result = value;

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToInt_ReturnsIntValue()
    {
        // Arrange
        DynValue value = DynValue.From(-5);

        // Act
        int result = value;

        // Assert
        result.Should().Be(-5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToUInt_ReturnsUIntValue()
    {
        // Arrange
        DynValue value = DynValue.From((uint)5);

        // Act
        uint result = value;

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToLong_ReturnsLongValue()
    {
        // Arrange
        DynValue value = DynValue.From((long)-5);

        // Act
        long result = value;

        // Assert
        result.Should().Be(-5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToULong_ReturnsULongValue()
    {
        // Arrange
        DynValue value = DynValue.From((ulong)5);

        // Act
        ulong result = value;

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToFloat_ReturnsFloatValue()
    {
        // Arrange
        DynValue value = DynValue.From(5.5f);

        // Act
        float result = value;

        // Assert
        result.Should().Be(5.5f);
    }

    [Fact]
    public void ImplicitConversion_DynValueToDouble_ReturnsDoubleValue()
    {
        // Arrange
        DynValue value = DynValue.From(5.5);

        // Act
        double result = value;

        // Assert
        result.Should().Be(5.5);
    }

    [Fact]
    public void ImplicitConversion_DynValueToString_ReturnsStringValue()
    {
        // Arrange
        DynValue value = DynValue.From("hello");

        // Act
        string result = value;

        // Assert
        result.Should().Be("hello");
    }

    [Fact]
    public void WithObject_ShouldReturn_ValidLongValue()
    {
        const long lvalue = 103409086;

        // Arrange
        var value = DynValue.From(lvalue);
        
        // Act
        var res = value.TrySolveInstance();

        // Assert
        res.Match(valid =>
        {
            valid.Should().BeOfType<long>();
            valid.Should().Be(lvalue);
        },
        () => throw new Exception("Invalid value"));
    }
}
