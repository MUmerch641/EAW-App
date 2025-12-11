using CoreGraphics;
using EatWork.Mobile.Contracts;
using EatWork.Mobile.iOS.Helpers;
using Foundation;
using System;
using System.Diagnostics;
using System.Drawing;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer_iOS))]

namespace EatWork.Mobile.iOS.Helpers
{
    public class ImageResizer_iOS : IImageResizer
    {
        public byte[] ScaleImage(byte[] imageData, float width, float height, bool updateOrientation)
        {
            try
            {
                // Load the image from the byte array
                UIImage originalImage;
                using (var data = NSData.FromArray(imageData))
                {
                    originalImage = UIImage.LoadFromData(data);
                }

                var orientation = originalImage.Orientation;

                var rotateImage = (originalImage.Size.Width > originalImage.Size.Height);

                if (rotateImage)
                {
                    // Set up a transform to rotate the image to portrait mode if needed
                    CGAffineTransform transform = CGAffineTransform.MakeIdentity();

                    if (orientation == UIImageOrientation.Left)
                    {
                        // Rotate the image 90 degrees to the right (clockwise) to make it portrait
                        transform = CGAffineTransform.MakeRotation((float)Math.PI / 2);
                    }
                    else if (orientation == UIImageOrientation.Right)
                    {
                        // Rotate the image 90 degrees to the left (counter-clockwise) to make it portrait
                        transform = CGAffineTransform.MakeRotation(-(float)Math.PI / 2);
                    }
                    else if (orientation == UIImageOrientation.Down)
                    {
                        // Rotate the image 180 degrees if it's upside down
                        transform = CGAffineTransform.MakeRotation((float)Math.PI);
                    }

                    // Apply the transformation to the image
                    CGSize imageSize = new CGSize(originalImage.Size.Width, originalImage.Size.Height);

                    UIGraphics.BeginImageContextWithOptions(imageSize, false, originalImage.CurrentScale);
                    CGContext context = UIGraphics.GetCurrentContext();

                    // Move origin to center before rotating
                    context.TranslateCTM(imageSize.Width / 2, imageSize.Height / 2);
                    context.RotateCTM((float)Math.PI / 2);  // Apply the rotation from transform

                    // Apply the transformation
                    context.ConcatCTM(transform);

                    // Draw the original image on the context
                    originalImage.Draw(new CGRect(-imageSize.Height / 2, -imageSize.Width / 2, imageSize.Height, imageSize.Width));
                }

                var rotatedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                // Resize the rotated image (optional resizing logic)
                var scaleFactor = 0.20f;
                var newWidth = rotatedImage.Size.Width * scaleFactor;
                var newHeight = rotatedImage.Size.Height * scaleFactor;
                var newSize = new CGSize(newWidth, newHeight);

                UIGraphics.BeginImageContextWithOptions(newSize, false, 1.0f);
                rotatedImage.Draw(new CGRect(0, 0, newSize.Width, newSize.Height));

                var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                // Convert the resized image to a JPEG byte array with compression
                using (var imageDataStream = resizedImage.AsJPEG().AsStream())
                {
                    byte[] result;
                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        imageDataStream.CopyTo(memoryStream);
                        result = memoryStream.ToArray();
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType().Name} : {ex.Message}");
                return imageData;
            }
        }
    }
}