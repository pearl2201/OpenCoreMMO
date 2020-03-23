﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Model.Items
{
    public class CipElement
    {
        public CipElement(int data)
        {
            Data = data;
            Attributes = new List<CipAttribute>();
        }

        public int Data { get; set; }

        public IList<CipAttribute> Attributes { get; set; }
    }

    public class CipAttribute
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}