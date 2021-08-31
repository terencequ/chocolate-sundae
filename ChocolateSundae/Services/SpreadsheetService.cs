using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using ChocolateSundae.Services.Abstractions;
using ChocolateSundae.Services.Models;
using CsvHelper;

namespace ChocolateSundae.Services
{
    public class SpreadsheetService : ISpreadsheetService
    {
        public const string OutputPath = "output";
        
        public async Task WriteUserDataToCsvAsync(params UserData[] data)
        {
            await using var writer = new StreamWriter($"{OutputPath}.csv");
            await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<UserData>();
            await csv.NextRecordAsync();
            await csv.WriteRecordsAsync(data);
        }
    }
}