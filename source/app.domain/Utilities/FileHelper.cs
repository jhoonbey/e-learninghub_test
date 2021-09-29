using System;
using System.IO;

namespace app.domain.Utilities
{
    public static class FileHelper
    {
        public static string PrepareTarget(string size, string folderName, string pathOnly, string newFileName)
        {
            string targetFolder = Path.Combine(Path.Combine(pathOnly, folderName), size);
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            return Path.Combine(targetFolder, newFileName);
        }

        public static void BackupAndRemove(string size, string folderName, string pathOnly, string fileName)
        {
            //from
            //folder
            string targetFolder_From = Path.Combine(Path.Combine(pathOnly, folderName), size);
            //file
            string targetFolder_From_File = Path.Combine(targetFolder_From, fileName);

            //to
            //folder
            string targetFolder_To = Path.Combine(Path.Combine(pathOnly, folderName), "Backup/" + size);
            if (!Directory.Exists(targetFolder_To)) Directory.CreateDirectory(targetFolder_To);

            //file
            string targetFolder_To_File = Path.Combine(targetFolder_To, fileName);
            if (File.Exists(targetFolder_To_File))
            {
                targetFolder_To_File = Path.Combine(targetFolder_To, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName);
            }

            //move
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Move(targetFolder_From_File, targetFolder_To_File);
        }

    }
}
