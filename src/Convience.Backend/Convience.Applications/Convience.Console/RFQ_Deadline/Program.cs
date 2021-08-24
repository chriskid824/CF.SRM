using System;
using Convience.Entity.Entity.SRM;
using Convience.Service.SRM;
using Microsoft.Extensions.DependencyInjection;

namespace RFQ_Deadline
{
    class Program
    {
        static void Main(string[] args)
        {
            var collection = new ServiceCollection();
            collection
                .AddDbContext<SRMContext>()
                .AddScoped<ISrmRfqHService, SrmRfqHService>();
            var rfqHservice = collection.BuildServiceProvider().GetService<ISrmRfqHService>();
        }
    }
}
