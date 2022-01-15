using Bogus;
using MovieNight.Service.Forms;

namespace MovieNight.Common.Testing.Fakes
{
    public class ResetPasswordFormFaker :
        Faker<ResetPasswordForm>
    {
        private string _email;
        private string _token;
        private string _password;
        private string _confirmPassword;

        public ResetPasswordFormFaker()
        {
            var password = new Faker().Random.String(10);
            RuleFor(x => x.Email, f => _email ?? f.Person.Email);
            RuleFor(x => x.Token, f => _token ?? f.Random.String(32));
            RuleFor(x => x.Password, f => _password ?? password);
            RuleFor(x => x.ConfirmPassword, f => _confirmPassword ?? password);
        }

        public ResetPasswordFormFaker WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public ResetPasswordFormFaker WithToken(string token)
        {
            _token = token;
            return this;
        }

        public ResetPasswordFormFaker WithPassword(string password)
        {
            _password = password;
            return this;
        }

        public ResetPasswordFormFaker WithConfirmPassword(string confirmPassword)
        {
            _confirmPassword = confirmPassword;
            return this;
        }
    }
}
