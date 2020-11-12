using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common
{
    public class MissingRequiredFieldException : Exception
    {
        public MissingRequiredFieldException(string message)
            : base(message)
        {
        }
    }

    public class MissingDomainObjectException : Exception
    {
        public MissingDomainObjectException(string message)
            : base(message)
        {
        }
    }

    public class DuplicatedDomainObjectException : Exception
    {
        public DuplicatedDomainObjectException(string message)
            : base(message)
        {
        }
    }

    public class MissingFileObjectException : Exception
    {
        public MissingFileObjectException(string message)
            : base(message)
        {
        }
    }


    public class ValidatebjectException : Exception
    {
        public ValidatebjectException(string message)
            : base(message)
        {
        }
    }
}
