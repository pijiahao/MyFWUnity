using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFWUnity.WebApp.Infrastructure.Model.File
{
    public class DocumentFileDataInfo
    {
        public string Status { get; set; }
        public string Tags { get; set; }
        public List<int> NotificationUserId { get; set; }
        public int ProjectID { get; set; }
        public bool RequireTransform { get; set; }
        public bool SupportDocNumber { get; set; }
        public int CreationUserID { get; set; }
        public string ClientRelativePath { get; set; }
        public string Name { get; set; }
        public long FolderID { get; set; }
        public string SizeDisplay { get; set; }
        public string UploadTempRelativePath { get; set; }
        public string UploadTempPath { get; set; }
        public DateTime UploadedTime { get; set; }
        public string UploadID { get; set; }
        public string BatchUploadID { get; set; }
        public string Suffix { get; set; }
        public string VerifiedPermissionName { get; set; }
    }
}
