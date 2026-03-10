using Microsoft.AspNetCore.Mvc;
using Yusr.Core.Abstractions.Enums;
using Yusr.Core.Abstractions.Primitives;
using Yusr.Identity.Abstractions.Primitives;

namespace Yusr.Api.Abstractions.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<ActionResult<T>> ExecuteAsync<T>(this ControllerBase controller, Func<JwtClaims, Task<OperationResult<T>>> Action, Func<ActionResult>? OnUnauthorized = null) where T : new()
        {
            var jwtClaimsRes = JwtClaims.ExtractClaims(controller.User);
            if (jwtClaimsRes.ResultType != ResultType.Ok)
            {
                if (OnUnauthorized is not null)
                    return OnUnauthorized();
                else
                    return controller.Unauthorized("ClientId is missing or invalid.");
            }

            var operationResult = await Action(jwtClaimsRes.Result);
            return operationResult.HandleRequest();
        }

        public static async Task<IActionResult> ExecuteAsync(this ControllerBase controller, Func<JwtClaims, Task<IActionResult>> Action, Func<ActionResult>? OnUnauthorized = null)
        {
            var jwtClaimsRes = JwtClaims.ExtractClaims(controller.User);
            if (jwtClaimsRes.ResultType != ResultType.Ok)
            {
                if (OnUnauthorized is not null)
                    return OnUnauthorized();
                else
                    return controller.Unauthorized("ClientId is missing or invalid.");
            }

            return await Action(jwtClaimsRes.Result);
        }
    }
}
