using System.Threading.Tasks;
using ChocolateSundae.Services.Models;

namespace ChocolateSundae.Services.Abstractions
{
    public interface ISpreadsheetService
    {
        Task WriteUserDataToCsvAsync(params UserData[] data);
    }
}