using System.Collections.Generic;

namespace OryxDomain.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<HomeProductModel> Products { get; set; }
    }
}
