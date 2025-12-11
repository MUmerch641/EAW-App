using System;
using System.Collections.Generic;
using System.Text;

namespace EatWork.Mobile.Contracts
{
    public interface IImageResizer
    {
        byte[] ScaleImage(byte[] imageData, float width, float height, bool updateOrientation);
    }
}
