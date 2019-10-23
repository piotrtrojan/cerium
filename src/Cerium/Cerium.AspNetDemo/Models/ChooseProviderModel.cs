using System.Collections.Generic;

namespace Cerium.AspNetDemo.Models
{
    public class ChooseProviderModel
    {
        public List<string> Providers { get; set; }
        public string ChosenProvider { get; set; }
    }
}