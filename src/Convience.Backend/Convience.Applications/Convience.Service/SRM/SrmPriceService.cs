using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Model.Models.SRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmPriceService
    {
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query);
    }
    public class SrmPriceService:ISrmPriceService
    {
        private readonly IRepository<SrmInforecord> _srmPriceRepository;
        private readonly SRMContext _context;
        public SrmPriceService(IRepository<SrmInforecord> srmPriceRepository, SRMContext context)
        {
            _srmPriceRepository = srmPriceRepository;
            _context = context;
        }
        public ViewSrmPriceDetail GetDetail(SrmQotH[] query)
        {
            //.AsEnumerable().Select(r => r.Field<string>("Name")).ToArray();
            int[] qotIds = query.AsEnumerable().Select(r => r.QotId).ToArray();
            int[] vendorIds = query.AsEnumerable().Select(r => r.VendorId.Value).Distinct().ToArray();
            var temp = query.AsEnumerable().Select(r => new { a = r.RfqId, b = r.VendorId }).ToArray();
            ViewSrmPriceDetail price = new ViewSrmPriceDetail();
            price.qot = query;
            //using (SRMContext _context = new SRMContext())
            //{

            price.material = (from material in _context.SrmQotMaterial
                              join qot in _context.SrmQotHs
                              on material.QotId equals qot.QotId
                              join vendor in _context.SrmVendors
                              on qot.VendorId equals vendor.VendorId
                              where qotIds.Contains(material.QotId.Value)
                              select new viewSrmQotMaterial(material)
                              {
                                  //QotMId = material.QotMId,
                                  //QotId = material.QotId,
                                  //MMaterial = material.MMaterial,
                                  //MPrice = material.MPrice,
                                  //MCostPrice = material.MCostPrice,
                                  //Length = material.Length,
                                  //Width = material.Width,
                                  //Height = material.Height,
                                  //Density = material.Density,
                                  //Weight = material.Weight,
                                  //Note = material.Note,
                                  VendorId = vendor.VendorId,
                                  VendorName = vendor.VendorName
                              }).ToArray();
            //price.material = (from material in db.SrmQotMaterial
            //           where qotIds.Contains(material.QotId.Value)
            //           select material).ToArray();
            price.process = (from process in _context.SrmQotProcesses
                             join qot in _context.SrmQotHs
                             on process.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(process.QotId.Value)
                             select new viewSrmQotProcess(process)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = process.PPrice.Value * (decimal)process.PHours.Value
                             }).ToArray();
            //price.process = (from process in db.SrmQotProcesses
            //                             where qotIds.Contains(process.QotId.Value)
            //                             select process).ToArray();
            price.surface = (from surface in _context.SrmQotSurfaces
                             join qot in _context.SrmQotHs
             on surface.QotId equals qot.QotId
                             join vendor in _context.SrmVendors
                             on qot.VendorId equals vendor.VendorId
                             where qotIds.Contains(surface.QotId.Value)
                             select new viewSrmQotSurface(surface)
                             {
                                 VendorId = vendor.VendorId,
                                 VendorName = vendor.VendorName,
                                 SubTotal = surface.SPrice.Value * (decimal)surface.STimes.Value
                             }).ToArray();
            price.other = (from other in _context.SrmQotOthers
                           join qot in _context.SrmQotHs
             on other.QotId equals qot.QotId
                           join vendor in _context.SrmVendors
                           on qot.VendorId equals vendor.VendorId
                           where qotIds.Contains(other.QotId.Value)
                           select new viewSrmQotOther(other)
                           {
                               VendorId = vendor.VendorId,
                               VendorName = vendor.VendorName
                           }).ToArray();


            //(from material in db.SrmQotMaterial
            // join qot in db.SrmQotHs
            // on material.QotId equals qot.QotId
            // join vendor in db.SrmVendors
            // on qot.VendorId equals vendor.VendorId
            // where qotIds.Contains(material.QotId.Value)
            // select new viewSrmQotMaterial(material)
            // {
            //     VendorId = vendor.VendorId,
            //     VendorName = vendor.VendorName
            // })
            // .GroupBy(r => r.VendorName).Select(g => new
            // {
            //     VendorId = g.Key,
            //     count = g.Sum(s => s.MCostPrice)
            // });

            viewSrmQotInfoRecord[] infos = new viewSrmQotInfoRecord[vendorIds.Count()];
            for (int i = 0; i < infos.Length; i++)
            {
                viewSrmQotInfoRecord info = new viewSrmQotInfoRecord();
                info.VendorId = vendorIds[i];
                info.VendorName = _context.SrmVendors.Where(r => r.VendorId == info.VendorId).Select(r => r.VendorName).FirstOrDefault();
                info.Atotal = price.material.Where(r => r.VendorId == info.VendorId && r.MCostPrice.HasValue).Select(r => r.MCostPrice).Sum().Value;
                info.Btotal = price.process.Where(r => r.VendorId == info.VendorId).Select(r => r.SubTotal).Sum();
                info.Ctotal = price.surface.Where(r => r.VendorId == info.VendorId).Select(r => r.SubTotal).Sum();
                info.Dtotal = price.other.Where(r => r.VendorId == info.VendorId && r.OPrice.HasValue).Select(r => r.OPrice).Sum().Value;
                infos[i] = info;
            }
            price.infoRecord = infos;
            //price.infoRecord = price.material.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.MCostPrice)
            //}).GroupJoin(
            //price.process.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), a => a.VendorId, b => b.VendorId, (material, process) => new
            //{
            //    material = material,
            //    process = process.DefaultIfEmpty()
            //}).GroupJoin(
            //price.surface.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.SubTotal)
            //}), r => r.material.VendorId, c => c.VendorId, (infoRecord, surface) => new
            //{
            //    infoRecord,
            //    surface = surface.DefaultIfEmpty()
            //}).GroupJoin(
            //price.other.GroupBy(r => r.VendorName).Select(g => new
            //{
            //    VendorId = g.Key,
            //    count = g.Sum(s => s.OPrice)
            //}), r => r.infoRecord.material.VendorId, d => d.VendorId, (infoRecord, other) => new viewSrmQotInfoRecord
            //{
            //    Atotal = infoRecord.infoRecord.material.count.Value,
            //    Btotal = infoRecord.infoRecord.process.,
            //    Ctotal = infoRecord.surface.count,
            //    Dtotal = other.count.Value
            //}).DefaultIfEmpty().ToArray();

            //}
            return price;
        }
    }
}
