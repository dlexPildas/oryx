using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class SearchPayloadModel<T>
    {
        public SearchPayloadModel()
        {
        }
        public IList<T> Items { get; set; }
        public bool HasNext { get; set; }
        public int Limit { get; set; }
    }
}
