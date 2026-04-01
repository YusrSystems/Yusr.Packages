using System;
using System.Collections.Generic;
using System.Text;

namespace Yusr.Core.Abstractions.Utilities
{
    public static class TaxHelper
    {
        public static decimal GetTaxFactor(decimal taxPerc)
        {
            return YusrMath.Round((100 + taxPerc) / 100);
        }
    }
}
