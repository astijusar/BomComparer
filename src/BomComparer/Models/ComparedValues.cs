using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomComparer.Models
{
    public class ComparedValues<T>
    {
        public T SourceValue { get; set; }
        public T TargetValue { get; set; }

        public ComparedValues(T source, T targetValue)
        {
            SourceValue = source;
            TargetValue = targetValue;
        }
    }
}
