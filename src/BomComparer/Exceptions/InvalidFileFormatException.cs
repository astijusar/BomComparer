namespace BomComparer.Exceptions
{
    public class InvalidFileFormatException : Exception
    {
        public InvalidFileFormatException()
        {
        }

        public InvalidFileFormatException(string message) : base(message)
        {
        }

        public InvalidFileFormatException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
