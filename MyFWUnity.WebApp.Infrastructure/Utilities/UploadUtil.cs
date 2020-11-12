using MyFWUnity.Common;
using MyFWUnity.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MyFWUnity.WebApp.Infrastructure.Utilities
{
    public class UploadUtil
    {
        public static List<AttachmentsDataInfo> GetAttachmentsDataInfos()
        {
            List<AttachmentsDataInfo> attachmentsDataInfos = new List<AttachmentsDataInfo>(); ;
            Dictionary<string, string> fileList = new Dictionary<string, string>();
            string errorInfo = string.Empty;
            if (!AttachmentsUploadSmallFile("/data/files/", ref attachmentsDataInfos, ref errorInfo))
            {
                attachmentsDataInfos = null;
            }

            return attachmentsDataInfos;
        }

        public static bool AttachmentsUploadSmallFile(string folderPath, ref List<AttachmentsDataInfo> filePaths, ref string errorInfo)
        {
            bool result = true;
            try
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                HttpFileCollection fileCollection = request.Files;
                if (fileCollection.Count > 0)
                {
                    folderPath += DateTime.Now.ToString("yyyy-MM-dd");
                    string tempFolderPath = HttpContext.Current.Server.MapPath("~" + folderPath);
                    if (!Directory.Exists(tempFolderPath))
                    {
                        Directory.CreateDirectory(tempFolderPath);
                    }
                    for (int i = 0; i < fileCollection.Count; i++)
                    {
                        string suffix = Path.GetExtension(fileCollection[i].FileName).ToLower();
                        var stream = fileCollection[i].InputStream;
                        string fileName = Guid.NewGuid().ToString("N") + suffix;
                        string filePath = Path.Combine(tempFolderPath, fileName);
                        if (!File.Exists(filePath))
                        {
                            byte[] myByte = stream.StreamToBytes();
                            using (var fileimage = File.Create(filePath))
                            {
                                fileimage.Write(myByte, 0, myByte.Length);
                            };
                            filePaths.Add(new AttachmentsDataInfo() { Name = Path.GetFileName(fileCollection[i].FileName), FilePath = filePath, WebUrl = request.GetCurrentHttpUrl() + folderPath+"/" + fileName });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                result = false;
                errorInfo = ex.Message;
            }
            return result;
        }

        public static bool UploadImage(string folderPath, ref string webImageUrl, ref string errorInfo)
        {
            bool result = true;
            try
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                HttpFileCollection fileCollection = request.Files;
                if (fileCollection.Count > 0)
                {
                    string suffix = Path.GetExtension(fileCollection[0].FileName).ToLower();
                    if (string.Equals(suffix, ".jpg") || string.Equals(suffix, ".gif") || string.Equals(suffix, ".png") || string.Equals(suffix, ".pdf"))
                    {
                        var stream = fileCollection[0].InputStream;
                        folderPath += "/" + DateTime.Now.ToString("yyyy-MM-dd");
                        string imgTempFolderPath = HttpContext.Current.Server.MapPath("~" + folderPath);
                        if (!Directory.Exists(imgTempFolderPath))
                        {
                            Directory.CreateDirectory(imgTempFolderPath);
                        }
                        string fileName = Guid.NewGuid().ToString("N") + suffix;
                        string imagePath = Path.Combine(imgTempFolderPath, fileName);
                        if (!File.Exists(imagePath))
                        {
                            byte[] myByte = stream.StreamToBytes();
                            using (var fileimage = File.Create(imagePath))
                            {
                                fileimage.Write(myByte, 0, myByte.Length);
                            };
                            webImageUrl = Path.Combine(folderPath, fileName);
                        }
                    }
                    else
                    {
                        errorInfo = "请上传图片格式（.jpg，.gif，.png，.pdf）";
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                errorInfo = ex.Message;
            }
            return result;
        }

        public static string UploadImage(string folderPath, ref string errorInfo)
        {
            string webImageUrl = string.Empty;
            try
            {
                HttpRequest request = System.Web.HttpContext.Current.Request;
                HttpFileCollection fileCollection = request.Files;
                if (fileCollection.Count > 0)
                {
                    string suffix = Path.GetExtension(fileCollection[0].FileName).ToLower();
                    if (string.Equals(suffix, ".jpg") || string.Equals(suffix, ".gif") || string.Equals(suffix, ".png") || string.Equals(suffix, ".pdf"))
                    {
                        var stream = fileCollection[0].InputStream;
                        string imgTempFolderPath = HttpContext.Current.Server.MapPath("~" + folderPath);
                        if (!Directory.Exists(imgTempFolderPath))
                        {
                            Directory.CreateDirectory(imgTempFolderPath);
                        }
                        Random rd = new Random();
                        string fileName = rd.Next(10, 100).ToString() + "_" + fileCollection[0].FileName;
                        string imagePath = Path.Combine(imgTempFolderPath, fileName);
                        if (!File.Exists(imagePath))
                        {
                            byte[] myByte = stream.StreamToBytes();
                            using (var fileimage = File.Create(imagePath))
                            {
                                fileimage.Write(myByte, 0, myByte.Length);
                            };
                            webImageUrl = Path.Combine(folderPath, fileName);
                        }
                    }
                    else
                    {
                        errorInfo = "请上传图片格式（.jpg，.gif，.png，.pdf）";
                    }
                }
            }
            catch (Exception ex)
            {
                errorInfo = ex.Message;
            }
            return webImageUrl;
        }
    }
}
