using _2FA.Data; 
using _2FA.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace _2FA.Services
{
    public class HolidayService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public HolidayService(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public class HolidayApiResponse
        {
            public string Date { get; set; } = string.Empty;
            public string LocalName { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string CountryCode { get; set; } = string.Empty;
            public bool Fixed { get; set; }
            public bool Global { get; set; }
            public object? Counties { get; set; } // Can be null or array
            public int? LaunchYear { get; set; }
            public string[]? Types { get; set; }
        }

        /// <summary>
        /// Fetches public holidays from Nager.Date API for a specific year and country
        /// </summary>
        /// <param name="year">The year to fetch holidays for</param>
        /// <param name="countryCode">ISO2 country code (e.g., "PA" for Panama)</param>
        /// <returns>List of HolidayApiResponse objects</returns>
        public async Task<List<HolidayApiResponse>> FetchHolidaysFromApiAsync(int year, string countryCode)
        {
            var url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var holidays = JsonSerializer.Deserialize<List<HolidayApiResponse>>(jsonString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return holidays ?? [];
                }
                else
                {
                    throw new Exception($"API request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching holidays from API: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the holidays table with data from the API for a specific year
        /// </summary>
        /// <param name="year">The year to update holidays for</param>
        /// <param name="countryCode">ISO2 country code</param>
        /// <returns>Number of holidays updated/added</returns>
        public async Task<int> UpdateHolidaysFromApiAsync(int year, string countryCode)
        {
            var apiHolidays = await FetchHolidaysFromApiAsync(year, countryCode);
            int updatedCount = 0;

            foreach (var apiHoliday in apiHolidays)
            {
                // Parse the date string to DateTime
                if (!DateTime.TryParse(apiHoliday.Date, out DateTime holidayDate))
                {
                    continue; // Skip invalid dates
                }

                // Check if holiday already exists
                var existingHoliday = await _context.HoliDays
                    .FirstOrDefaultAsync(h => h.Date.Date == holidayDate.Date);

                if (existingHoliday == null)
                {
                    // Add new holiday
                    var newHoliday = new HoliDayEntity
                    {
                        Date = holidayDate,
                        Description = apiHoliday.LocalName ?? apiHoliday.Name,
                        IsActive = true,
                        Created = DateTime.UtcNow,
                        CreatedBy = "HolidayService"
                    };

                    _context.HoliDays.Add(newHoliday);
                    updatedCount++;
                }
                else
                {
                    // Update existing holiday if description changed
                    if (existingHoliday.Description != (apiHoliday.LocalName ?? apiHoliday.Name))
                    {
                        existingHoliday.Description = apiHoliday.LocalName ?? apiHoliday.Name;
                        existingHoliday.Modified = DateTime.UtcNow;
                        existingHoliday.ModifiedBy = "HolidayService";
                        updatedCount++;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return updatedCount;
        }

        /// <summary>
        /// Checks if a specific date is a holiday
        /// </summary>
        /// <param name="date">The date to check</param>
        /// <returns>True if the date is a holiday, false otherwise</returns>
        public async Task<bool> IsHolidayAsync(DateTime date)
        {
            return await _context.HoliDays
                .AnyAsync(h => h.Date.Date == date.Date && h.IsActive);
        }

        /// <summary>
        /// Gets all active holidays for a specific year
        /// </summary>
        /// <param name="year">The year to get holidays for</param>
        /// <returns>List of active holidays</returns>
        public async Task<List<HoliDayEntity>> GetHolidaysForYearAsync(int year)
        {
            return await _context.HoliDays
                .Where(h => h.Date.Year == year && h.IsActive)
                .OrderBy(h => h.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Deactivates all holidays for a specific year (useful for cleanup before updating)
        /// </summary>
        /// <param name="year">The year to deactivate holidays for</param>
        /// <returns>Number of holidays deactivated</returns>
        public async Task<int> DeactivateHolidaysForYearAsync(int year)
        {
            var holidaysToDeactivate = await _context.HoliDays
                .Where(h => h.Date.Year == year && h.IsActive)
                .ToListAsync();

            foreach (var holiday in holidaysToDeactivate)
            {
                holiday.IsActive = false;
                holiday.Modified = DateTime.UtcNow;
                holiday.ModifiedBy = "HolidayService";
            }

            await _context.SaveChangesAsync();
            return holidaysToDeactivate.Count;
        }

        /// <summary>
        /// Gets the next upcoming holiday from a specific date
        /// </summary>
        /// <param name="fromDate">The date to start searching from</param>
        /// <returns>The next holiday or null if none found</returns>
        public async Task<HoliDayEntity?> GetNextHolidayAsync(DateTime fromDate)
        {
            return await _context.HoliDays
                .Where(h => h.Date >= fromDate && h.IsActive)
                .OrderBy(h => h.Date)
                .FirstOrDefaultAsync();
        }
    }
}