using Convience.Entity.Entity.SRM;
using Convience.Model.Models;
using Convience.Model.Models.SRM;
using Convience.Util.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmProcessService
    {
        public PagingResultModel<SrmProcess> GetProcessList(QuerySrmProcess query);
        public string AddProcess(SrmProcess process);
    }
    public class SrmProcessService: ISrmProcessService
    {
        private readonly SRMContext _context;
        public SrmProcessService(SRMContext context)
        {
            _context = context;
        }
        public PagingResultModel<SrmProcess> GetProcessList(QuerySrmProcess query)
        {
            int skip = (query.Page - 1) * query.Size;
            var resultQuery = (from process in _context.SrmProcesss
                               select process)
                .AndIfHaveValue(query.process.Process, r => r.Process.Contains(query.process.Process));
            var processes = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<SrmProcess>
            {
                Data = JsonConvert.DeserializeObject<SrmProcess[]>(JsonConvert.SerializeObject(processes)),
                Count = resultQuery.Count()
            };
        }
        public string AddProcess(SrmProcess process)
        {
            process.Process = process.Process.Trim();
            if (_context.SrmProcesss.Where(r => r.Process.Equals(process.Process)).Any())
            {
                return ($"{process.Process}已存在");
            }
            process.Staus = (int)Status.已核發;
            _context.Add(process);
            _context.SaveChanges();
            return "";
        }
    }
}
