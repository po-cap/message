using Po.Api.Response;

namespace Message.Presentation.Utilities;

public static class HttpContextExtension
{
    public static long UserID(this HttpContext context)
    {
        var sub = context.User.FindFirst("sub")?.Value;
        if(sub == null)
            throw Failure.Unauthorized();
    
        if(!long.TryParse(sub, out var userId))
            throw Failure.Unauthorized();

        return userId;
    }
    
}