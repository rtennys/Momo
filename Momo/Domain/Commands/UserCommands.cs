using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Momo.Domain.Entities;

namespace Momo.Domain.Commands
{
    public class AddUserCommand : ICommand
    {
        [Required, RegularExpression(@"^[A-Za-z]+[A-Za-z0-9-]*$", ErrorMessage = "Start with a letter and then letters, numbers, and dashes only")]
        public string Username { get; set; }
    }

    public class AddUserCommandHandler : ICommandHandler<AddUserCommand>
    {
        public AddUserCommandHandler(IRepository repository, IValidationFacade validationFacade)
        {
            _repository = repository;
            _validationFacade = validationFacade;
        }

        private readonly IRepository _repository;
        private readonly IValidationFacade _validationFacade;

        public CommandResult Handle(AddUserCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            if (_repository.Find<UserProfile>().Any(x => x.Username == command.Username))
                return result.Add("Username", "Username already exists");

            var user = new UserProfile(command.Username);

            _repository.Add(user);

            result.Data.Id = user.Id;

            return result;
        }
    }
}
