namespace BomComparer.Exceptions
{
    public class MissingColumnNameException : Exception
    {
        public MissingColumnNameException()
        {
        }

        public MissingColumnNameException(string propertyName) 
            : base($"The property '{propertyName}' is missing the 'ColumnName' attribute.")
        {
        }

        public MissingColumnNameException(string propertyName, Exception inner)
            : base($"The property '{propertyName}' is missing the 'ColumnName' attribute.", inner)
        {
        }
    }
}
