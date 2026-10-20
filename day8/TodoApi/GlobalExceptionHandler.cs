using Microsoft.AspNetCore.Diagnostics;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger /*, IProblemDetailsService problemDetailsService*/) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandle exception for {Method} {Path}", context.Request.Method, context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return ValueTask.FromResult(false);

        /* 
        return false → 只做副作用(log),回應交給 middleware 預設行為
        middleware 會自己呼叫 IProblemDetailsService 產生 ProblemDetails
        有多個 handler 時會繼續問下一個 —— 這是責任鏈,不是缺點

        return true  → 我接管整個回應,middleware 不再插手
        所以必須自己寫 body,否則客戶端拿到 500 + 空 body
        下面那段就是自己呼叫 IProblemDetailsService 寫出 ProblemDetails
        用的是同一個服務,只是由我發起

        什麼時候才需要 true:要自訂欄位、要針對特定例外給不同狀態碼(例如 TimeoutException → 504)


        我有在 Program.cs 注入 AddProblemDetails(); 就不用再自己自訂 handler 去處理了 
        // return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        // {
        //     HttpContext = context,
        //     ProblemDetails = new ProblemDetails
        //     {
        //         Status = StatusCodes.Status500InternalServerError,
        //         Title = "An error occurred while processing your request."
        //     }
        // });

        通常什麼時候會需要使用要自訂的 handle, 當你要偏離預設行為時
        1. 把特定例外型別對應到不同狀態碼
        if(exception is TimeoutException) 
        {
            context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
            // ... 寫回應
            return true; // 我處理了
        }
        return false; // 其他例外交給預設的 500
        */
    }
}