using System;
using System.ComponentModel.DataAnnotations;
using Momo.Common.DataAccess;
using Momo.Domain.Entities;

namespace Momo.Domain.Commands
{
    public class AddShoppingListItemCommand : ICommand
    {
        [Required]
        public int ShoppingListId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int? Quantity { get; set; }

        public int? Isle { get; set; }

        public decimal? Price { get; set; }
    }

    public class AddShoppingListItemCommandHandler : ICommandHandler<AddShoppingListItemCommand>
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

            if (command.Isle.HasValue) item.Isle = command.Isle.Value;
            if (command.Price.HasValue) item.Price = command.Price.Value;

            return result;
        }
    }
}
