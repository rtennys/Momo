using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Momo.Domain
{
    public class DomainResult
    {
        private readonly IList<DomainError> _errors = new List<DomainError>();

        public dynamic Data { get; } = new ExpandoObject();

        public IReadOnlyList<DomainError> Errors => _errors.AsReadOnly();

        public DomainResult Add(string key, string error)
        {
            _errors.Add(new DomainError(key, error));
            return this;
        }

        public DomainResult Add(string error)
        {
            return Add("", error);
        }

        public bool AnyErrors()
        {
            return _errors.Any();
        }
    }

    public class DomainError
    {
        public DomainError(string key, string error)
        {
            Key = key ?? "";
            Error = error ?? "Unknown error";
        }

        public string Key { get; }
        public string Error { get; }
    }
}
