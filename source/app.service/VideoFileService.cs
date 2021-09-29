using app.domain.Exceptions;
using app.domain.Utilities;
using app.service.Model.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace app.service
{
    public class VideoFileService : IFileService
    {
        public GenericServiceResponse<string> Save_Create(IFormFile attachedFile, string id, string pathOnly, string folderName)
        {
            var response = new GenericServiceResponse<string>();

            try
            {
                string newFileName = id + Path.GetExtension(attachedFile.FileName);

                //prepare path
                string newFilePathOriginal = FileHelper.PrepareTarget("Original", folderName, pathOnly, newFileName);

                //save original
                using (var fileStream = new FileStream(newFilePathOriginal, FileMode.Create))
                {
                    attachedFile.CopyTo(fileStream);
                }

                //no any ResizeToTarget ??????????????????????

                response.Model = newFileName;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }
            return response;
        }

        public BoolServiceResponse Delete(string pathOnly, string folderName, string fileName)
        {
            var response = new BoolServiceResponse();
            try
            {
                //FileHelper.BackupAndRemove("64", folderName, pathOnly, fileName);
                //FileHelper.BackupAndRemove("256", folderName, pathOnly, fileName);
                //FileHelper.BackupAndRemove("512", folderName, pathOnly, fileName);
                //FileHelper.BackupAndRemove("1200", folderName, pathOnly, fileName);
                FileHelper.BackupAndRemove("Original", folderName, pathOnly, fileName);

                response.Model = true;
                response.IsSuccessfull = true;
            }
            catch (BusinessException exp)
            {
                response.LoadFrom(exp);
            }
            catch (Exception exp)
            {
                response.ErrorMessage = exp.ToString();
            }

            return response;
        }

        public void ResizeToTarget(int size, string folderName, string pathOnly, string newFileName, string newFilePathOriginal)
        {
        }

        public BoolServiceResponse ExtractImageFromVideo(string ffmpegPath, string videoFullPath, string imagePath, string imageName, double second)
        {
            throw new NotImplementedException();
        }
    }
}
