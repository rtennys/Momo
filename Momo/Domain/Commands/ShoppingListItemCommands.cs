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

        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int? Quantity { get; set; }

        public int? Aisle { get; set; }
        public decimal? Price { get; set; }
    }

    public class AddShoppingListItemCommand : AddEditShoppingListItemCommand
    {
    }

    public class EditShoppingListItemCommand : AddEditShoppingListItemCommand
    {
        [Required]
        public int Id { get; set; }
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

            item.Quantity = command.Quantity.GetValueOrDefault();
            item.Picked = false;

            if (command.Aisle.HasValue) item.Aisle = command.Aisle.Value;
            if (command.Price.HasValue) item.Price = command.Price.Value;

            return result;
        }

        public CommandResult Handle(EditShoppingListItemCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var shoppingList = _repository.Get<ShoppingList>(command.ShoppingListId);
            var item = shoppingList.ShoppingListItems.Single(x => x.Id == command.Id);

            item.Name = command.Name;
            item.Quantity = command.Quantity.GetValueOrDefault();
            item.Aisle = command.Aisle.GetValueOrDefault();
            item.Price = command.Price.GetValueOrDefault();

            return result;
        }
    }
}
