namespace MsbtLib
{
    public class MsbtException
    {
        public class InvalidMagicException : Exception
        {
            public InvalidMagicException() : base("Invalid magic bytes") { }
            public InvalidMagicException(string message) : base($"Invalid magic bytes: {message}") { }
            public InvalidMagicException(string message, Exception exception) : base($"Invalid magic bytes: {message}", exception) { }
        }

        public class InvalidBomException : Exception
        {
            public InvalidBomException() : base("Invalid BOM") { }
            public InvalidBomException(string message) : base($"Invalid BOM: {message}") { }
            public InvalidBomException(string message, Exception exception) : base($"Invalid BOM: {message}", exception) { }
        }

        public class InvalidEncodingException : Exception
        {
            public InvalidEncodingException() : base("Invalid encoding") { }
            public InvalidEncodingException(string message) : base($"Invalid encoding: {message}") { }
            public InvalidEncodingException(string message, Exception exception) : base($"Invalid encoding: {message}", exception) { }
        }

        public class InvalidUtf8Exception : Exception
        {
            public InvalidUtf8Exception() : base("Invalid UTF-8") { }
            public InvalidUtf8Exception(string message) : base($"Invalid UTF-8: {message}") { }
            public InvalidUtf8Exception(string message, Exception exception) : base($"Invalid UTF-8: {message}", exception) { }
        }

        public class InvalidUtf16Exception : Exception
        {
            public InvalidUtf16Exception() : base("Invalid UTF-16") { }
            public InvalidUtf16Exception(string message) : base($"Invalid UTF-16: {message}") { }
            public InvalidUtf16Exception(string message, Exception exception) : base($"Invalid UTF-16: {message}", exception) { }
        }
    }
}
