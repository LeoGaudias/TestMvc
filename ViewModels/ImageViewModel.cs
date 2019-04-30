using Microsoft.AspNetCore.Http;
using TestMvc.Models;

namespace TestMvc.ViewModels
{
    public class ImageViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get;set; }
    }
}