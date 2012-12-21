using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Momo.Common;

namespace Momo.Domain
{
    public class CommandResult
    {
        private readonly IList<CommandError> _errors = new List<CommandError>();
        private readonly dynamic _data = new ExpandoObject();

        public dynamic Data
        {
            get { return _data; }
        }

        public IReadOnlyList<CommandError> Errors
        {
            get { return _errors.AsReadOnly(); }
        }

        public CommandResult Add(CommandError error)
        {
            _errors.Add(error);
            return this;
        }

        public CommandResult Add(string key, string error)
        {
            return Add(new CommandError(key, error));
        }

        public CommandResult Add(string error)
        {
            return Add(new CommandError("", error));
        }

        public bool AnyErrors()
        {
            return _errors.Any();
        }

        public bool NoErrors()
        {
            return !AnyErrors();
        }
    }

    public class CommandError
    {
        public CommandError(string key, string error)
        {
            Key = key ?? "";
            Error = error ?? "Unknown error";
        }

        public string Key { get; private set; }
        public string Error { get; private set; }
    }
}
