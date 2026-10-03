using System;
using System.Collections.Generic;

namespace Alexapps.SkinCare.Dtos.Common
{
    public class PaginationMetadata
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public long TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PaginationMetadata(int page, int limit, long totalCount)
        {
            Page = page;
            Limit = limit;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / limit);
            HasNextPage = page < TotalPages;
            HasPreviousPage = page > 1;
        }
    }

    public class PagedResultWithMetadata<T>
    {
        public List<T> Items { get; set; }
        public PaginationMetadata Metadata { get; set; }

        public PagedResultWithMetadata(List<T> items, int page, int limit, long totalCount)
        {
            Items = items;
            Metadata = new PaginationMetadata(page, limit, totalCount);
        }
    }
}
