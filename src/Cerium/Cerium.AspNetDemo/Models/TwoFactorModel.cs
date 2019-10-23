namespace Cerium.AspNetDemo.Models
{
    public class TwoFactorModel
    {
        public string Provider { get; set; }
        public string Code { get; set; }
        public bool RememberBrowser { get; set; }
    }
}