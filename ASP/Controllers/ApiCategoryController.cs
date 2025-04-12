using ASP.Data;
using ASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class ApiCategoryController : ControllerBase
    {
        private readonly DataAccessor _dataAccessor;

        public ApiCategoryController(DataAccessor dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }
        [HttpGet]
        public RestResponse Categories() // название произвольное. маршрутизация по [HttpGet]
        {
            return new()
            {
                Service = "Api Products Categories",
                DataType = "array",
                CacheTime = 600,
                Data = _dataAccessor.AllCategories()
            };
        }

        [HttpGet("{id}")]
        public RestResponse Category(string id)
        {
            return new()
            {
                Service = "Api Products Categories",
                DataType = "object",
                CacheTime = 600,
                Data = _dataAccessor.GetCategory(id)
            };
        }
    }
}

/*
 * CORS policy (CORP) https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-9.0
 * Cross-Origin Resource Sharing - правила междоменного обмена ресурсами
 * Данные от сервера с другого хоста/порта/протокола (http/https) должны быть проигнорированы если в них нет
 * обозначений о:
 * общий доступ - Access-Control-Allow-Origin
 * разрешенные методы - Access-Control-Allow-Methods
 * разрешенные заголовки - Access-Control-Allow-Headers
 * + перед отправкой основного запроса (кроме GET) необходимо отправить предыдущий запрос (preflight) методом OPTIONS,
 * получить ведомости о CORS и, при их отсутствии или несоответсвии, не отправлять основной запрос.
 *
 * 
 * MVC Controller:                  API Controller
 * одинаковые методы запроса        Одинаковые адреса
 * маршрутизация по названиям       маршрутизация по методам запроса
 * GET /Shop/Category               GET  /api/category
 * GET /Shop/Cart                   POST /api/category
 * GET /Shop/Product                PUT  /api/category
 * Возрват IActionResult            Возврат object/string
 * чаще всего View(html)            Чаще всего JSON (object)
 *
 * Большая функциональность,        Меньшая функциональность, легче
 * сложнее
 */
