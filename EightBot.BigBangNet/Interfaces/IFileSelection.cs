using System;
using System.Threading.Tasks;

namespace EightBot.BigBang.Interfaces
{
    public interface IFileSelection
    {
        Task<ViewModel.FileInformationViewModel> SelectFileAsync(int maxFileSizeInBytes = -1);

        Task<bool> SaveFileAsync(string documentUrl);

        Task<bool> SaveFileAsync(string filename, byte[] fileContent);
    }
}
