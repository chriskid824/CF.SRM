using AutoMapper;

using Convience.Entity.Data;
using Convience.Entity.Entity;
using Convience.Entity.Entity.Identity;
using Convience.Entity.Entity.SRM;
using Convience.EntityFrameWork.Repositories;
using Convience.Filestorage.Abstraction;
using Convience.JwtAuthentication;
using Convience.Model.Constants.SystemManage;
using Convience.Model.Models;
using Convience.Model.Models.SRM;
using Convience.Model.Models.SystemManage;
using Convience.Util.Extension;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmFileService
    {
        public bool AddTemplate(SrmFileUploadTemplate data);

        public PagingResultModel<ViewSrmFileUploadTemplate> GetTemplateList(QueryFile query);
        public IEnumerable<ViewSrmFileRecordResult> GetFileListByNumer(QueryFile query);
        public bool UpdateTemplate(SrmFileUploadTemplate data);
        public bool DeleteDeliveryL(ViewSrmDeliveryL data);
        public string ReceiveDeliveryL(List<ViewSrmDeliveryL> datalist);
        public Task<string> UploadAsync(FileUploadViewModel viewModel);
        public Task<Stream> DownloadAsync(NzFileViewModel viewModel);
        public Task<bool> DeleteFileAsync(NzFileViewModel viewModel);
        public IEnumerable<AnnouncementType> GetAnnList(QueryFile query);
    }

    public class SrmFileService : ISrmFileService
    {
        private readonly SRMContext _context;

        //private readonly IMapper _mapper;
        private readonly IRepository<SrmPoH> _srmPohRepository;

        private readonly IRepository<SrmPoL> _srmPolRepository;
        private IMapper _mapper;
        private readonly IFileStore _fileStore;
        private readonly appSettings _appSettingsService;
        private readonly ISrmPoService _srmPoService;

        public SrmFileService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper, IFileStore fileStore
            , IOptions<appSettings> appSettingsOption, ISrmPoService srmPoService)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            _fileStore = fileStore;
            _appSettingsService = appSettingsOption.Value;
            _srmPoService = srmPoService;
            //_systemIdentityDbUnitOfWork = systemIdentityDbUnitOfWork;
        }

        public bool AddTemplate(SrmFileUploadTemplate data)
        {
            //SrmFileUploadTemplate insertdata = new SrmFileUploadTemplate() {
            //    TemplateType= data.TemplateType,
            //    Werks=data.Werks,
            //    Type=data.Type,
            //    EffectiveDate=data.EffectiveDate,
            //    Deadline=data.Deadline,
            //    Filetype=data.Filetype,
            //    CreateDate=DateTime.Now
            //};
            _context.SrmFileUploadTemplates.Add(data);
            _context.SaveChanges();
            return true;
        }
        public PagingResultModel<ViewSrmFileUploadTemplate> GetTemplateList(QueryFile query)
        {
            var result = (from tmp in _context.SrmFileUploadTemplates
                          join func in _context.SrmFunctionLists on tmp.TemplateType equals func.FunctionId
                          select new ViewSrmFileUploadTemplate
                          {
                              TemplateId = tmp.TemplateId,
                              TemplateType = tmp.TemplateType,
                              Werks = tmp.Werks,
                              Type = tmp.Type,
                              EffectiveDate = tmp.EffectiveDate,
                              Deadline = tmp.Deadline,
                              Filetype = tmp.Filetype,
                              CreateDate = tmp.CreateDate,
                              CreateBy = tmp.CreateBy,
                              LastUpdateDate = tmp.LastUpdateDate,
                              LastUpdateBy = tmp.LastUpdateBy,
                              FunctionName = func.FunctionName
                          })
                .AndIfCondition(query.id != 0, p => p.TemplateId == query.id)
                .AndIfCondition(query.werk != 0, p => p.Werks == query.werk)
                .ToList();
            if (result.Count() > 0)
            {
                result.ForEach(p => {
                if (!string.IsNullOrEmpty(p.Filetype))
                {
                        string[] filtypes = p.Filetype.Split(',');
                        var names = _context.SrmFileTypeProfiles
                         .Where(m => filtypes.Contains(m.TypeId.ToString()))
                        .Select(m => m.TypeName).ToArray();
                        p.FileTypesDesc = string.Join(',', names);
                    }

                });
            }
            return new PagingResultModel<ViewSrmFileUploadTemplate>
            {
                Data = result,
                Count = result.Count()
            };
        }

        public bool UpdateTemplate(SrmFileUploadTemplate data)
        {
            _context.SrmFileUploadTemplates.Update(data);
            _context.SaveChanges();
            return true;
        }
        public bool DeleteDeliveryL(ViewSrmDeliveryL data)
        {
            SrmDeliveryL dl = _context.SrmDeliveryLs.Find(data.DeliveryLId);
            if (dl != null)
            {
                _context.SrmDeliveryLs.Remove(dl);
                SrmPoL pol = _context.SrmPoLs.Find(data.PoId, data.PoLId);
                pol.Status = 14;
                _context.SrmPoLs.Update(pol);
                SrmPoH poh = _context.SrmPoHs.Find(data.PoId);
                poh.Status = 14;
                _context.SrmPoHs.Update(poh);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public string ReceiveDeliveryL(List<ViewSrmDeliveryL> datalist)
        {
            #region 1.修改status
            ViewSrmDeliveryL dl = datalist.FirstOrDefault();
            #region 1.1 dh
            SrmDeliveryH dh = _context.SrmDeliveryHs.Find(dl.DeliveryId);
            dh.Status = 12;
            _context.SrmDeliveryHs.Update(dh);
            #endregion
            #region 1.2 pol
            datalist.ForEach(m =>
                {
                    SrmPoL pl = _context.SrmPoLs.Find(m.PoId, m.PoLId);
                    pl.Status = 12;
                    _context.SrmPoLs.Update(pl);
                });
            #endregion
            _context.SaveChanges();
            #region poh
            List<int?> poidList = datalist.Select(p => p.PoId).ToList();
            poidList.ForEach(m =>
            {
                if (_context.SrmPoLs.Any(p => p.PoId == m && p.Status != 12))
                { }
                else
                {
                    SrmPoH ph = _context.SrmPoHs.Find(m);
                    ph.Status = 12;
                    _context.SrmPoHs.Update(ph);
                }
            });
            _context.SaveChanges();
            #endregion
            #endregion
            //2.rfc


            return null;
        }
        public bool CheckAllDelivery(int poid, int polid)
        {
            float deliveryQty = _context.SrmDeliveryLs.Where(p => p.PoId == poid && p.PoLId == polid).Sum(p => p.DeliveryQty).Value;
            float Qty = _context.SrmPoLs.Where(p => p.PoId == poid && p.PoLId == polid).Select(p => p.Qty).FirstOrDefault().Value;
            return deliveryQty == Qty;
        }
        public IEnumerable<ViewSrmFileRecordResult> GetFileListByNumer(QueryFile query)
        {
            var configuration = new MapperConfiguration(cfg => cfg.CreateMap<ViewSrmFileTemplate, ViewSrmFileRecord>());
            List<ViewSrmFileRecordResult> result = new List<ViewSrmFileRecordResult>();
            result=_context.ViewSrmFileRecords
                .Where(p => p.TemplateType == query.functionId && p.Number == query.number && p.Werks == query.werk && p.Type == query.type)
                .Select(p => new ViewSrmFileRecordResult
                {
                    RecordHId=p.RecordHId,
                    TemplateId=p.TemplateId,
                    Number=p.Number,
                    TemplateType = p.TemplateType,
                    FunctionName = p.FunctionName,
                    Werks = p.Werks,
                    Type = p.Type,
                    EffectiveDate = p.EffectiveDate,
                    Deadline = p.Deadline,
                    Filetype = p.Filetype,
                    TypeName = p.TypeName,
                    RecordLId=p.RecordLId,
                    Url=p.Url,
                    CreateBy=p.CreateBy,
                    CreateDate=p.CreateDate,
                    LastUpdateBy=p.LastUpdateBy,
                    LastUpdateDate=p.LastUpdateDate
                })
                .ToList();
            if (result == null || result.Count() == 0)
            {
                //result = _mapper.Map<List<ViewSrmFileRecord>>(_context.ViewSrmFileTemplates
                //    .Where(p => p.TemplateType == query.functionId && p.Werks==query.werk && p.Type==query.type ).ToList());
                result = _context.ViewSrmFileTemplates
                    .Where(p => p.TemplateType == query.functionId && p.Werks == query.werk && p.Type == query.type)
                    .Select(p => new ViewSrmFileRecordResult
                    {
                        TemplateId = p.TemplateId,
                        TemplateType = p.TemplateType,
                        FunctionName = p.FunctionName,
                        Werks = p.Werks,
                        Type = p.Type,
                        EffectiveDate = p.EffectiveDate,
                        Deadline = p.Deadline,
                        Filetype = p.Filetype,
                        TypeName = p.TypeName,
                    })
                    .ToList();
                //result = _mapper.Map<ViewSrmFileTemplate[], List<ViewSrmFileRecord>>(source);
            }
            else
            {
                result.ForEach(p => {
                    if (p.RecordLId.HasValue)
                    {
                        List<NzFileViewModel> filelist = new List<NzFileViewModel>();
                        NzFileViewModel file = new NzFileViewModel()
                        {
                            Uid = p.RecordLId.Value,
                            Name = p.Url.Substring(p.Url.LastIndexOf('/') + 1, p.Url.Length - p.Url.LastIndexOf('/') - 1),
                            Status = "done"
                        };
                        filelist.Add(file);
                        p.fileList = filelist;
                    }
                });
            }
            return result;
        }

        public async Task<string> UploadAsync(FileUploadViewModel viewModel)
        {
            SrmFileuploadRecordH rh = new SrmFileuploadRecordH();
            if (_context.SrmFileuploadRecordHs.Where(p => p.TemplateId == viewModel.file.TemplateId && p.Number == viewModel.file.Number).Count() <= 0)
            {
                rh = new SrmFileuploadRecordH()
                {
                    CreateBy = viewModel.user.UserName,
                    CreateDate = DateTime.Now,
                    LastUpdateBy = viewModel.user.UserName,
                    LastUpdateDate = DateTime.Now,
                    TemplateId = viewModel.file.TemplateId,
                    Number = viewModel.file.Number
                };
                _context.SrmFileuploadRecordHs.Add(rh);
                _context.SaveChanges();
                viewModel.file.RecordHId = rh.RecordHId;
            }
            else
            {
                rh = _context.SrmFileuploadRecordHs.Where(p => p.TemplateId == viewModel.file.TemplateId && p.Number == viewModel.file.Number).FirstOrDefault();
            }
            //資料庫的部分
            if (viewModel.file.RecordHId == 0)
            {

            }
            int count = 0;

            foreach (var file in viewModel.Files)
            {
                var path =  viewModel.file.Werks.ToString()+'/'+viewModel.file.Number+'/'+ (viewModel.file.Type==1? "廠內" : "廠外");
                ////判斷檔案路徑是否存在，不存在則建立資料夾
                //if (!System.IO.Directory.Exists(path))
                //{
                //    System.IO.Directory.CreateDirectory(path);//不存在就建立目錄
                //}
                path= path + '/' + file.FileName;
                var info = await _fileStore.GetFileInfoAsync(path);
                if (info != null)
                {
                    return "文件名冲突！";
                }

                var stream = file.OpenReadStream();
                var result = await _fileStore.CreateFileFromStreamAsync(path, stream);
                if (string.IsNullOrEmpty(result))
                {
                    return "文件上传失败！";
                }

                SrmFileuploadRecordL rl = new SrmFileuploadRecordL();
                if (_context.SrmFileuploadRecordLs.Where(p => p.Filetype == viewModel.fileTypeList[count] && p.RecordHId == viewModel.file.RecordHId).Count() > 0)
                {
                    rl = _context.SrmFileuploadRecordLs.Where(p => p.Filetype == viewModel.fileTypeList[count] && p.RecordHId == viewModel.file.RecordHId).FirstOrDefault();
                    rl.LastUpdateBy = viewModel.user.UserName;
                    rl.LastUpdateDate = DateTime.Now;
                    rl.Url = path;
                    _context.Update(rl);
                }
                else
                {
                    rl = new SrmFileuploadRecordL()
                    {
                        RecordHId = rh.RecordHId,
                        Filetype = viewModel.fileTypeList[count],
                        Url = path,
                        Enable="Y",
                        CreateBy = viewModel.user.UserName,
                        CreateDate = DateTime.Now,
                        LastUpdateBy = viewModel.user.UserName,
                        LastUpdateDate = DateTime.Now,
                    };
                    _context.Add(rl);
                }

                    count++;
            }
            _context.SaveChanges();
            return string.Empty;
        }
        public async Task<Stream> DownloadAsync(NzFileViewModel viewModel)
        {
            SrmFileuploadRecordL rl= _context.SrmFileuploadRecordLs.Find(viewModel.Uid);
            return await _fileStore.GetFileStreamAsync(rl.Url);
        }
        public async Task<bool> DeleteFileAsync(NzFileViewModel viewModel)
        {
            SrmFileuploadRecordL rl = _context.SrmFileuploadRecordLs.Where(p => p.RecordLId == viewModel.Uid).FirstOrDefault();
            string path = rl.Url;
            _context.SrmFileuploadRecordLs.Remove(rl);
            _context.SaveChanges();
            return await _fileStore.TryDeleteFileAsync(path);

        }
        public IEnumerable<AnnouncementType> GetAnnList(QueryFile query)
        {
            List<AnnouncementType> result = new List<AnnouncementType>();

            result.Add(GetPoAnn(query));
            result.Add(GetDeliveryAnn(query));
            result.Add(GetAnn3(query));
            result.Add(GetAnn4(query));
            result.Add(GetAnn5(query));
            result.Add(GetAnn6(query));

            return result;
        }
        public AnnouncementType GetPoAnn(QueryFile query) {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "black",
                TxtTypeName = "採購單",
                Icon = "mdi-account-box-multiple",
                Router="po",
            };
            QueryPoList q = new QueryPoList();
            //var aaa = query.Property("poNum");
            q.dataStatus = 2;
            q.user = query.user;
            var aaa = _srmPoService.GetAll(q);
            ann.NumberList = aaa.Select(p => p.PoNum).ToList();
            return ann;
        }
        public AnnouncementType GetDeliveryAnn(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "red",
                TxtTypeName = "出貨單",
                Icon = "mdi-finance",
                Router = "delivery",
            };
            return ann;
        }
        public AnnouncementType GetAnn3(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "blue",
                TxtTypeName = "詢價單",
                Icon = "mdi-desktop-classic",
            };
            return ann;
        }
        public AnnouncementType GetAnn4(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "green",
                TxtTypeName = "報價單",
                Icon = "mdi-library",
            };
            return ann;
        }
        public AnnouncementType GetAnn5(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "lightgreen",
                TxtTypeName = "供應商",
                Icon = "mdi-mother-heart",
            };
            return ann;
        }
        public AnnouncementType GetAnn6(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "orange",
                TxtTypeName = "料號",
                Icon = "mdi-google-cloud",
            };
            return ann;
        }
    }
}