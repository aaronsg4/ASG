using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ASG.Areas.BugTracker.Models
{
        public class FileUploadValidator
        {
            public static bool IsWebFriendlyFile(HttpPostedFileBase File)
            {
                //check for actual object
                if (File == null)
                    return false;
                //check size - file must be less than 2 mb and greater that 1 kb
                if (File.ContentLength > 2 * 1024 * 1024 || File.ContentLength < 1024)
                    return false;
                try
                {
                    var uploadedFileName = File.FileName;
                    var extension = Path.GetExtension(uploadedFileName);

                    return extension.Equals(".docx") || extension.Equals(".xls") || extension.Equals(".pdf") || extension.Equals(".jpeg") || extension.Equals(".bmp") || extension.Equals(".gif")
                        || extension.Equals(".jpg") || extension.Equals(".zip") || extension.Equals(".rar") || extension.Equals(".pdf");



                }
                catch
                {
                    return false;
                }
            }
        }
    }
