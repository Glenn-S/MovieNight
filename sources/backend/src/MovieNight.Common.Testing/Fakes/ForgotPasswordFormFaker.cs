using Bogus;
using MovieNight.Service.Forms;

namespace MovieNight.Common.Testing.Fakes
{
    public class ForgotPasswordFormFaker :
        Faker<ForgotPasswordForm>
    {
        private string _email;

        public ForgotPasswordFormFaker()
        {
            RuleFor(x => x.Email, f => _email ?? f.Person.Email);
        }

        public ForgotPasswordFormFaker WithEmail(string email)
        {
            _email = email;
            return this;
        }
    }
}
