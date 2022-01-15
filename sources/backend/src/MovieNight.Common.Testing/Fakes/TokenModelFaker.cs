using Bogus;
using MovieNight.Service.Models;

namespace MovieNight.Common.Testing.Fakes
{
    internal class TokenModelFaker :
        Faker<TokenModel>
    {
        private string _token;

        public TokenModelFaker()
        {
            RuleFor(x => x.Token, f => _token ?? f.Random.String(36));
        }

        public TokenModelFaker WithToken(string token)
        {
            _token = token;
            return this;
        }
    }
}
