using Google.YouTube;

namespace VodadModel.Helpers
{
    public class YouTubeHelper
    {
        public YouTubeRequest GetYouTubeRequestObject(string token)
        {
            var youTubeRequestSettings = new YouTubeRequestSettings(Utilities.Constants.YouTube.AppName,
                                                                    Utilities.Constants.YouTube.Devkey, token);
            return new YouTubeRequest(youTubeRequestSettings);
        }

    }
}
