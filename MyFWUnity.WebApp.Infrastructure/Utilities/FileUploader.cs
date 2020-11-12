using MyFWUnity.Common;
using MyFWUnity.Common.Module;
using MyFWUnity.WebApp.Infrastructure.Model.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MyFWUnity.WebApp.Infrastructure.Utilities
{
    public class FileUploader
    {
        public static void GetUploadedFileInfo()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpFileCollection fileCollection = request.Files;
            string batchUploadID = request.Form["BatchUploadID"];
            string guid = request.Form["guid"];
            string fileTempRelativeFolder = guid;
            if (!string.IsNullOrEmpty(batchUploadID))
            {
                fileTempRelativeFolder = Path.Combine(batchUploadID, guid);
            }
            var file = fileCollection[0];
            if (file.ContentLength != Convert.ToInt32(request.Form["size"]))
            {
                throw new Exception("");
            }
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/document");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileTempFullFolder = Path.Combine(path, fileTempRelativeFolder);

            if (!Directory.Exists(fileTempFullFolder))
            {
                Directory.CreateDirectory(fileTempFullFolder);
            }

            // Save party file
            string filePartName = request.Form["fileName"].Trim() + request.Form["part"];
            string filePartFullPath = Path.Combine(fileTempFullFolder, filePartName);
            file.SaveAs(filePartFullPath);
            //string md5=  GetFileMD5.GetMD5HashFromFile(file.InputStream);
            // if (md5 != GetFileMD5.GetMD5HashFromFile(file.InputStream))
            // {
            //     return ResultJson.BuildJsonResponse(new { success = false });
            // }
            if (request.Form["isLast"] == "true")
            {
                string tempFilePath = Path.Combine(fileTempFullFolder, Guid.NewGuid().ToString() + Path.GetExtension(request.Form["fileName"]));

                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }

                string[] allPartyFiles = null;
                DateTime datetime = DateTime.Now;
                bool isall = true;
                while (isall)
                {
                    allPartyFiles = Directory.GetFiles(fileTempFullFolder, request.Form["fileName"].Trim() + "*");
                    if (allPartyFiles.Count() == Convert.ToInt32(request.Form["allfilesCount"]) || datetime.AddMinutes(1) < DateTime.Now)
                    {
                        isall = false;
                    }
                }

                //创建空的文件流
                using (FileStream tempFile = new FileStream(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    Thread.Sleep(1000);

                    allPartyFiles = allPartyFiles != null ? allPartyFiles.OrderBy(s => int.Parse(Regex.Match(s, @"\d+$").Value)).ToArray() : new string[] { };

                    using (BinaryWriter bw = new BinaryWriter(tempFile))
                    {
                        for (int i = 0; i < allPartyFiles.Length; i++)
                        {
                            using (BinaryReader reader = new BinaryReader(File.OpenRead(allPartyFiles[i])))
                            {
                                byte[] data = new byte[4194304]; //流读取,缓存空间
                                int readLen = 0; //每次实际读取的字节大小
                                readLen = reader.Read(data, 0, data.Length);
                                bw.Write(data, 0, readLen);
                            }
                        }
                    }
                }
                for (int i = 0; i < allPartyFiles.Length; i++)
                {
                    File.Delete(allPartyFiles[i]);
                }
            }

        }
        public static IEnumerable<DocumentFileDataInfo> GetBacthUploadedTempFileInfos(string batchUploadID)
        {
            // lower file name to file info, one batch, one same file
            Dictionary<string, DocumentFileDataInfo> fileLowerNameToFiles = new Dictionary<string, DocumentFileDataInfo>();
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/document");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string tempRelativeParentFolder = Path.Combine(path, batchUploadID);
            string tempFullParentFolder = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, tempRelativeParentFolder);

            if (!Directory.Exists(tempFullParentFolder))
            {
                return fileLowerNameToFiles.Values;
            }

            DirectoryInfo parentDirInfo = new DirectoryInfo(tempFullParentFolder);
            // Each dir contains a temp uploaded file and dir is an original file upload ID

            Regex searchPartyPattern = new Regex(@"part\d+$", RegexOptions.IgnoreCase);
            foreach (DirectoryInfo subDir in parentDirInfo.GetDirectories())
            {
                string guid = subDir.Name;

                // Search file which is last created but not party
                // Current, only one file in the directory
                FileInfo uploadedFile = subDir.GetFiles("*", SearchOption.TopDirectoryOnly).Where(f => !searchPartyPattern.IsMatch(f.Name))
                                .OrderBy(f => f.CreationTime).LastOrDefault();

                if (uploadedFile != null)
                {
                    string directParentFolderName = uploadedFile.Directory.Name;
                    DocumentFileDataInfo fileInfo = new DocumentFileDataInfo()
                    {
                        BatchUploadID = batchUploadID,
                        UploadID = guid,
                        UploadedTime = uploadedFile.CreationTime,
                        UploadTempRelativePath = "/" + uploadedFile.FullName.Replace(System.AppDomain.CurrentDomain.BaseDirectory, "").Replace("\\", "/"),
                        Name = uploadedFile.Name,
                        Suffix = Path.GetExtension(uploadedFile.Name),
                        UploadTempPath = uploadedFile.FullName,
                        SizeDisplay = uploadedFile.Length.GetSizeDisplayAs(),
                    };

                    string lowerFileName = uploadedFile.Name.ToLower();
                    if (fileLowerNameToFiles.ContainsKey(lowerFileName))
                    {
                        DocumentFileDataInfo addedFileInfo = fileLowerNameToFiles[lowerFileName];
                        if (fileInfo.UploadedTime.CompareTo(addedFileInfo.UploadedTime) > 0)
                        {
                            fileLowerNameToFiles[lowerFileName] = fileInfo;
                        }
                        else
                        {
                            // Ignore previous uploaded same file
                            continue;
                        }
                    }
                    else
                    {
                        fileLowerNameToFiles[lowerFileName] = fileInfo;
                    }
                }
            }

            return fileLowerNameToFiles.Values;
        }

        public static void RemoveTempUploadedFileOfBatch(string batchUploadID, string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data/document");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string tempFullParentFolder = Path.Combine(path, batchUploadID);

            if (!Directory.Exists(tempFullParentFolder))
            {
                return;
            }

            DirectoryInfo parentDirInfo = new DirectoryInfo(tempFullParentFolder);
            FileInfo[] files = parentDirInfo.GetFiles(fileName, SearchOption.AllDirectories);
            // Remove the file's directory totally
            foreach (FileInfo file in files)
            {
                try
                {
                    if (file.Directory.Exists)
                    {
                        file.Directory.Delete(true);
                    }
                }
                catch (Exception ex)
                {
                    LogModule.Error(string.Format("Failed to delete temp file {0}", file.Name));
                }
            }
        }

    }
}
