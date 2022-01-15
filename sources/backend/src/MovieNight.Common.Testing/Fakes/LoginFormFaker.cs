using Bogus;
using MovieNight.Service.Forms;

namespace MovieNight.Common.Testing.Fakes
{
    public class LoginFormFaker :
        Faker<LoginForm>
    {
        private string _username;
        private string _password;

        public LoginFormFaker()
        {
            RuleFor(x => x.Username, f => _username ?? f.Person.UserName);
            RuleFor(x => x.Password, f => _password ?? f.Random.AlphaNumeric(10));
        }

        public LoginFormFaker WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public LoginFormFaker WithPassword(string password)
        {
            _password = password;
            return this;
        }
    }
}
