using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Momo.Common;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;

namespace Momo.Domain.Commands
{
    public class AddEditShoppingListCommandBase : ICommand
    {
        [Required]
        public string Username { get; set; }

        [Required, RegularExpression(@"^[A-Za-z]+[A-Za-z0-9-]*$", ErrorMessage = "Username may only contain alphanumeric characters or dashes and must begin with a letter")]
        public string Name { get; set; }
    }

    public class AddShoppingListCommand : AddEditShoppingListCommandBase
    {
    }

    public class RenameShoppingListCommand : AddEditShoppingListCommandBase
    {
        [Required]
        public int Id { get; set; }
    }

    public class DeleteShoppingListCommand : ICommand
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public int Id { get; set; }
    }

    public class ShoppingListCommandHandler : ICommandHandler<AddShoppingListCommand>, ICommandHandler<RenameShoppingListCommand>, ICommandHandler<DeleteShoppingListCommand>
    {
        public ShoppingListCommandHandler(IRepository repository, IValidationFacade validationFacade)
        {
            _repository = repository;
            _validationFacade = validationFacade;
        }

        private readonly IRepository _repository;
        private readonly IValidationFacade _validationFacade;

        public CommandResult Handle(AddShoppingListCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var user = _repository.Get<UserProfile>(x => x.Username == command.Username);

            if (user.ShoppingLists.Any(x => string.Equals(x.Name, command.Name, StringComparison.OrdinalIgnoreCase)))
                result.Add(command.GetName(x => x.Name), "Name Must Be Unique");
            else
                user.AddShoppingList(command.Name);

            return result;
        }

        public CommandResult Handle(RenameShoppingListCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var user = _repository.Get<UserProfile>(x => x.Username == command.Username);
            var shoppingList = user.ShoppingLists.SingleOrDefault(x => x.Id == command.Id);

            if (shoppingList == null)
                result.Add(command.GetName(x => x.Id), "Shopping List Not Found");
            else if (!string.Equals(shoppingList.Name, command.Name, StringComparison.OrdinalIgnoreCase) && user.ShoppingLists.Any(x => string.Equals(x.Name, command.Name, StringComparison.OrdinalIgnoreCase)))
                result.Add(command.GetName(x => x.Name), "Name Must Be Unique");
            else
                shoppingList.Name = command.Name;

            return result;
        }

        public CommandResult Handle(DeleteShoppingListCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var user = _repository.Get<UserProfile>(x => x.Username == command.Username);
            var shoppingList = user.ShoppingLists.SingleOrDefault(x => x.Id == command.Id);

            if (shoppingList == null)
                result.Add(command.GetName(x => x.Id), "Shopping List Not Found");
            else
                user.Remove(shoppingList);

            return result;
        }
    }
}
