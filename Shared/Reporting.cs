using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;


namespace daedalus.Shared.Model
{

    public class Period
    {
        public PeriodType PeriodType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
