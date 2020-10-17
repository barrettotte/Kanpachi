using System.Text;

namespace Kanpachi.Lib{

    public static class KanpachiDefaults{

        public readonly static string DefaultEncoding = Encoding.UTF8.BodyName;
        public readonly static int SshPort = 22;
        public readonly static int ConnectAttempts = 5;
        public readonly static double Timeout = 10.0;
    }
}