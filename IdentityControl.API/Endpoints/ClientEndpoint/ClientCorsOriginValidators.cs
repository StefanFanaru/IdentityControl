// using System.Linq;
// using FluentValidation;
//
// namespace IdentityControl.API.Endpoints.ClientEndpoint
// {
//     public static class ClientCorsOriginValidators
//     {
//         public class InsertClientCorsOriginValidator : AbstractValidator<InsertClientCorsOriginRequest>
//         {
//             public InsertClientCorsOriginValidator()
//             {
// RuleFor(x => x.Origin).NotEmpty().MaximumLength(100).Custom((origin, context) =>
// {
//     if (!origin.Contains("http://") || !origin.Contains("https://") || origin.Count(x => x == '/') > 2)
//     {
//         context.AddFailure("Invalid Origin address");
//     }
// });
//             }
//         }
//
//         public class UpdateClientCorsOriginValidator : AbstractValidator<UpdateClientCorsOriginRequest>
//         {
//             public UpdateClientCorsOriginValidator()
//             {
//                 RuleFor(x => x.Origin).NotEmpty().MaximumLength(100).Custom((origin, context) =>
//                 {
//                     if (!origin.Contains("http://") || !origin.Contains("https://") || origin.Count(x => x == '/') > 2)
//                     {
//                         context.AddFailure("Invalid Origin address");
//                     }
//                 });
//             }
//         }
//     }
// }

