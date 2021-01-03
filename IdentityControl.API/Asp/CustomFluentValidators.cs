using FluentValidation;

namespace IdentityControl.API.Asp
{
    public static class CustomFluentValidators
    {
        public static IRuleBuilderOptions<T, string> MustBeLink<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            // TODO: add regex patterns
            // simplified without a pattern, to allow localhost urls (for now)
            return ruleBuilder.Must(m => m != null && (m.StartsWith("http://") || m.StartsWith("https://")))
                .WithMessage("'{PropertyName}' is not a valid URI");
        }
    }
}