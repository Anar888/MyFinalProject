using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Helpers
{
    public class FileManager
    {
        public static string Save(string rootPath, string folderPath, IFormFile file)
        {
            string newFileName = Guid.NewGuid().ToString() + file.FileName;
            string path = Path.Combine(rootPath, folderPath, newFileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return newFileName;
        }

        public static bool Delete(string rootPath, string folderPath, string imgName)
        {
            string path = Path.Combine(rootPath, folderPath, imgName);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }
    }
}
