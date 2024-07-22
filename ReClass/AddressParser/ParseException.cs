namespace ReClass.AddressParser;

public class ParseException : Exception {
    public ParseException(string message)
        : base(message) {
    }
}
