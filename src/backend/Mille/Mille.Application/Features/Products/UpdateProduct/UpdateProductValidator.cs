using FluentValidation;

namespace Mille.Application.Features.Products.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Valid CategoryId is required.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status.");

            RuleForEach(x => x.Variants).ChildRules(v =>
            {
                v.RuleFor(x => x.SKU)
                    .NotEmpty().WithMessage("SKU is required.")
                    .MaximumLength(100);
                v.RuleFor(x => x.Price)
                    .GreaterThan(0).WithMessage("Price must be greater than 0.");
                v.RuleFor(x => x.Stock)
                    .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
                v.RuleFor(x => x.Size).MaximumLength(20);
                v.RuleFor(x => x.Color).MaximumLength(50);
            });
        }
    }
}