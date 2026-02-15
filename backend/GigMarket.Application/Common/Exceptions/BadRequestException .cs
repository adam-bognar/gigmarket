using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Common.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public string[]? Errors { get; }

        public BadRequestException(string message, string[]? errors = null) : base(message)
            => Errors = errors;
    }
}
