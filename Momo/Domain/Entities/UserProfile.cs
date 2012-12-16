namespace Momo.Domain.Entities
{
    public class UserProfile : EntityBase
    {
        protected UserProfile()
        {
        }

        public UserProfile(string userName)
        {
            UserName = userName;
        }

        public virtual string UserName { get; protected set; }
    }
}