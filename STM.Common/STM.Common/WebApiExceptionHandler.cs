using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security;
using System.Net.Http;
using System.Data.SqlClient;
using System.Web.Http;
using System.Data.Entity.Validation;

namespace STM.Common
{
    public class WebApiExceptionHandler
    {
        public static void SetupExceptionHandlers()
        {
            GlobalConfiguration.Configuration.Filters.Add(
                new UnhandledExceptionFilterAttribute()
                .Register<KeyNotFoundException>(HttpStatusCode.NotFound)
                .Register<SecurityException>(HttpStatusCode.Forbidden)
                .Register<ArgumentException>(HttpStatusCode.BadRequest)
                .Register<ArgumentNullException>(HttpStatusCode.BadRequest)
                .Register<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest)
                .Register<DivideByZeroException>(HttpStatusCode.InternalServerError)
                .Register<DllNotFoundException>(HttpStatusCode.InternalServerError)
                .Register<FormatException>(HttpStatusCode.BadRequest)
                .Register<HttpRequestException>(HttpStatusCode.BadRequest)
                .Register<InvalidOperationException>(HttpStatusCode.InternalServerError)
                .Register<NotImplementedException>(HttpStatusCode.InternalServerError)
                .Register<NotSupportedException>(HttpStatusCode.MethodNotAllowed)
                .Register<NullReferenceException>(HttpStatusCode.InternalServerError)
                .Register<ProtocolViolationException>(HttpStatusCode.InternalServerError)
                .Register<TimeoutException>(HttpStatusCode.RequestTimeout)
                .Register<DbEntityValidationException>((exception, request) =>
                {
                    var efException = exception as DbEntityValidationException;
                    var response = request.CreateResponse(HttpStatusCode.InternalServerError);
                    response.ReasonPhrase = string.Empty;
                    foreach (var valError in efException.EntityValidationErrors)
                    {
                        foreach (var err in valError.ValidationErrors)
                        {
                            response.ReasonPhrase += string.Format("Error {0} for property {1}." + Environment.NewLine, err.ErrorMessage, err.PropertyName);
                        }
                    }
                    return response;
                })
                .Register<SqlException>((exception, request) =>
                {
                    var sqlException = exception as SqlException;

                    if (sqlException.Number > 50000)
                    {
                        var response = request.CreateResponse(HttpStatusCode.BadRequest);
                        response.ReasonPhrase = sqlException.Message.Replace(Environment.NewLine, String.Empty);

                        return response;
                    }
                    else
                    {
                        return request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }));

        }
    }
}
