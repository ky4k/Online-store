﻿using FluentValidation;
using HM.BLL.Models.Products;

namespace HM.BLL.Validators;

public class ProductInstanceCreateDtoValidator : AbstractValidator<ProductInstanceCreateDto>
{
    public ProductInstanceCreateDtoValidator()
    {
        RuleFor(product => product.Price)
            .GreaterThan(0)
                .WithMessage("Product price must be greater than 0.")
            .LessThan(1000000)
                .WithMessage("Product price must be less than 1000000.");

        RuleFor(product => product.StockQuantity)
            .GreaterThanOrEqualTo(0)
                .WithMessage("Stock Quantity must be greater than or equal to 0.");

        RuleFor(product => product.SKU)
            .MinimumLength(3)
                .WithMessage("Stock keeping unit (SKU) must be at least 3 characters long.")
            .MaximumLength(50)
                .WithMessage("Stock keeping unit (SKU) cannot be longer than 50 characters.")
            .Matches(@"^[\p{L}0-9\s#\/\-():_]+$")
                .WithMessage("Stock keeping unit (SKU) contain only letters, numbers, spaces " +
                    "and characters #/\\-():_.");

        RuleFor(product => product.AbsoluteDiscount)
            .Must((p, d) => d >= 0 && d <= p.Price)
                .WithMessage("Absolute discount must be between 0 and the product price.");

        RuleFor(product => product.PercentageDiscount)
            .InclusiveBetween(0, 100)
                .WithMessage("Percentage discount must be between 0 and 100.");

        RuleFor(product => product.Color)
            ;

        RuleFor(product => product.Size)
            ;
    }
}
