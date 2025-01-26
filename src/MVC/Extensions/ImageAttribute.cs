using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Library_Managment_System.Extensions
{
    public class ImageAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = (IFormFile)value;
            var configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            long fileSizeLimit = configuration.GetValue<long>("FileSizeLimit");
            string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };

            Dictionary<string, List<byte[]>> fileSignature = new Dictionary<string, List<byte[]>>{
                { ".jpeg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                },
                { ".jpg", new List<byte[]>{
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                    }
                },
                { ".png", new List<byte[]>{
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A}
                    }
                }
           };
            
            if (file == null)
            {
                return new ValidationResult("The file field is required");
            }

            if (file.Length > fileSizeLimit)
            {
                return new ValidationResult("The file size is over the limit");
            }

            var fileExtenstion = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtenstion))
            {
                string extension = string.Join(", ", allowedExtensions);

                return new ValidationResult($"The file extension must be one of : " + extension);
            }

            using (var reader = new BinaryReader(file.OpenReadStream()))
            {
                var signatures = fileSignature[fileExtenstion];
                var headerBytes = reader.ReadBytes(signatures.Max(x => x.Length));

                if (!signatures.Any(x => headerBytes.Take(x.Length).SequenceEqual(x)))
                {
                    return new ValidationResult("This file is not a image");
                }
            }

            return ValidationResult.Success;
        }
    }
}
