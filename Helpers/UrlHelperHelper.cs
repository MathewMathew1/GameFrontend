namespace BoardGameFrontend.Helpers
{
    public static class UrlHelper
    {
        private static readonly string DevUrl = "http://localhost:5078/api/"; //"https://game-6mzf.onrender.com/api/";//"http://localhost:5078/api/";
        private static readonly string ProdUrl = "https://game-6mzf.onrender.com/api/";

        private static readonly string DevUrlSignal = "http://localhost:5078/lobbyHub";
        private static readonly string ProdUrlSignal = "https://game-6mzf.onrender.com/lobbyHub";

        public static string GetBaseUrl()
        {
            #if DEBUG
                // If the application is running in Debug mode (development)
                return DevUrl;
            #else
                // If the application is running in Release mode (production)
                return ProdUrl;
            #endif
        }

        public static string GetBaseUrlSignalR()
        {
            #if DEBUG
                // If the application is running in Debug mode (development)
                return DevUrlSignal ;
            #else
                // If the application is running in Release mode (production)
                return ProdUrlSignal;
            #endif
        }
    }
}