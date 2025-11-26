using Microsoft.AspNetCore.Mvc;

namespace Server.Helpers.Extensions;
public static class ControllerBaseExtensions {
    public static ObjectResult InternalServerError(this ControllerBase controller) =>
        controller.InternalServerError(null);
    public static ObjectResult InternalServerError(this ControllerBase controller, object? value) =>
        new(value) { StatusCode = StatusCodes.Status500InternalServerError };
}
