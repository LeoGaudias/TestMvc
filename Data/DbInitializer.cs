using System.Linq;
using TestMvc.Models;

namespace TestMvc.Data
{
    public class DbInitializer
    {
        public static void Initialize(TestContext context)
        {
            context.Database.EnsureCreated();

            if (context.Images.Any())
                return;

            context.Images.Add(new Image { Title = "TEST 1", Description = "Test png", Path = "/img/Test1.png" });
            context.Images.Add(new Image { Title = "TEST 2", Description = "Test gif", Path = "/img/Test2.gif" });
            context.Images.Add(new Image { Title = "TEST 3", Description = "Test jpg", Path = "/img/Test3.jpg" });

            context.SaveChanges();
        }
    }
}