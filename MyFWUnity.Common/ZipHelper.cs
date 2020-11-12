using MyFWUnity.Common.Module;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.Common
{
    // Refer to: 
    // 1. http://www.cnblogs.com/leco/archive/2010/11/18/1881301.html
    // 2. http://blog.sina.com.cn/s/blog_8c32cdb401013jct.html

    // Comments:
    // 该方法调用时，文件的路径必须为绝对路径，并且格式为x:\\xx\\……
    // string[] paths = new string[] { "D:\\testFolder\\folder2", "D:\\testFolder\\c#压缩解压.doc", "D:\\wode.doc" };
    // string error = "";
    // 打包
    // ZipHelper.Pack(paths, "c:\\hi0.zip", 6, "123", out error);
    // 解包
    // ZipHelper.Unpack("c:\\hi0.zip","c:\\hi","123",out error);

    public class ZipHelper
    {
        //public static bool Pack(List<string[]> filesOrDirectoriesPaths, string strZipPath, out string error)
        //{
        //    return Pack(filesOrDirectoriesPaths, strZipPath, 6, string.Empty, out error);
        //}

        /// <summary>
        ///  将多个文件或文件夹打包
        /// </summary>
        /// <param name="filesOrDirectoriesPaths">被打包文件的路径</param>
        /// <param name="strZipPath">打包后存放的路径</param>
        /// <param name="intZipLevel">打包压缩级别0-9(0为不压缩)</param>
        /// <param name="strPassword">打包密码</param>
        /// <param name="error">打包过程中的错误信息</param>
        /// <returns>是否打包成功</returns>
        public static bool Pack(List<string[]> filesOrDirectoriesPaths, string strZipPath, int intZipLevel,
            string strPassword, out string error)
        {
            // ZIP文件路径是否存在
            if (!Directory.Exists(strZipPath.Substring(0, strZipPath.LastIndexOf("\\") + 1)))
            {
                error = "请检查指定的压缩ZIP文件路径是否存在！";
                return false;
            }

            //List<string[]> files = new List<string[]>();

            //if (filesOrDirectoriesPaths.Count() > 0) // get all files path
            //{
            //    foreach (string[] fileName in filesOrDirectoriesPaths)
            //    {
            //        if (File.Exists(fileName[0]))
            //        {
            //            files.Add(new string[] { fileName[0], fileName[1] });
            //        }
            //        else if (Directory.Exists(fileName[0]))
            //        {
            //            string root = fileName[1];
            //            GetAllDirectories(fileName[0], root, ref files);
            //        }
            //        else
            //        {
            //            error = string.Format("请检查文件路径，文件夹或文件: {} 不存在！！！", fileName);
            //            return false;
            //        }
            //    }
            //}

            //执行压包操作
            ZipOutputStream zipOutputStream = null;
            try
            {
                // ZIP文件是否已存在
                if (File.Exists(strZipPath))
                {
                    File.Delete(strZipPath);
                }

                zipOutputStream = new ZipOutputStream(File.Create(strZipPath));
                zipOutputStream.SetLevel(intZipLevel);
                if (strPassword != string.Empty)
                {
                    zipOutputStream.Password = strPassword;
                }

                foreach (string[] strFile in filesOrDirectoriesPaths)
                {
                    try
                    {
                        FileStream fs = File.OpenRead(strFile[0]);

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        //string strFileName = strFile[0].Replace(strFile[1], String.Empty);

                        ZipEntry entry = new ZipEntry(strFile[1]);
                        entry.DateTime = DateTime.Now;
                        zipOutputStream.PutNextEntry(entry);
                        zipOutputStream.Write(buffer, 0, buffer.Length);

                        fs.Close();
                        fs.Dispose();
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                        LogModule.Error("Failed to zip file", ex);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
            finally
            {
                if (null != zipOutputStream)
                {
                    zipOutputStream.Finish();
                    zipOutputStream.Close();
                }
            }

            error = "";
            return true;
        }


        ///<summary>
        ///实现解包操作
        ///</summary>
        ///<param name="zipfilename">要解包文件(物理路径)</param>
        ///<param name="UnZipDir">解包目的路径(物理路径)</param>
        ///<param name="password">解包密码</param>
        /// <param name="error">异常信息</param>
        ///<returns>是否解包成功</returns>
        public static bool Unpack(string zipfilename, string UnZipDir, string password, out string error)
        {
            //判断待解包文件路径
            if (!File.Exists(zipfilename))
            {
                File.Delete(UnZipDir);
                error = "待解包文件路径不存在!";
                return false;
            }
            //创建ZipInputStream
            ZipInputStream newinStream = new ZipInputStream(File.OpenRead(zipfilename));

            //判断Password
            if (password != null && password.Length > 0)
            {
                newinStream.Password = password;
            }
            //执行解包操作
            try
            {
                ZipEntry theEntry;

                //获取Zip中单个File
                while ((theEntry = newinStream.GetNextEntry()) != null)
                {
                    //判断目的路径
                    if (Directory.Exists(UnZipDir))
                    {
                        Directory.CreateDirectory(UnZipDir);//创建目的目录
                    }
                    //获得目的目录信息
                    string Driectoryname = UnZipDir;
                    string pathname = Path.GetDirectoryName(theEntry.Name);//获得子级目录
                    string filename = Path.GetFileName(theEntry.Name);//获得子集文件名

                    Driectoryname = Driectoryname + "\\" + pathname;
                    //创建
                    Directory.CreateDirectory(Driectoryname);
                    //解包指定子目录
                    if (filename != string.Empty)
                    {
                        FileStream newstream = File.Create(Driectoryname + "\\" + filename);// pathname);
                        int size = 2048;
                        byte[] newbyte = new byte[size];
                        while (true)
                        {
                            size = newinStream.Read(newbyte, 0, newbyte.Length);
                            if (size > 0)
                            {
                                //写入数据
                                newstream.Write(newbyte, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }

                        newstream.Close();
                    }
                }
                newinStream.Close();
            }
            catch (Exception se)
            {
                error = se.Message.ToString();
                return false;
            }
            finally
            {
                newinStream.Close();
            }

            error = "";
            return true;
        }

        /// <summary>
        /// 取得目录下所有文件及文件夹，分别存入files及paths
        /// </summary>
        /// <param name="rootPath">根目录</param>
        public static void GetAllDirectories(string path, string name, ref List<string[]> files)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] arDirectory = directory.GetDirectories();
            foreach (DirectoryInfo d in arDirectory)
            {
                files.Add(new string[] { d.FullName, d.Name });
                GetAllDirectories(d.FullName, d.Name, ref files);
            }
            FileInfo[] arFile = directory.GetFiles();
            foreach (FileInfo f in arFile)
            {
                files.Add(new string[] { f.FullName, f.Name });
            }
        }

        /// <summary>
        /// 打包文件夹
        /// 快速压缩目录，文件没有被压缩
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="folderPath"></param>
        public static void PackFolder(string zipPath, string folderPath)
        {
            (new FastZip()).CreateZip(zipPath, folderPath, true, "");
        }

    }
}
