using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MilanoFashion.Models
{
    public class PagenatedList<T> : List<T>
    {
        public PagenatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.PageSize = pageSize;
            this.AddRange(items);
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext => PageIndex < TotalPages;
        public bool HasPrev => PageIndex > 1;

        public static PagenatedList<T> Create(IQueryable<T> query, int pageIndex, int pageSize)
        {
            var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PagenatedList<T>(items, query.Count(), pageIndex, pageSize);
        }
    }
}
