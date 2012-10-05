using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carcass.Common.Utility;
namespace Carcass.Common.MVC.DataTypes
{
    public class PostedFile
    {
        public PostedFile(System.Web.HttpPostedFileBase file)
        {
            Throw.IfNullArgument(file, "file");

            ContentSize = file.ContentLength;
            ContentType = file.ContentType;
            FileName = Path.GetFileName(file.FileName);
            FileDirectory = Path.GetDirectoryName(file.FileName);
            InputStream = file.InputStream;
        }

        /// <summary>
        /// The length of the file, in bytes.
        /// </summary>
        public int ContentSize { get; set; }
        
        /// <summary>
        /// The MIME content type of the file.
        /// </summary>
        public string ContentType { get; set; }
     
        /// <summary>
        /// The name of the file on the client, which includes the directory path if it provided by client.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// File directory path if it provided by client.
        /// </summary>
        public string FileDirectory { get; set; }
        
        /// <summary>
        /// An object for reading a file.
        /// </summary>
        public Stream InputStream { get; set; }
    }
}
