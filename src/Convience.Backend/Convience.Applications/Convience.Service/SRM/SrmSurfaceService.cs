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
    public interface ISrmSurfaceService
    {
        public PagingResultModel<SrmSurface> GetSurfaceList(QuerySrmSurface query);
        public string AddSurface(SrmSurface surface);
    }
    public class SrmSurfaceService: ISrmSurfaceService
    {
        private readonly SRMContext _context;
        public SrmSurfaceService(SRMContext context)
        {
            _context = context;
        }
        public PagingResultModel<SrmSurface> GetSurfaceList(QuerySrmSurface query)
        {
            int skip = (query.Page - 1) * query.Size;
            var resultQuery = (from surface in _context.SrmSurfaces
                               select surface)
                .AndIfHaveValue(query.surface.SurfaceDesc, r => r.SurfaceDesc.Contains(query.surface.SurfaceDesc));
            var surfaces = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<SrmSurface>
            {
                Data = JsonConvert.DeserializeObject<SrmSurface[]>(JsonConvert.SerializeObject(surfaces)),
                Count = resultQuery.Count()
            };
        }
        public string AddSurface(SrmSurface surface)
        {
            surface.SurfaceDesc = surface.SurfaceDesc.Trim();
            if (_context.SrmSurfaces.Where(r => r.SurfaceDesc.Equals(surface.SurfaceDesc)).Any())
            {
                return ($"{surface.SurfaceDesc}已存在");
            }
            surface.Staus = (int)Status.已核發;
            _context.Add(surface);
            _context.SaveChanges();
            return "";
        }
    }
}
