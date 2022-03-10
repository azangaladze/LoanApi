using System;

namespace LoanProject.Core.Exceptions
{
    public class EntityNotFoundException<T> : Exception where T : class
    {

    }

    public class UserExistsException : Exception
    {

    }

    public class IncorrectPasswordException : Exception
    {

    }
}
