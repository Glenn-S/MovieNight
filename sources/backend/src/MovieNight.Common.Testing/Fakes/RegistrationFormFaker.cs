using Bogus;
using MovieNight.Service.Forms;

namespace MovieNight.Common.Testing.Fakes
{
    public class RegistrationFormFaker :
        Faker<RegistrationForm>
    {
        private string _username;
        private string _email;
        private string _password;

        public RegistrationFormFaker()
        {
            RuleFor(x => x.Username, f => _username ?? f.Person.UserName);
            RuleFor(x => x.Password, f => _password ?? f.Random.AlphaNumeric(10));
            RuleFor(x => x.Email, f => _email ?? f.Person.Email);
        }

        public RegistrationFormFaker WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public RegistrationFormFaker WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public RegistrationFormFaker WithEmail(string email)
        {
            _email = email;
            return this;
        }
    }
}
