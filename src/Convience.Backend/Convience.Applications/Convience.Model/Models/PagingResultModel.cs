using System.Collections.Generic;

namespace Convience.Model.Models
{
    public class PagingResultModel<T> where T : class
    {
        public IList<T> Data { get; set; }

        public int Count { get; set; }
    }
    public class PageResultModel<T> where T : class
    {
        public T Data { get; set; }

        public int Count { get; set; }
    }
}
