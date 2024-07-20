using System.Diagnostics.Contracts;
using System.Text;
using ReClassNET.Extensions;

namespace ReClassNET.MemoryScanner;

public abstract class ScanResult {
    public abstract ScanValueType ValueType { get; }

    public IntPtr Address { get; set; }

    public abstract int ValueSize { get; }

    public abstract ScanResult Clone();
}

public class ByteScanResult : ScanResult, IEquatable<ByteScanResult> {
    public override ScanValueType ValueType => ScanValueType.Byte;

    public override int ValueSize => sizeof(byte);

    public byte Value { get; }

    public ByteScanResult(byte value) {
        Value = value;
    }

    public bool Equals(ByteScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new ByteScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as ByteScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class ShortScanResult : ScanResult, IEquatable<ShortScanResult> {
    public override ScanValueType ValueType => ScanValueType.Short;

    public override int ValueSize => sizeof(short);

    public short Value { get; }

    public ShortScanResult(short value) {
        Value = value;
    }

    public bool Equals(ShortScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new ShortScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as ShortScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class IntegerScanResult : ScanResult, IEquatable<IntegerScanResult> {
    public override ScanValueType ValueType => ScanValueType.Integer;

    public override int ValueSize => sizeof(int);

    public int Value { get; }

    public IntegerScanResult(int value) {
        Value = value;
    }

    public bool Equals(IntegerScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new IntegerScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as IntegerScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class LongScanResult : ScanResult, IEquatable<LongScanResult> {
    public override ScanValueType ValueType => ScanValueType.Long;

    public override int ValueSize => sizeof(long);

    public long Value { get; }

    public LongScanResult(long value) {
        Value = value;
    }

    public bool Equals(LongScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new LongScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as LongScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class FloatScanResult : ScanResult, IEquatable<FloatScanResult> {
    public override ScanValueType ValueType => ScanValueType.Float;

    public override int ValueSize => sizeof(float);

    public float Value { get; }

    public FloatScanResult(float value) {
        Value = value;
    }

    public bool Equals(FloatScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new FloatScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as FloatScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class DoubleScanResult : ScanResult, IEquatable<DoubleScanResult> {
    public override ScanValueType ValueType => ScanValueType.Double;

    public override int ValueSize => sizeof(double);

    public double Value { get; }

    public DoubleScanResult(double value) {
        Value = value;
    }

    public bool Equals(DoubleScanResult other) => other != null && Address == other.Address && Value == other.Value;

    public override ScanResult Clone() => new DoubleScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as DoubleScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class ArrayOfBytesScanResult : ScanResult, IEquatable<ArrayOfBytesScanResult> {
    public override ScanValueType ValueType => ScanValueType.ArrayOfBytes;

    public override int ValueSize => Value.Length;

    public byte[] Value { get; }

    public ArrayOfBytesScanResult(byte[] value) {
        Contract.Requires(value != null);

        Value = value;
    }

    public bool Equals(ArrayOfBytesScanResult other) => other != null && Address == other.Address && Value.SequenceEqual(other.Value);

    public override ScanResult Clone() => new ArrayOfBytesScanResult(Value) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as ArrayOfBytesScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode();
}

public class StringScanResult : ScanResult, IEquatable<StringScanResult> {
    public override ScanValueType ValueType => ScanValueType.String;

    public override int ValueSize => Value.Length * Encoding.GuessByteCountPerChar();

    public string Value { get; }

    public Encoding Encoding { get; }

    public StringScanResult(string value, Encoding encoding) {
        Contract.Requires(value != null);
        Contract.Requires(encoding != null);

        Value = value;
        Encoding = encoding;
    }

    public bool Equals(StringScanResult other) => other != null && Address == other.Address && Value == other.Value && Encoding.Equals(other.Encoding);

    public override ScanResult Clone() => new StringScanResult(Value, Encoding) { Address = Address };

    public override bool Equals(object obj) => Equals(obj as StringScanResult);

    public override int GetHashCode() => Address.GetHashCode() * 19 + Value.GetHashCode() * 19 + Encoding.GetHashCode();
}

public class RegexStringScanResult : StringScanResult {
    public override ScanValueType ValueType => ScanValueType.Regex;

    public RegexStringScanResult(string value, Encoding encoding)
        : base(value, encoding) {

    }

    public override ScanResult Clone() => new RegexStringScanResult(Value, Encoding) { Address = Address };
}
