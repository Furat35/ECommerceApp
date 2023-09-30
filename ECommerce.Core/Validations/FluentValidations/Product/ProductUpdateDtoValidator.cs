using ECommerce.Core.DTOs.Product;
using FluentValidation;

namespace ECommerce.Core.Validations.FluentValidations.Product
{
    public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateDtoValidator()
        {
            RuleFor(_ => _.Id)
               .NotEmpty()
               .NotNull()
               .WithMessage("Must contain id!");

            RuleFor(_ => _.Name)
              .NotEmpty().NotNull()
              .WithMessage("Name can't be empty!")
              .MinimumLength(2)
              .WithMessage("Must contain more than 2 charachters!")
                .MaximumLength(40)
              .WithMessage("Can't contain more than 40 charachters!");

            RuleFor(_ => _.Description)
                .NotEmpty().NotNull()
                .WithMessage("Description can't be empty!")
                .MinimumLength(2)
                .WithMessage("Must contain more than 2 charachters!")
                .MaximumLength(100)
                .WithMessage("Can't contain more than 100 charachters!");

            RuleFor(_ => _.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0!")
                .LessThan(100000)
                .WithMessage("Price must be less than 100000!");

            RuleFor(_ => _.CategoryIds)
                .NotEmpty().NotNull()
                .WithMessage("Must contain at least 1 category!");
        }
    }
}
