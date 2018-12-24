using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Momo.Domain.Entities;

namespace Momo.Domain.Commands
{
    public class AddEditShoppingListItemCommand : ICommand
    {
        [Required]
        public int ShoppingListId { get; set; }

        [Required]
        public string Name { get; set; }

        public int? Aisle { get; set; }
    }

    public class AddShoppingListItemCommand : AddEditShoppingListItemCommand
    {
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
        public int? Quantity { get; set; }
    }

    public class EditShoppingListItemCommand : AddEditShoppingListItemCommand
    {
        [Required]
        public int Id { get; set; }

        [Required, Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive.")]
        public int? Quantity { get; set; }

        public decimal? Price { get; set; }
    }

    public class DeleteShoppingListItemCommand : ICommand
    {
        public int Id { get; set; }
    }

    public class ShoppingListItemCommandHandler :
        ICommandHandler<AddShoppingListItemCommand>,
        ICommandHandler<EditShoppingListItemCommand>,
        ICommandHandler<DeleteShoppingListItemCommand>
    {
        public ShoppingListItemCommandHandler(IRepository repository, IValidationFacade validationFacade)
        {
            _repository = repository;
            _validationFacade = validationFacade;
        }

        private readonly IRepository _repository;
        private readonly IValidationFacade _validationFacade;

        public CommandResult Handle(AddShoppingListItemCommand command)
        {
            TrimInput(command);

            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var shoppingList = _repository.Get<ShoppingList>(command.ShoppingListId);
            var item = shoppingList.GetOrAddItem(command.Name);

            item.Picked = false;
            item.Quantity = Math.Max(1, command.Quantity.GetValueOrDefault());
            item.Aisle = command.Aisle.GetValueOrDefault();

            return result;
        }

        public CommandResult Handle(EditShoppingListItemCommand command)
        {
            TrimInput(command);

            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            var shoppingList = _repository.Get<ShoppingList>(command.ShoppingListId);
            var item = shoppingList.ShoppingListItems.Single(x => x.Id == command.Id);

            if (shoppingList.ShoppingListItems.Any(x => x.Id != command.Id && string.Equals(x.Name, command.Name, StringComparison.OrdinalIgnoreCase)))
                return result.Add("Name", "Items must be unique");

            item.Name = command.Name;
            item.Quantity = command.Quantity.GetValueOrDefault();
            item.Aisle = command.Aisle.GetValueOrDefault();
            item.Price = command.Price.GetValueOrDefault();

            result.Data.ShoppingList = shoppingList;

            return result;
        }

        public CommandResult Handle(DeleteShoppingListItemCommand command)
        {
            var result = _validationFacade.Validate(command);
            if (result.AnyErrors())
                return result;

            _repository.Remove<ShoppingListItem>(command.Id);

            return result;
        }

        private void TrimInput(AddEditShoppingListItemCommand command)
        {
            command.Name = (command.Name ?? "").Trim();
        }
    }
}
