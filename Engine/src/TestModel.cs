using Microsoft.Extensions.Configuration;

namespace Civ2engine
{
    public class TestModel
    {
        private readonly IConfiguration Configuration;

        public TestModel(IConfiguration configuration)
        {
            var key = Configuration["Civ2Path"];
        }


    }
}
