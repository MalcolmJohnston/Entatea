using System;

namespace Entatea.Tests.Configuration
{
    public class TestConfiguration
    {
        public string MsSqlServer { get; set; } = "localhost";

        public int MsSqlPort { get; set; } = 1400;

        public string MsSqlUsername { get; set; } = "sa";

        public string MsSqlPassword { get; set; } = "Passw0rd!";

        public string MySqlServer { get; set; } = "localhost";

        public int MySqlPort { get; set; } = 3308;

        public string MySqlUsername { get; set; } = "root";

        public string MySqlPassword { get; set; } = string.Empty;
    }
}
