namespace UniversiteDomain.Exceptions.SecurityExceptions;

[Serializable]
public class UniversiteUserNotFoundException : Exception
{
    public UniversiteUserNotFoundException() : base() { }
    public UniversiteUserNotFoundException(string message) : base(message) { }
    public UniversiteUserNotFoundException(string message, Exception inner) : base(message, inner) { }
}