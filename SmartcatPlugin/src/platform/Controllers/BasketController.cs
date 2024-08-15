using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data.Managers;
using Sitecore.Globalization;
using Sitecore.Tasks;
using SmartcatPlugin.Cache;
using SmartcatPlugin.Models.Dtos;
using SmartcatPlugin.Services;

namespace SmartcatPlugin.Controllers
{
    [RoutePrefix("api/basket")]
    public class BasketController : ApiController
    {
        [Route("get-selected-items")]
        [HttpGet]
        public IHttpActionResult GetSelectedItems()
        {
            var basketService = new BasketService();

            var result = basketService.BuildSelectedItemTree();

            return Json(result);
        }

        [Route("update-ribbon")]
        [HttpGet]
        public IHttpActionResult UpdateRibbon()
        {
            Thread.Sleep(5000);
            Sitecore.Context.ClientPage.ClientResponse.Timer("item:refresh", 2000);

            return Ok();
        }

        [Route("get-validating-info")]
        [HttpGet]
        public IHttpActionResult GetValidatingInfo()
        {

            string cachedData = CustomCacheManager.GetCache("selectedItems");
            if (string.IsNullOrEmpty(cachedData))
            {
                return Ok(new List<string>()); // todo: exception
            }

            var itemIds = JsonConvert.DeserializeObject<List<string>>(cachedData);
            var itemService = new ItemService();

            var invalidItemNames = itemService.GetInvalidItemsNames(itemIds);

            var validatingInfo = new ValidatingInfoDto
            {
                InvalidItemNames = string.Join(", ", invalidItemNames),
                InvalidItemCount = invalidItemNames.Count,
                ValidItemCount = itemIds.Count - invalidItemNames.Count
            };

            return Ok(validatingInfo);
        }

        [Route("save-project-info")]
        [HttpPost]
        public IHttpActionResult SaveProjectInfo([FromBody] SaveProjectInfoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Fill required fields");
            }
            var projectInfo = JsonConvert.SerializeObject(dto);
            CustomCacheManager.SetCache("projectInfo", projectInfo);

            return Ok();
        }

        [Route("get-saved-project-info")]
        [HttpGet]
        public IHttpActionResult GetSavedProjectInfo()
        {
            string cachedData = CustomCacheManager.GetCache("projectInfo");
            if (string.IsNullOrEmpty(cachedData))
            {
                return Ok(new List<string>()); // todo: exception
            }

            var projectInfo = JsonConvert.DeserializeObject<SaveProjectInfoDto>(cachedData);

            return Ok(projectInfo);
        }

        [Route("get-translation-languages")]
        [HttpGet]
        public IHttpActionResult GetTranslationLanguages()
        {
            var result = new TranslationLanguagesDto
            {
                SourceLanguages = new List<LanguageDto>
                {
                    new LanguageDto
                    {
                        Name = "English", Code = "en"
                    },
                    new LanguageDto
                    {
                        Name = "Russian", Code = "ru"
                    }
                },
                TargetLanguages = new List<LanguageDto>
                {
                    new LanguageDto
                    {
                        Name = "English", Code = "en"
                    },
                    new LanguageDto
                    {
                        Name = "Russian", Code = "ru"
                    },
                    new LanguageDto
                    {
                        Name = "Spanish", Code = "es"
                    },
                    new LanguageDto
                    {
                        Name = "French", Code = "fr"
                    },
                    new LanguageDto
                    {
                        Name = "German", Code = "de"
                    },
                }
            };

            return Ok(result);
        }

        /*[Route("confirm-project")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateSmartcatProject()
        {

        }*/
    }
}