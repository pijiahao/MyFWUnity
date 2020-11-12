using MyFWUnity.Common;
using MyFWUnity.Core.Model;
using MyFWUnity.WebApp.Infrastructure;
using MyFWUnity.WebApp.Infrastructure.Model.File;
using MyFWUnity.WebApp.Infrastructure.Model.ResultData;
using MyFWUnity.WebApp.Infrastructure.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyFWUnity.WebApp.WebAPI.APIController.File
{
    public class FileController : BaseApiController
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[ApiFilter(Permission = "UploadFile")]
        public HttpResponseMessage TempUploadBigFile()
        {
            return ReturnResult(() =>
            {
                FileUploader.GetUploadedFileInfo();
                return ResultJson.BuildJsonResponse(new { success = true });
            });
        }
        /// <summary>
        /// Upload file into temp folder and use later
        /// Used when create file and entity relationship
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetBatchUploadedTempFiles(string batchUploadID)
        {
            return ReturnResult(() =>
            {
                if (string.IsNullOrEmpty(batchUploadID))
                {
                    throw new ArgumentNullException("batchUploadID");
                }
                IEnumerable<DocumentFileDataInfo> fileInfos = FileUploader.GetBacthUploadedTempFileInfos(batchUploadID);
                return ResultJson.BuildJsonResponse(new { items = fileInfos });
            });
        }
        [HttpGet]
        public HttpResponseMessage UploadFiles(string batchUploadID, string enityID, string enityType)
        {
            return ReturnResult(() =>
            {
                IEnumerable<DocumentFileDataInfo> fileInfos = FileUploader.GetBacthUploadedTempFileInfos(batchUploadID);
                if (fileInfos.Count() > 0)
                {

                }
                return ResultJson.BuildEmptySuccessJsonResponse();
            });
        }

        [HttpGet]
        public HttpResponseMessage RemoveTempUploadedFileOfBatch(string batchUploadID, string fileName)
        {
            return ReturnResult(() =>
            {
                if (string.IsNullOrEmpty(batchUploadID))
                {
                    throw new ArgumentNullException("batchUploadID");
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ArgumentNullException("fileName");
                }
                FileUploader.RemoveTempUploadedFileOfBatch(batchUploadID, fileName);
                return ResultJson.BuildEmptySuccessJsonResponse();
            });
        }


        [HttpPut]
        public HttpResponseMessage UploadImage()
        {
            return ReturnResult(() =>
            {
                string errorInfo = string.Empty;
                string webImageUrl = string.Empty;
                if (UploadUtil.UploadImage("/data/image/", ref webImageUrl, ref errorInfo))
                {
                    webImageUrl = HttpContext.Current.Request.GetCurrentHttpUrl() + webImageUrl;
                    return ResultJson.BuildJsonResponse(new { success = true, url = webImageUrl }, MessageType.Information, errorInfo);
                }
                else
                {
                    return ResultJson.BuildJsonResponse(new { success = false }, MessageType.Information, errorInfo);
                }

            });
        }
        [HttpPost]
        public HttpResponseMessage UploadWebImage()
        {
            return ReturnResult(() =>
            {
                string errorInfo = string.Empty;
                string webImageUrl = string.Empty;
                if (UploadUtil.UploadImage("/data/image/", ref webImageUrl, ref errorInfo))
                {
                    webImageUrl = HttpContext.Current.Request.GetCurrentHttpUrl() + webImageUrl;
                    return ResultJson.BuildJsonResponse(new { success = true, url = webImageUrl }, MessageType.Information, errorInfo);
                }
                else
                {
                    return ResultJson.BuildJsonResponse(new { success = false }, MessageType.Error, errorInfo);
                }

            });
        }

        [HttpGet]
        public HttpResponseMessage DownloadByttachment(string id)
        {
            HttpResponseMessage result = null;

            AttachmentsDataInfo attachmentsDataInfo = SysService.GetAttachmentsById(id);
            if (attachmentsDataInfo != null)
            {
                if (!string.IsNullOrEmpty(attachmentsDataInfo.FilePath))
                {
                    result = new HttpResponseMessage(HttpStatusCode.OK);
                    result.Content = new StreamContent(new FileStream(attachmentsDataInfo.FilePath, FileMode.Open));
                    result.Content.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    result.Content.Headers.ContentDisposition.FileName = attachmentsDataInfo.Name;
                }
                else
                {
                    result = new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            else
            {
                result = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            return result;
        }

        #region upload image
        private void showError(string message)
        {
            HttpContext context = HttpContext.Current;
            Hashtable hash = new Hashtable();
            hash["error"] = 1;
            hash["message"] = message;
            context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
            context.Response.Write(hash.ToJson());
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
        public void UploadImageByKindEditor()
        {

            HttpContext context = HttpContext.Current;

            //文件保存目录路径
            String savePath = "/Content/Plugins/KindEditor/attached/";

            //文件保存目录URL
            String saveUrl = "/Content/Plugins/KindEditor/attached/";

            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            extTable.Add("flash", "swf,flv");
            extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //最大文件大小
            int maxSize = 1000000;

            HttpPostedFile imgFile = context.Request.Files["imgFile"];
            if (imgFile == null)
            {
                showError("请选择文件。");
            }

            String dirPath = context.Server.MapPath(savePath);
            //if (!Directory.Exists(dirPath))
            //{
            //    showError("上传目录不存在。");
            //}

            String dirName = context.Request.QueryString["dir"];
            if (String.IsNullOrEmpty(dirName))
            {
                dirName = "image";
            }
            if (!extTable.ContainsKey(dirName))
            {
                showError("目录名不正确。");
            }

            String fileName = imgFile.FileName;
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                showError("上传文件大小超过限制。");
            }

            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                showError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable[dirName]) + "格式。");
            }

            //创建文件夹
            dirPath += dirName + "/";
            saveUrl += dirName + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            String ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            dirPath += ymd + "/";
            saveUrl += ymd + "/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            String filePath = dirPath + newFileName;

            imgFile.SaveAs(filePath);

            String fileUrl = saveUrl + newFileName;

            Hashtable hash = new Hashtable();
            hash["error"] = 0;
            hash["url"] = fileUrl;
            context.Response.Write(hash.ToJson());
            context.Response.End();
        }
        #endregion

    }
}
