﻿using System;

namespace Momo.Domain.Entities
{
    public class Log : Entity
    {
        public virtual DateTimeOffset Date { get; protected set; }
        public virtual string Level { get; protected set; }
        public virtual string Logger { get; protected set; }
        public virtual string Thread { get; protected set; }
        public virtual string Username { get; protected set; }
        public virtual string Message { get; protected set; }
        public virtual string Exception { get; protected set; }
    }
}
