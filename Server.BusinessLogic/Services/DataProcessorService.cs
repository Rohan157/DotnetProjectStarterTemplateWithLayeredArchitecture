using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NPOI.XSSF.UserModel;
using Server.BusinessLogic.Services.Interface;
using Server.Domain.Entities;
using Server.Domain.Exceptions;
using Server.Infrastructure.Repositories.Interface;
using System.Reflection;

namespace Server.BusinessLogic.Services
{
    public class DataProcessorService : IDataProcessorService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IBeCauseRepository _beCauseRepository;
        private readonly ILogger<DataProcessorService> _logger;
        public DataProcessorService(IHotelRepository hotelRepository, IBeCauseRepository beCauseRepository, ILogger<DataProcessorService> logger)
        {
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository), "HotelRepository cannot be null.");
            _beCauseRepository = beCauseRepository ?? throw new ArgumentNullException(nameof(beCauseRepository), "BecauseRepository cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> GetCertificationStatusAsync(string hotelId)
        {
            _logger.LogInformation($"[{GetType()?.Name}/{MethodBase.GetCurrentMethod()?.Name}] Started");
            var beCauseId = await _hotelRepository.GetBeCauseIdByHotelIdAsync(hotelId);
            if (beCauseId == 0) { return  false; }
            var isCertified = await _beCauseRepository.GetCertificateStatusByIdAsync(beCauseId);
            _logger.LogInformation($"[{GetType()?.Name}/{MethodBase.GetCurrentMethod()?.Name}] Ended");
            return isCertified;
        }
        public async Task<bool> SaveFileAsync(IFormFile file)
        {
            _logger.LogInformation($"[{GetType()?.Name}/{MethodBase.GetCurrentMethod()?.Name}] Started");
            var stream = new MemoryStream();
            file.CopyTo(stream);
            stream.Position = 0;
            var workbook = new XSSFWorkbook(stream);
            var count = workbook.NumberOfSheets;
            if (count == 2)
            {
                var extractedHotelData = ExtractedDataForHotelEntity(workbook, 0);
                if(extractedHotelData.Count() != 0 )
                {
                    await SaveHotelRecordsInDatabaseAsync(extractedHotelData);
                }
                var extractBeCauseData = ExtractedDataForBeCauseEntity(workbook, 1);
                if(extractBeCauseData.Count() != 0)
                {
                    await SaveBeCauseRecordsInDatabaseAsync(extractBeCauseData);
                }
            }
            if (count == 1)
            {
                if (workbook.GetSheetAt(0).GetRow(0).LastCellNum > 9)
                {
                    var extractBeCauseData = ExtractedDataForBeCauseEntity(workbook, 0);
                    if (extractBeCauseData != null)
                    {
                        await SaveBeCauseRecordsInDatabaseAsync(extractBeCauseData);
                    }
                }
                else
                {
                    var extractedHotelData = ExtractedDataForHotelEntity(workbook, 0);
                    if (extractedHotelData != null)
                    {
                        await SaveHotelRecordsInDatabaseAsync(extractedHotelData);
                    }
                }     
            }
            _logger.LogInformation($"[{GetType()?.Name}/{MethodBase.GetCurrentMethod()?.Name}] Ended");
            return true;
        }
        private static IEnumerable<Hotels> ExtractedDataForHotelEntity(XSSFWorkbook workbook, int index)
        {
            List<Hotels> extractedData = new();
            var sheet = workbook.GetSheetAt(index); // We will start from one because the first line is headings
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                if (row != null && !string.IsNullOrEmpty(row.GetCell(0)?.ToString()) 
                    && !string.IsNullOrEmpty(row.GetCell(1)?.ToString()))
                {
                    var hotel = new Hotels()
                    {
                        HotelName = row.GetCell(0).ToString(),
                        Id = row.GetCell(1)?.ToString(),
                        City = row.GetCell(2)?.ToString(),
                        Country = row.GetCell(3)?.ToString(),
                        Latitude = row.GetCell(4)?.ToString(),
                        Longitude = row.GetCell(5)?.ToString(), 
                        ZipCode = int.TryParse(row.GetCell(6)?.ToString(), out int zipCode) ? (int?)zipCode : null,
                        Address = row.GetCell(7)?.ToString(),
                        BeCauseId = int.TryParse(row.GetCell(8)?.ToString(), out int beCauseId) ? (int?)beCauseId : null,
                    };
                    extractedData.Add(hotel);
                }
            }
            return extractedData;
        }
        private static IEnumerable<BeCause> ExtractedDataForBeCauseEntity(XSSFWorkbook workbook, int index)
        {
            List<BeCause> extractedData = new();
            var sheet2 = workbook.GetSheetAt(index);
            for (int i = 1; i <= sheet2.LastRowNum; i++)
            {
                var row = sheet2.GetRow(i);
                if (row != null && !string.IsNullOrEmpty(row.GetCell(0)?.ToString()))
                {
                    var beCause = new BeCause()
                    {
                        Id = int.Parse(row.GetCell(0)?.ToString()),
                        HotelName = row.GetCell(1)?.ToString() ?? "",
                        Country = row.GetCell(2)?.ToString(),
                        State = row.GetCell(3)?.ToString(),
                        City = row.GetCell(4)?.ToString(),
                        Address = row.GetCell(5)?.ToString(),
                        PostalCode = int.TryParse(row.GetCell(6)?.ToString(), out int postalCode) ? (int?)postalCode : null,
                        Latitude = row.GetCell(7)?.ToString(),
                        Longitude = row.GetCell(8)?.ToString(),
                        IsCertified = row.GetCell(9)?.ToString() == "Yes" ? true : false,
                        IssueDate = row.GetCell(10)?.ToString(),
                        ExpirationDate = row.GetCell(11)?.ToString(),
                        Website = row.GetCell(12)?.ToString()
                    };
                    extractedData.Add(beCause);
                }
            }
            return extractedData;
        }
        private async Task<IEnumerable<Hotels>> GetHotelListAsync()
        {
            try
            {
                var hotels = await _hotelRepository.GetAllAsync();
                return hotels;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error in GetHotelListAsync: An error occurred while retrieving hotels.");
                throw new CustomException("An error occurred while getting hotels from the database.", ex);
            }
        }
        private async Task<IEnumerable<BeCause>> GetBeCauseListAsync()
        {
            try
            {
                var list = await _beCauseRepository.GetAllAsync();
                return list;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error in GetBeCauseListAsync: An error occurred while retrieving beCause list.");
                throw new CustomException("An error occurred while getting the beCause List from the database.", ex);
            }
        }
        private async Task<IEnumerable<Hotels>> AddHotelRecordsAsync(IEnumerable<Hotels> hotels)
        {
            try
            {
                var response = await _hotelRepository.AddRangeAsync(hotels);
                return response;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error in AddHotelRecordsAsync: An error occurred while adding hotels.");
                throw new CustomException("An error occurred while adding hotels to the database.", ex);
            }
        }
        private async Task<IEnumerable<BeCause>> AddBeCauseRecordsAsync(IEnumerable<BeCause> beCauseRecords)
        {
            try
            {
                var response = await _beCauseRepository.AddRangeAsync(beCauseRecords);
                return response;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBeCauseRecordsAsync: An error occurred while adding beCause records.");
                throw new CustomException("An error occurred while Adding beCause List to the database.", ex);
            }
        }
        private async Task<IEnumerable<Hotels>> UpdateHotelRecordsAsync(IEnumerable<Hotels> hotels)
        {
            try
            {
                var response = await _hotelRepository.UpdateHotelListAsync(hotels);
                return response;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateHotelRecordsAsync: An error occurred while updating hotels.");
                throw new CustomException("An error occurred while updating hotels to the database.", ex);
            }
        }
        private async Task<IEnumerable<BeCause>> UpdateBeCauseRecordsAsync(IEnumerable<BeCause> beCauseRecords)
        {
            try
            {
                var response = await _beCauseRepository.UpdateBeCauseListAsync(beCauseRecords);
                return response;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateBeCauseRecordsAsync: An error occurred while updating beCause records.");
                throw new CustomException("An error occurred while updating beCause List to the database.", ex);
            }
        }
        private async Task SaveHotelRecordsInDatabaseAsync(IEnumerable<Hotels> extractedHotelData)
        {
            var hotelsfromDb = await GetHotelListAsync();
            var newHotels = extractedHotelData.Except(hotelsfromDb, new HotelComparer());
            var previousHotels = extractedHotelData.Intersect(hotelsfromDb, new HotelComparer());
            if (newHotels.Any())
            {
                await AddHotelRecordsAsync(newHotels);
            }
            if (previousHotels.Any())
            {
                await UpdateHotelRecordsAsync(previousHotels);
            }
        }
        private async Task SaveBeCauseRecordsInDatabaseAsync(IEnumerable<BeCause> extractedBeCauseData)
        {
            var beCauseListfromDb = await GetBeCauseListAsync();
            var newBecauseRecords = extractedBeCauseData.Except(beCauseListfromDb, new BeCauseComparer());
            var previousBecauseRecords = extractedBeCauseData.Intersect(beCauseListfromDb, new BeCauseComparer());
            if (newBecauseRecords.Any())
            {
                await AddBeCauseRecordsAsync(newBecauseRecords);
            }
            if (previousBecauseRecords.Any())
            {
                await UpdateBeCauseRecordsAsync(previousBecauseRecords);
            }
        }
        public class HotelComparer : IEqualityComparer<Hotels>
        {
            public bool Equals(Hotels? x, Hotels? y)
            {
                return (x is not null && y is not null && x.Id == y.Id);
            }
            public int GetHashCode(Hotels obj)
            {
                return obj.Id.GetHashCode();
            }
        }
        public class BeCauseComparer : IEqualityComparer<BeCause>
        {
            public bool Equals(BeCause? x, BeCause? y)
            {
                return (x is not null && y is not null && x.Id == y.Id);
            }
            public int GetHashCode(BeCause obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
