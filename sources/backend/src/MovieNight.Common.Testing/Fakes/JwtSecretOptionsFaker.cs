using Bogus;
using MovieNight.Common.Options;

namespace MovieNight.Common.Testing.Fakes
{
    public class JwtSecretOptionsFaker :
        Faker<JwtSecretOptions>
    {
        private string _key;
        private string _issuer;
        private string _audience;

        public JwtSecretOptionsFaker()
        {
            RuleFor(x => x.Key, f => _key ?? f.Random.AlphaNumeric(32));
            RuleFor(x => x.Issuer, f => _issuer ?? f.Internet.Url());
            RuleFor(x => x.Audience, f => _audience ?? f.Internet.Url());
        }

        public JwtSecretOptionsFaker WithKey(string key)
        {
            _key = key;
            return this;
        }

        public JwtSecretOptionsFaker WithIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public JwtSecretOptionsFaker WithAudience(string audience)
        {
            _audience = audience;
            return this;
        }
    }
}
