using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;

namespace Momo.Domain.Commands
{
    public class AddEditShoppingListItemCommand : ICommand
    {
        [Required]
        public int ShoppingListId { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class AddShoppingListItemCommand : AddEditShoppingListItemCommand
    {
    }

    public class EditShoppingListItemCommand : AddEditShoppingListItemCommand
    {
        [Required]
        public int Id { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
        public int? Quantity { get; set; }

        public int? Aisle { get; set; }

        public decimal? Price { get; set; }
    }

    public class AddShoppingListItemCommandHandler : ICommandHandler<AddShoppingListItemCommand>, ICommandHandler<EditShoppingListItemCommand>
    {
        public AddShoppingListItemCommandHandler(IRepository repository, IValidationFacade validationFacade)
        {
            _repository = repository;
            _validationFacade = validationFacade;
        }

        private readonly IRepository _repository;
        private readonly IValidationFacade _validationFacade;

        public CommandResult Handle(AddShoppingListItemCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var shoppingList = _repository.Get<ShoppingList>(command.ShoppingListId);
            var item = shoppingList.GetOrAddItem(command.Name);

            if (item.Quantity <= 0) item.Quantity = 1;

            item.Picked = false;

            result.Data.Item = item;

            return result;
        }

        public CommandResult Handle(EditShoppingListItemCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var shoppingList = _repository.Get<ShoppingList>(command.ShoppingListId);
            var item = shoppingList.ShoppingListItems.Single(x => x.Id == command.Id);

            if (shoppingList.ShoppingListItems.Any(x => x.Id != command.Id && string.Equals(x.Name, command.Name, StringComparison.OrdinalIgnoreCase)))
                return result.Add("Name", "List item name must be unique");

            item.Name = command.Name;
            item.Quantity = command.Quantity.GetValueOrDefault();
            item.Aisle = command.Aisle.GetValueOrDefault();
            item.Price = command.Price.GetValueOrDefault();

            result.Data.ShoppingList = shoppingList;

            return result;
        }
    }
}
