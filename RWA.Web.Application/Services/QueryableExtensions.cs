using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models.Dtos;

namespace RWA.Web.Application.Services
{
    public static class QueryableExtensions
    {
        public static async Task<DataTablesResponse<T>> ToDataTablesResponse<T>(
            this IQueryable<T> query,
            DataTableRequest request,
            CancellationToken cancellationToken = default)
        {
            // Apply filtering
            if (request.Filters != null)
            {
                foreach (var filter in request.Filters)
                {
                    if (!string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where($"{filter.Key}.ToLower().Contains(@0)", filter.Value.ToLower());
                    }
                }
            }

            var totalItems = await query.CountAsync(cancellationToken);

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = query.OrderBy($"{request.SortBy} {(request.SortDesc ? "descending" : "ascending")}");
            }

            // Apply pagination
            var pagedData = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new DataTablesResponse<T>
            {
                Items = pagedData,
                TotalItems = totalItems
            };
        }
    }
}
