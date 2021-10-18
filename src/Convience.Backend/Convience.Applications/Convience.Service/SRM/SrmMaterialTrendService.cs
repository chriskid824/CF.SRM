using Convience.Entity.Entity.SRM;
using Convience.Filestorage.Abstraction;
using Convience.Model.Models;
using Convience.Model.Models.SRM;
using Convience.Util.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Convience.Service.SRM
{
    public interface ISrmMaterialTrendService
    {
        public string UploadAsync(FileUploadViewModel viewModel);
        public PagingResultModel<ViewSrmMaterialTrend> GetMaterialTrendList(QuerySrmMaterialTrend query);
        public PagingResultModel<SrmMaterial> GetMaterialList(QuerySrmMaterial query);
        public string AddMaterial(SrmMaterial material);
    }
    public class SrmMaterialTrendService: ISrmMaterialTrendService
    {
        private readonly SRMContext _context;
        private readonly IFileStore _fileStore;
        public SrmMaterialTrendService( SRMContext context, IFileStore fileStore)
        {
            _context = context;
            _fileStore = fileStore;
        }
        #region upload
        public string UploadAsync(FileUploadViewModel viewModel)
        {
            if (viewModel.EffectiveDate > viewModel.Deadline)
            {
                return "生效日期需小於等於截止日期";
            }
            foreach (var file in viewModel.Files)
            {
                Guid g = Guid.NewGuid();
                var path = viewModel.CurrentDirectory?.TrimEnd('/') + '/' + viewModel.Material + '/' + g + Path.GetExtension(file.FileName);
                switch (Path.GetExtension(file.FileName).ToLower())
                {
                    case ".png":
                    case ".jpg":
                        break;
                    default:
                        return "限定圖檔(png,jpg)！";
                }
                var info = GetFileInfoAsync(path);
                if (info != null)
                {
                    return "文件名重複！";
                }
                var stream = file.OpenReadStream();
                var result = CreateFileFromStreamAsync(path, stream);
                if (string.IsNullOrEmpty(result))
                {
                    return "文件上傳失敗！";
                }
                viewModel.ImageUrl = "/assets/material-trend/" + viewModel.Material + '/' + new FileInfo(path).Name;
                viewModel.CreateDate = DateTime.Now;
                AddSRM_MATERIAL_TREND(viewModel);
            }
            return string.Empty;
        }
        public FileInfo GetFileInfoAsync(string path)
        {
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                return fileInfo;
            }

            return null;
        }
        public string CreateFileFromStreamAsync(string path, Stream inputStream, bool overwrite = false)
        {
            if (!overwrite && File.Exists(path))
            {
                throw new FileStoreException($"Cannot create file '{path}' because it already exists.");
            }

            if (Directory.Exists(path))
            {
                throw new FileStoreException($"Cannot create file '{path}' because it already exists as a directory.");
            }

            // Create directory path if it doesn't exist.
            var physicalDirectoryPath = Path.GetDirectoryName(path);
            Directory.CreateDirectory(physicalDirectoryPath);

            var fileInfo = new FileInfo(path);
            using (var outputStream = fileInfo.Create())
            {
                inputStream.CopyTo(outputStream);
            }
            return path;
        }
        #endregion upload
        public void AddSRM_MATERIAL_TREND(SrmMaterialTrend materialTrend)
        {
            _context.SrmMaterialTrends.Add(materialTrend);
            _context.SaveChanges();
        }

        public PagingResultModel<ViewSrmMaterialTrend> GetMaterialTrendList(QuerySrmMaterialTrend query)
        {
            int skip = (query.Page - 1) * query.Size;

            var resultQuery = (from materialTrend in _context.SrmMaterialTrends
                               join user in _context.AspNetUsers
                               on materialTrend.CreateBy equals user.UserName
                               select new ViewSrmMaterialTrend(materialTrend)
                               {
                                   Material = materialTrend.Material,
                                   EffectiveDate = materialTrend.EffectiveDate,
                                   Deadline = materialTrend.Deadline,
                                   CreateDate = materialTrend.CreateDate,
                                   CreateName = user.Name
                               })
                .AndIfHaveValue(query.materialTrend.Material, r => r.Material.Contains(query.materialTrend.Material))
                .AndIfHaveValue(query.searchDate, r => r.EffectiveDate <= query.searchDate.Value && r.Deadline >= query.searchDate)
                .OrderByDescending(r => r.CreateDate);

            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<ViewSrmMaterialTrend>
            {
                Data = JsonConvert.DeserializeObject<ViewSrmMaterialTrend[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
        public PagingResultModel<SrmMaterial> GetMaterialList(QuerySrmMaterial query) {
            int skip = (query.Page - 1) * query.Size;
            var resultQuery = (from material in _context.SrmMaterials
                               select material)
                .AndIfHaveValue(query.material.Material, r => r.Material.Contains(query.material.Material));
            var materials = resultQuery.Skip(skip).Take(query.Size).ToArray();

            return new PagingResultModel<SrmMaterial>
            {
                Data = JsonConvert.DeserializeObject<SrmMaterial[]>(JsonConvert.SerializeObject(materials)),
                Count = resultQuery.Count()
            };
        }
        public string AddMaterial(SrmMaterial material) {
            material.Material = material.Material.Trim();
            if (_context.SrmMaterials.Where(r => r.Material.Equals(material.Material)).Any()) {
                return ($"{material.Material}已存在");
            }
            material.Staus = (int)Status.已核發;
            _context.Add(material);
            _context.SaveChanges();
            return "";
        }
    }
}
