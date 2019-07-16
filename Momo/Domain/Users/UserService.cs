using System;
using System.Linq;
using Momo.Domain.Entities;

namespace Momo.Domain.Users
{
    public interface IUserService
    {
        DomainResult AddUser(AddUserModel model);
    }

    public sealed class UserService : IUserService
    {
        public UserService(IRepository repository, IValidationFacade validationFacade)
        {
            _repository = repository;
            _validationFacade = validationFacade;
        }

        private readonly IRepository _repository;
        private readonly IValidationFacade _validationFacade;

        public DomainResult AddUser(AddUserModel model)
        {
            var result = _validationFacade.Validate(model);
            if (result.AnyErrors())
                return result;

            if (_repository.Find<UserProfile>().Any(x => x.Username == model.Username))
                return result.Add("Username", "Username already exists");

            _repository.Add(new UserProfile(model.Username));

            return result;
        }
    }
}
