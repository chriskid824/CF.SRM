using System;
using System.IO;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Model.Models.SRM;
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
                .AddRepositories<SRMContext>() 
                .AddScoped<ISrmRfqHService, SrmRfqHService>();

            var a = collection.BuildServiceProvider();
            var rfqHservice = a.GetService<ISrmRfqHService>();
            rfqHservice.End(new QueryRfqList()
            {
                werks = new int[] {1100, 1200, 3100 },
                status = (int)Status.啟動,
                end = true
            });


            //var dbOptions = new DbContextOptionsBuilder<SRMContext>()
            //    .UseSqlServer("Data Source=10.1.1.181;Initial Catalog=SRM;User ID=sa;Password=Chen@full")
            //    .Options;
            //IRepository<SrmRfqH>, BaseRepository < SrmRfqH, SRMContext >
            //using (var db = new SRMContext(dbOptions))
            //{
            //    var repo = new SrmRfqHService(SrmRfqHService, db,null);
            //    var VehicleList = repo.GetRfqList(
            //        new QueryRfqList()
            //        {
            //            end = true
            //        }
            //        );
            //}

        }
    }
}
