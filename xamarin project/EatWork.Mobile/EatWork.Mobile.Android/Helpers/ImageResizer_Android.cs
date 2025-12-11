using Android.Graphics;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.Droid.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer_Android))]

namespace EatWork.Mobile.Droid.Helpers
{
    public class ImageResizer_Android : IImageResizer
    {
        public byte[] ScaleImage(byte[] imageData, float maxWidth, float maxHeight, bool updateOrientation)
        {
            int quality = 100;
            float scaleFactor = 0.20f;

            // Load the image from the byte array
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            // Get the image's EXIF orientation to preserve the original orientation
            var exif = new Android.Media.ExifInterface(new System.IO.MemoryStream(imageData));
            int orientation = exif.GetAttributeInt(Android.Media.ExifInterface.TagOrientation, (int)Android.Media.Orientation.Normal);

            Bitmap correctedImage = originalImage;
            bool rotateImage = (originalImage.Width > originalImage.Height);

            if (rotateImage)
            {
                Matrix matrix = new Matrix();

                switch (orientation)
                {
                    case (int)Android.Media.Orientation.Rotate90:
                        // Already landscape, rotate to make it portrait
                        matrix.PostRotate(90);
                        break;

                    case (int)Android.Media.Orientation.Rotate180:
                        // Upside-down, rotate to make it upright
                        matrix.PostRotate(180);
                        break;

                    case (int)Android.Media.Orientation.Rotate270:
                        // Landscape, rotate to portrait
                        matrix.PostRotate(90);
                        break;

                    case (int)Android.Media.Orientation.Normal:
                    case 0: // If EXIF is "undefined"
                            // Assume it is landscape if undefined and rotate to portrait
                        matrix.PostRotate(90);
                        break;

                    default:
                        // If the orientation is already correct, no need to rotate
                        break;
                }

                correctedImage = Bitmap.CreateBitmap(originalImage, 0, 0, originalImage.Width, originalImage.Height, matrix, true);
            }

            float newHeight = correctedImage.Height * scaleFactor;
            float newWidth = correctedImage.Width * scaleFactor;

            // Create a scaled bitmap
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(correctedImage, (int)newWidth, (int)newHeight, true);

            // Convert the resized image to a byte array
            using (var stream = new System.IO.MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Png, quality, stream);
                return stream.ToArray();
            }
        }
    }
}