using app.domain.Exceptions;
using app.domain.Utilities;
using app.service.Model.Response;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Reflection;

namespace app.service
{
    public class ImageFileService : IFileService
    {
        private readonly ILogger<ImageFileService> _logger;

        public ImageFileService(ILogger<ImageFileService> logger)
        {
            _logger = logger;
        }

        public GenericServiceResponse<string> Save_Create(IFormFile attachedFile, string id, string pathOnly, string folderName)
        {
            var response = new GenericServiceResponse<string>();

            try
            {
                string newFileName = id + Path.GetExtension(attachedFile.FileName);

                //save original
                string newFilePathOriginal = FileHelper.PrepareTarget("Original", folderName, pathOnly, newFileName);
                using (var fileStream = new FileStream(newFilePathOriginal, FileMode.Create))
                {
                    attachedFile.CopyTo(fileStream);
                }

                //save others
                ResizeToTarget(64, folderName, pathOnly, newFileName, newFilePathOriginal);
                ResizeToTarget(256, folderName, pathOnly, newFileName, newFilePathOriginal);
                ResizeToTarget(512, folderName, pathOnly, newFileName, newFilePathOriginal);
                ResizeToTarget(1200, folderName, pathOnly, newFileName, newFilePathOriginal);

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
                try
                {
                    FileHelper.BackupAndRemove("64", folderName, pathOnly, fileName);
                }
                catch (Exception) { }

                try
                {
                    FileHelper.BackupAndRemove("256", folderName, pathOnly, fileName);
                }
                catch (Exception) { }

                try
                {
                    FileHelper.BackupAndRemove("512", folderName, pathOnly, fileName);
                }
                catch (Exception) { }

                try
                {
                    FileHelper.BackupAndRemove("1200", folderName, pathOnly, fileName);

                }
                catch (Exception) { }

                try
                {
                    FileHelper.BackupAndRemove("Original", folderName, pathOnly, fileName);
                }
                catch (Exception) { }

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
            string targetFolder = Path.Combine(Path.Combine(pathOnly, folderName), size.ToString());
            if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

            using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load(newFilePathOriginal))
            {
                var (w, h) = Common.FindImageSizeIfOverLimit(image.Width, image.Height, size, size);
                image.Mutate(x => x.Resize(w, h));
                image.Save(Path.Combine(targetFolder, newFileName));
            }
        }

        public BoolServiceResponse ExtractImageFromVideo(string ffmpegPath, string videoFullPath, string imagePath, string imageName, double second)
        {
            var response = new BoolServiceResponse();
            try
            {
                if (!Directory.Exists(imagePath))
                    Directory.CreateDirectory(imagePath);

                var inputFile = new MediaFile { Filename = videoFullPath };
                var outputFile = new MediaFile { Filename = Path.Combine(imagePath, imageName) };

                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } -  inputFile: { inputFile.Filename}");
                _logger.LogInformation($"{ MethodBase.GetCurrentMethod().Name } -  outputFile: { outputFile.Filename}");

                using (var engine = new Engine(ffmpegPath))
                {
                    engine.GetMetadata(inputFile);

                    // Saves the frame located on the second'th second of the video.
                    var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(second) };
                    engine.GetThumbnail(inputFile, outputFile, options);
                }

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
    }
}
