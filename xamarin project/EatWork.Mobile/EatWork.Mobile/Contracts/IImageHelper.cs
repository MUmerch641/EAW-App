namespace EatWork.Mobile.Contracts
{
    public interface IImageHelper
    {
        byte[] ScaleImage(byte[] imageData, float maxWidth, float maxHeight);
    }
}