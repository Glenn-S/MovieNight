using Bogus;
using MovieNight.Service.Forms;

namespace MovieNight.Common.Testing.Fakes
{
    public class VerifyEmailFormFaker :
        Faker<VerifyEmailForm>
    {
        private string _email;
        private string _token;

        public VerifyEmailFormFaker()
        {
            RuleFor(x => x.Email, f => _email ?? f.Person.Email);
            RuleFor(x => x.Token, f => _token ?? f.Random.AlphaNumeric(32));
        }

        public VerifyEmailFormFaker WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public VerifyEmailFormFaker WithToken(string token)
        {
            _token = token;
            return this;
        }
    }
}
