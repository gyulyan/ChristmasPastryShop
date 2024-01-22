﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ChristmasPastryShop.Models.Delicacies
{
    public class Gingerbread : Delicacy
    {
        private const double GingerbreadPrice = 4;
        public Gingerbread(string name) 
            : base(name, GingerbreadPrice)
        {
        }
    }
}
