﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.CrossCircuitCommunication
{
    public class MessagePayload
    {
        public string Key;
        public int Index;
        public object Message;
    }
}
