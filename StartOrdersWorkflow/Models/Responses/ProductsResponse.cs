using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartOrdersWorkflow.Models.Responses
{
    public class ProductsResponse : LambdaResponse
    {
        public IEnumerable<Product> Products { get; set;}
    }
}
