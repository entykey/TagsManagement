using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace TagsManagement.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class FileController : Controller
    {

        // GET https://localhost:7199/api/File/getImage
        //[CustomAuthorize]
        //[TypeFilter(typeof(CustomAuthorizeFilter))]
        [HttpGet("getImage")]
        public async Task<FileContentResult> GetOriginalImage()
        {
            // correct file mime type is important, is not match -> the browser might refuse to render/download
            string mimeType = "png";
            string fileName = "Using-Both-Repository";
            string filePath = @"C:\Users\TuanMacbookPro\Downloads\" + fileName + "."+ mimeType;
            string contentType = $"image/{mimeType}";   // auto convert to this contentType

            Byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);


            return File(fileBytes, contentType);   
        }

        // @"C:\Users\TuanMacbookPro\Documents\ASPWebAPIs\ebooks\"
        [HttpGet("downloadPdf")]
        public async Task<FileContentResult> DownloadOriginalPdf()
        {
            // correct file mime type is important, is not match -> the browser might refuse to render/download
            string mimeType = "pdf";
            string fileName = "ASP_NET_Identity";
            string filePath = @"C:\Users\TuanMacbookPro\Documents\ASPWebAPIs\ebooks\" + fileName + "." + mimeType;
            string contentType = $"application/{mimeType}";

            Byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            // set Content-Disposition header to "attachment"
            Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}.{mimeType}");

            return File(fileBytes, contentType);
        }


        [HttpGet("getResizedImage")]
        public async Task<FileContentResult> GetResizedImage()
        {
            // correct file mime type is important, is not match -> the browser might refuse to render/download
            string mimeType = "png";
            string filePath = @"C:\Users\TuanMacbookPro\Downloads\Using-Both-Repository" + "." + mimeType;

            // Load the image from file (must install package: System.Drawing.Common  !!!)
            System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
            //Image img = Image.FromFile(filePath);

            // Set the maximum width and height for the resized image
            int maxWidth = 360;
            int maxHeight = 200;

            // Calculate the new width and height for the resized image while maintaining the aspect ratio
            int newWidth, newHeight;
            if (img.Width > maxWidth || img.Height > maxHeight)
            {
                double aspectRatio = (double)img.Width / (double)img.Height;
                if (img.Width > maxWidth)
                {
                    newWidth = maxWidth;
                    newHeight = (int)Math.Round(maxWidth / aspectRatio);
                }
                else
                {
                    newHeight = maxHeight;
                    newWidth = (int)Math.Round(maxHeight * aspectRatio);
                }
            }
            else
            {
                newWidth = img.Width;
                newHeight = img.Height;
            }

            // Resize the image
            System.Drawing.Image resizedImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(img, 0, 0, newWidth, newHeight);
            }

            // Save the resized image to a byte array in the specified format
            byte[] resizedBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Save(ms, ImageFormat.Jpeg);  // change the output format if needed
                resizedBytes = ms.ToArray();
            }

            return File(resizedBytes, $"image/{mimeType}");
        }


        [HttpGet("getResizedImageImageSharp")]
        public async Task<FileContentResult> GetResizedImageUsingImageSharp()
        {
            // correct file mime type is important, is not match -> the browser might refuse to render/download
            string mimeType = "png";
            string filePath = @"C:\Users\TuanMacbookPro\Downloads\Using-Both-Repository" + "." + mimeType;

            // Load the image from file using ImageSharp
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(filePath))
            {
                // Set the maximum width and height for the resized image
                int maxWidth = 360;
                int maxHeight = 200;

                // Calculate the new width and height for the resized image while maintaining the aspect ratio
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new SixLabors.ImageSharp.Size(maxWidth, maxHeight),
                    Mode = ResizeMode.Max,
                    Position = AnchorPositionMode.Center
                }));

                // Save the resized image to a byte array in the specified format
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, new JpegEncoder());  // change the output format if needed
                    var resizedBytes = ms.ToArray();

                    return File(resizedBytes, $"image/{mimeType}");
                }
            }
        }

    }
}
