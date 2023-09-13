using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartOrdersWorkflow.Models
{
    [DynamoDBTable("dev-bg-products-table")]
    public class Product
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Type { get; set; }
        public int AvailableAfter { get; set; }
    }
}
