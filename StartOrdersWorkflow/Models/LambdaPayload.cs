using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartOrdersWorkflow.Models
{
    public class LambdaPayload
    {
        public IEnumerable<string> ProductIds { get; set; }
    }
}
