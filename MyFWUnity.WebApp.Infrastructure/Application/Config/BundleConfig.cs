using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace MyFWUnity.WebApp.Infrastructure.Application.Config
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Layout
            bundles.Add(new ScriptBundle("~/Layout/js").Include(
                        "~/Content/Plugins/jQuery/jQuery-v2.1.0.js",
                        "~/Content/Plugins/jQuery/jquery-validate/jquery-validate.min.js",
                         "~/Content/Plugins/jQuery/jquery-validate/messages_zh.js",
                        "~/Content/Plugins/bootstrap/dist/js/bootstrap.min.js",
                        "~/Content/Plugins/bootstrap-table/bootstrap-table.js",
                        "~/Content/Plugins/bootstrap-select/bootstrap-select.js",
                        "~/Content/Plugins/sweetalert/sweetalert.js",
                        "~/Content/Js/AjaxCustom.js",
                        "~/Content/Js/Common.js"
                        ));

            bundles.Add(new StyleBundle("~/Layout/css").Include(
                      "~/Content/Plugins/bootstrap/dist/css/bootstrap.min.css",
                      "~/Content/Plugins/bootstrap-select/bootstrap-select.css",
                     "~/Content/Plugins/sweetalert/sweetalert.css"
                      ));
            #endregion

            #region 自定义上传控件
            bundles.Add(new ScriptBundle("~/Content/UploadFile/js").Include(
               "~/Content/Plugins/UploadFile/uploadfile.js"
            ));
            bundles.Add(new StyleBundle("~/Content/UploadFile/css").Include(
                  "~/Content/Plugins/UploadFile/fileinput.css",
                  "~/Content/Plugins/UploadFile/uploadfile.css"
                ));
            #endregion
        }
    }
}
