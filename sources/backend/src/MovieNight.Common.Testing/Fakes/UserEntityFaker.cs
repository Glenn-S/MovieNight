using Bogus;
using MovieNight.Common.Entities;

namespace MovieNight.Common.Testing.Fakes
{
    public class UserEntityFaker :
        Faker<UserEntity>
    {
        private string _id;
        private string _username;
        private string _email;

        public UserEntityFaker()
        {
            RuleFor(x => x.Id, f => _id ?? f.Random.Guid().ToString());
            RuleFor(x => x.UserName, f => _username ?? f.Person.UserName);
            RuleFor(x => x.Email, f => _email ?? f.Person.Email);
        }

        public UserEntityFaker WithId(string id)
        {
            _id = id;
            return this;
        }

        public UserEntityFaker WithUsername(string username)
        {
            _username = username;
            return this;
        }

        public UserEntityFaker WithEmail(string email)
        {
            _email = email;
            return this;
        }
    }
}
