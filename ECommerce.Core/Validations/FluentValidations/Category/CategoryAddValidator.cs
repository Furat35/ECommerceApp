using ECommerce.Core.DTOs.Category;
using FluentValidation;

namespace ECommerce.Core.Validations.FluentValidations.Category
{
    public class CategoryAddValidator : AbstractValidator<CategoryAddDto>
    {
        public CategoryAddValidator()
        {
            RuleFor(_ => _.CategoryName)
            .NotEmpty().NotNull()
            .WithMessage("Name can't be empty!")
            .MinimumLength(2)
            .WithMessage("Must contain more than 2 charachters!")
            .MaximumLength(40)
            .WithMessage("Can't contain more than 40 charachters!");
        }
    }
}
