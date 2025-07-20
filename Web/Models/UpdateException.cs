using System.Runtime.Serialization;

[Serializable]
internal class UpdateException : Exception
{
    public UpdateException()
    {
    }

    public UpdateException(string? message) : base(message)
    {
    }

    public UpdateException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}