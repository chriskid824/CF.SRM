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
        public Task<string> UploadAsync(FileUploadViewModel viewModel);
        public Task<string> UploadPoAsync(FileUploadViewModel viewModel);
        public Task<Stream> DownloadAsync(NzFileViewModel viewModel);
        public Task<Stream> DownloadPoFileAsync(NzFileViewModel viewModel);
        public Task<bool> DeleteFileAsync(NzFileViewModel viewModel);
        public IEnumerable<AnnouncementType> GetAnnList(QueryFile query);
        public string UpdateNumber(string number, string guid);
        public bool HasTemplate(int werks, int functionId,int type);
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
        private readonly ISrmQotService _srmQotService;
        private readonly ISrmRfqHService _srmRfqHService;

        public SrmFileService(
            //IMapper mapper,
            IRepository<SrmPoH> srmPohRepository, IRepository<SrmPoL> srmPolRepository, SRMContext context, IMapper mapper, IFileStore fileStore
            , IOptions<appSettings> appSettingsOption, ISrmPoService srmPoService, ISrmQotService srmQotService, ISrmRfqHService srmRfqHService)
        {
            _mapper = mapper;
            _srmPohRepository = srmPohRepository;
            _srmPolRepository = srmPolRepository;
            _context = context;
            _fileStore = fileStore;
            _appSettingsService = appSettingsOption.Value;
            _srmPoService = srmPoService;
            _srmQotService = srmQotService;
            _srmRfqHService = srmRfqHService;
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
                    //RecordLId=p.RecordLId,
                    //Url=p.Url,
                    //CreateBy=p.CreateBy,
                    //CreateDate=p.CreateDate,
                    //LastUpdateBy=p.LastUpdateBy,
                    //LastUpdateDate=p.LastUpdateDate
                }).Distinct()
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
                    List<SrmFileuploadRecordL> RecordLList = _context.SrmFileuploadRecordLs.Where(l => l.RecordHId == p.RecordHId && l.Filetype.ToString() == p.Filetype).ToList();
                    List<NzFileViewModel> filelist = new List<NzFileViewModel>();
                    RecordLList.ForEach(l => {

                        NzFileViewModel file = new NzFileViewModel()
                        {
                            Uid = l.RecordLId,
                            Name = l.Url.Substring(l.Url.LastIndexOf('/') + 1, l.Url.Length - l.Url.LastIndexOf('/') - 1),
                            Status = "done"
                        };
                        filelist.Add(file);

                    });
                    p.fileList = filelist;
                    //if (p.RecordLId.HasValue)
                    //{
                    //    List<NzFileViewModel> filelist = new List<NzFileViewModel>();
                    //    NzFileViewModel file = new NzFileViewModel()
                    //    {
                    //        Uid = p.RecordLId.Value,
                    //        Name = p.Url.Substring(p.Url.LastIndexOf('/') + 1, p.Url.Length - p.Url.LastIndexOf('/') - 1),
                    //        Status = "done"
                    //    };
                    //    filelist.Add(file);
                    //    p.fileList = filelist;
                    //}
                });
            }
            return result.OrderBy(p=>p.Filetype);
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
                string typename = _context.SrmFileTypeProfiles.Where(p => p.TypeId == viewModel.fileTypeList[count]).Select(p => p.TypeName).FirstOrDefault();
                var path =  viewModel.file.Werks.ToString()+'/'+viewModel.file.Number+'/'+ (viewModel.file.Type==1? "廠內" : "廠外")+'/'+ typename;
                ////判斷檔案路徑是否存在，不存在則建立資料夾
                //if (!System.IO.Directory.Exists(path))
                //{
                //    System.IO.Directory.CreateDirectory(path);//不存在就建立目錄
                //}
                path= path + '/' + file.FileName;
                var info = await _fileStore.GetFileInfoAsync(path);
                if (info != null)
                {
                    FileInfo fileinfo =  new FileInfo(_fileStore.GetPhysicalPath(path));
                    fileinfo.Delete();
                   // return "文件名冲突！";
                }

                var stream = file.OpenReadStream();
                var result = await _fileStore.CreateFileFromStreamAsync(path, stream);
                if (string.IsNullOrEmpty(result))
                {
                    return "文件上传失败！";
                }

                SrmFileuploadRecordL rl = new SrmFileuploadRecordL();
                if (_context.SrmFileuploadRecordLs.Where(p => p.Filetype == viewModel.fileTypeList[count] && p.RecordHId == viewModel.file.RecordHId && p.Url == path).Count() > 0)
                {
                    rl = _context.SrmFileuploadRecordLs.Where(p => p.Filetype == viewModel.fileTypeList[count] && p.RecordHId == viewModel.file.RecordHId && p.Url == path).FirstOrDefault();
                    rl.LastUpdateBy = viewModel.user.UserName;
                    rl.LastUpdateDate = DateTime.Now;
                    //rl.Url = path;
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

        public async Task<string> UploadPoAsync(FileUploadViewModel viewModel)
        {

            foreach (var file in viewModel.Files)
            {
                var path = "PoFiles/" + viewModel.number;
                ////判斷檔案路徑是否存在，不存在則建立資料夾
                if (System.IO.Directory.Exists(_fileStore.GetPhysicalPath(path)))
                {
                    //System.IO.Directory.CreateDirectory(path);//不存在就建立目錄
                    DirectoryInfo di = new DirectoryInfo(_fileStore.GetPhysicalPath(path));
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo deletefile in files)
                    {
                        deletefile.Delete();
                    }
                }
                path = path + '/' + file.FileName;
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
            }
            return string.Empty;
        }

        public async Task<Stream> DownloadAsync(NzFileViewModel viewModel)
        {
            SrmFileuploadRecordL rl= _context.SrmFileuploadRecordLs.Find(viewModel.Uid);
            return await _fileStore.GetFileStreamAsync(rl.Url);
        }
        public async Task<Stream> DownloadPoFileAsync(NzFileViewModel viewModel)
        {
            return await _fileStore.GetFileStreamAsync("/PoFiles/" + viewModel.Uid+"/"+ viewModel.Name);
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
            if (!query.user.GetIsVendor())
            {
                result.Add(GetAnn3(query));
            }
            result.Add(GetAnn4(query));


            //result.Add(GetAnn5(query));
            //result.Add(GetAnn6(query));

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
            //q.dataStatus = 2;
            q.user = query.user;
            List<int> statusarr = new List<int>() { 11,21 };
            var aaa = _srmPoService.GetAll(q).Where(p=> statusarr.Contains(p.Status.Value)).OrderBy(p=>p.CreateDate);
            ann.NumberList = aaa.Select(p => new AnnouncementDetail() { id=p.PoId.ToString(),number=p.PoNum,status=p.Status.Value} ).ToList();
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
                Router = "rfq",
            };
            QueryRfqList q = new QueryRfqList();
            UserClaims user = query.user.GetUserClaims();
            q.werks = user.Werks;
            int page = 1;
            int size = 50;
            List<int> statusarr = new List<int>() { 18, 19, 20 };
            var aaa = _srmRfqHService.GetRfqList(q, page, size).Data.Where(p=> !statusarr.Contains(p.Status.Value));
            ann.NumberList = aaa.Select(p => new AnnouncementDetail() { id = p.RfqId.ToString(), number = p.RfqNum, status = p.Status.Value }).ToList();
            //new AnnouncementDetail() { id = p.PoId.ToString(), number = p.PoNum, status = p.Status.Value, statusDesc = p.StatusDesc }
            return ann;
        }
        public AnnouncementType GetAnn4(QueryFile query)
        {
            AnnouncementType ann = new AnnouncementType()
            {
                Stylecolor = "green",
                TxtTypeName = "報價單",
                Icon = "mdi-library",
                Router = "qotlist",
            };


            //qot.matnr = query.user.GetUserWerks();


            if (query.user.GetIsVendor())
            {
                QueryQotList qot = new QueryQotList();
                qot.status = 1;//(int)query["status"];
                qot.vendor = query.user.GetVendorId();
                var result = _srmQotService.GetQotList(qot).Where(p=> p.SrmQotHs!=null&&p.SrmQotHs.Count!=0).OrderBy(p => p.VDeadline).ToList();
                if (result != null)
                {
                    ann.NumberList = result.Select(p => new AnnouncementDetail() { id = p.VRfqId.ToString(), number = p.VRfqNum, status = p.VStatus.Value }).ToList();
                }
            }
            else
            {
                QueryQotList qot = new QueryQotList();
                qot.status = 1;//(int)query["status"];
                var result = _srmQotService.GetQotListByAdmin(qot).Where(p => p.SrmQotHs != null && p.SrmQotHs.Count != 0).OrderBy(p=>p.VDeadline).ToList();
                if (result != null)
                {
                    ann.NumberList = result.Select(p => new AnnouncementDetail() { id = p.VRfqId.ToString(), number = p.VRfqNum, status = p.VStatus.Value }).ToList();
                }
                //ann.NumberList = result.Select(p => p).ToList();
            }
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
        public string UpdateNumber(string number, string guid)
        {
            if(number==guid) return string.Empty;
            List<SrmFileuploadRecordH> hList = _context.SrmFileuploadRecordHs.Where(p => p.Number == guid).ToList();
            if (hList.Count <= 0) { return "不存在該guid"; }
            hList.ForEach(p => {
                string werks = _context.SrmFileUploadTemplates.Where(t => t.TemplateId == p.TemplateId).FirstOrDefault().Werks.ToString();
                string srcFolderPath =  werks + '/' + guid;
                srcFolderPath = _fileStore.GetPhysicalPath(srcFolderPath);
                string destFolderPath = werks + '/' + number;
                destFolderPath= _fileStore.GetPhysicalPath(destFolderPath);
                if (System.IO.Directory.Exists(srcFolderPath))
                {
                    System.IO.Directory.Move(srcFolderPath, destFolderPath);
                }
                p.Number = number;
                List<SrmFileuploadRecordL> lList = _context.SrmFileuploadRecordLs.Where(l => l.RecordHId == p.RecordHId).ToList();
                lList.ForEach(l => { l.Url=l.Url.Replace(guid, number); });
            });

            _context.SaveChanges();
            return string.Empty;
        }
        public bool HasTemplate(int werks, int functionId, int type)
        {
            return _context.SrmFileUploadTemplates.Any(p => p.Werks == werks && p.TemplateType == functionId && p.Type == type);
        }
    }
}