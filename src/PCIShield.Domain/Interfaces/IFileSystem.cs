using System.Threading.Tasks;
namespace PCIShield.Domain.Interfaces
{
    public interface IFileSystem
    {
        Task<bool> SavePicture(string pictureName, string pictureBase64);
    }
}