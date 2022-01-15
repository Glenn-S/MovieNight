namespace MovieNight.Common.Options
{
    /// <summary>
    /// Class for injecting app settings related to Jwt tokens.
    /// </summary>
    public class JwtSecretOptions
    {
        public const string JwtSecret = "JwtSecret";

        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
