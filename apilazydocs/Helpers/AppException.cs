using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Helpers
{
    // Custom exception class for throwing application specific exceptions (e.g. for validation) 
    // that can be caught and handled within the application
    public class AppException : Exception
    {
        public int statusCode { get; set; }
        public AppException() : base() { }

        public AppException(string message, int statusCode = 400) : base(message)
        {
            this.statusCode = statusCode;
        }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }

    }
}
