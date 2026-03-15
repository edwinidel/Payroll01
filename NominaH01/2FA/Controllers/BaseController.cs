using Microsoft.AspNetCore.Mvc;
using _2FA.Data;
using Microsoft.EntityFrameworkCore;

namespace _2FA.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets the currently selected company ID from session
        /// </summary>
        protected int? GetCurrentCompanyId()
        {
            if (HttpContext?.Session == null)
                return null;
            return HttpContext.Session.GetInt32("SelectedCompanyId");
        }

        /// <summary>
        /// Gets the currently selected company entity
        /// </summary>
        protected async Task<Data.Entities.CompanyEntity?> GetCurrentCompanyAsync()
        {
            var companyId = GetCurrentCompanyId();
            if (!companyId.HasValue)
                return null;

            return await _context.Companies
                .Include(c => c.BusinessGroup)
                .FirstOrDefaultAsync(c => c.Id == companyId.Value);
        }

        /// <summary>
        /// Ensures a company is selected, redirects to switch company page if not
        /// </summary>
        protected IActionResult EnsureCompanySelected()
        {
            if (HttpContext?.Session == null || !GetCurrentCompanyId().HasValue)
            {
                TempData["Error"] = "Debe seleccionar una compañía antes de continuar.";
                return RedirectToAction("SwitchCompany", "Home");
            }
            return null; // No redirect needed
        }

        /// <summary>
        /// Filters a query by the current company
        /// </summary>
        protected IQueryable<T> FilterByCurrentCompany<T>(IQueryable<T> query, string companyPropertyName = "CompanyId") where T : class
        {
            var companyId = GetCurrentCompanyId();
            if (!companyId.HasValue)
                return query; // Return unfiltered if no company selected

            // Check if the entity type has the specified company property
            var propertyInfo = typeof(T).GetProperty(companyPropertyName);
            if (propertyInfo == null)
                return query; // Return unfiltered if property doesn't exist

            // Use reflection to filter by company property
            var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, companyPropertyName);
            var constant = System.Linq.Expressions.Expression.Constant(companyId.Value);
            var equality = System.Linq.Expressions.Expression.Equal(property, constant);
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(equality, parameter);

            return query.Where(lambda);
        }
    }
}