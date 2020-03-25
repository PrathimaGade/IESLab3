using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;

namespace FTPAp.Models.Utilities
{
    public class Imaging
    {





        /// <summary>
        /// Converts an Image object to Base64
        /// </summary>
        /// <param name="image">An Image object</param>
        /// <param name="format">The format of the image (JPEG, BMP, etc.)</param>
        /// <returns>Base64 encoded string representation of an Image</returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// Converts a Base64 encoded string to an Image
        /// </summary>
        /// <param name="base64String">Base64 encoded Image string</param>
        /// <returns>Decoded Image</returns>
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String.Trim());
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        //public Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    try
        //    {
        //        MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
        //        ms.Write(byteArrayIn, 0, byteArrayIn.Length);
        //        returnImage = Image.FromStream(ms, true);//Exception occurs here
        //    }
        //    catch { }
        //    return returnImage;
        //}

    }
    }
