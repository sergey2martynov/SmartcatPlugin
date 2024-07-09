using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.Authentication;
using SmartcatPlugin.Models.Smartcat.GetFolderList;
using SmartcatPlugin.Models.Smartcat.GetItemById;
using SmartcatPlugin.Models.Smartcat.GetItemContent;
using SmartcatPlugin.Models.Smartcat.GetItemList;
using SmartcatPlugin.Models.Smartcat.GetParentDirectories;
using SmartcatPlugin.Models.Smartcat.ImportTranslation;
using SmartcatPlugin.Services;
using SmartcatPlugin.Tools;

namespace SmartcatPlugin.Controllers
{
    [Authorize]
    [RoutePrefix("smartcat")]
    public class PageController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");

        [Route("authenticate")]
        [HttpPost]
        public IHttpActionResult Authenticate(HttpRequestMessage request)
        {
            var headers = request.Headers;

            if (!headers.Contains("Authorization"))
            {
                return Json(ApiResponse.Return(401, "Unauthorized"));
            }

            var encryptedToken = headers.GetValues("Authorization").FirstOrDefault();

            string decryptedToken;

            using (var rsa = new RSACryptoServiceProvider())
            {
                var publicKey = Sitecore.Configuration.Settings.GetSetting("Smartcat.PublicKey");

                rsa.FromXmlString(publicKey);

                byte[] encryptedBytes = Convert.FromBase64String(encryptedToken);
                byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, false);
                decryptedToken = Encoding.UTF8.GetString(decryptedBytes);
            }

            var token = JsonConvert.DeserializeObject<SmartcatToken>(decryptedToken);

            if (string.IsNullOrEmpty(token.SmartcatAuthKey))
            {
                return Json(ApiResponse.Return(400, "SmartcatAuthKey can not be empty"));
            }

            var authenticateService = new AuthenticationService();

            var tokenItem = authenticateService.SaveToken(token.SmartcatAuthKey);

            return Json(ApiResponse.Return(200, "Authenticated"));
        }

        [Route("directory-list")]
        [HttpPost]
        public IHttpActionResult GetFolderList([FromBody] GetFolderListRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalId.ToLower() == ConstantIds.Root)
            {
                rootItem = _masterDb.GetItem("/sitecore/content/");
            }
            else
            {
                var id = new ID(request.ParentDirectoryId.ExternalId);
                rootItem = _masterDb.GetItem(id);
            }

            if (rootItem == null)
            {
                return NotFound();
            }

            var getDataDirectoriesResponse = new GetFolderListResponse
            {
                NextBatchKey = null,
                Directories = rootItem.GetChildDirectories()
            };

            return Json(getDataDirectoriesResponse);
        }

        [Route("file-list")]
        [HttpPost]
        public IHttpActionResult GetItemList([FromBody] GetItemsListRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalType != ConstantItemTypes.Folder)
            {
                return Json(GetItemListResponse.Empty);
            }

            if (request.ParentDirectoryId.ExternalId.ToLower() == ConstantIds.Root)
            {
                rootItem = _masterDb.GetItem("/sitecore/content/");
            }
            else
            {
                var id = new ID(request.ParentDirectoryId.ExternalId);
                rootItem = _masterDb.GetItem(id);
            }

            if (rootItem == null)
            {
                return NotFound();
            }

            var getDataItemsResponse = new GetItemListResponse
            {
                NextBatchKey = null,
                Items = rootItem.GetChildPages(request.SearchQuery, _masterDb)
            };

            return Json(getDataItemsResponse);
        }

        [Route("file-content")]
        [HttpPost]
        public IHttpActionResult GetItemContent([FromBody] GetItemContentRequest request)
        {
            var id = new ID(request.ItemId.ExternalId);
            var item = _masterDb.GetItem(id, Language.Parse(request.SourceLocale));

            if (!item.IsHaveLayout())
            {
                return Json(new Dictionary<string, LocJsonContent>());
            }

            var result = item.GetPageContent(_masterDb, request);

            return Json(new GetItemContentResponse{LocaleContent = result});
        }

        [Route("import-translation")]
        [HttpPost]
        public IHttpActionResult ImportTranslation([FromBody] TranslationImportRequest request)
        {
            foreach (var locale in request.LocaleContent)
            {
                Language newLanguage = Language.Parse(locale.Key);
                var locJson = locale.Value;
                var newVersionItemIds = new List<ID>();

                foreach (var unit in locJson.Units)
                {
                    var itemIdFieldName = IdFieldExtractor.ExtractIdAndField(unit.Key);
                    var childrenPageId = new ID(itemIdFieldName.Id);
                    var allVersionsItem = _masterDb.GetItem(childrenPageId);
                    bool languageVersionExists = allVersionsItem.Languages.Any(l => l == newLanguage);

                    var newLanguageItem = _masterDb.GetItem(childrenPageId, newLanguage);

                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        if (languageVersionExists && !newVersionItemIds.Contains(newLanguageItem.ID))
                        {
                            newLanguageItem = newLanguageItem.Versions.AddVersion();
                        }

                        newVersionItemIds.Add(newLanguageItem.ID);
                        newLanguageItem.Editing.BeginEdit();
                        newLanguageItem.Fields[itemIdFieldName.Field].Value = string.Join(string.Empty, unit.Target);
                        newLanguageItem.Editing.EndEdit();
                    }
                }
            }

            return Json(ApiResponse.Success);
        }

        [Route("parent-directories-by-id")]
        [HttpPost]
        public IHttpActionResult GetParentFolders([FromBody] GetParentFoldersRequest request)
        {
            var parentFolders = new List<GetParentDirectoriesResponseItem>();

            foreach (var directoryExternalId in request.DirectoryIds)
            {
                var folderId = new ID(directoryExternalId.ExternalId);
                var folder = _masterDb.GetItem(folderId);

                if (!folder.IsFolder())
                {
                    var response = ApiResponse.Error;
                    response.ErrorCode = 400;
                    response.ErrorMessage = $"Item with id: {directoryExternalId.ExternalId} is not folder";
                    return Json(response);
                }

                parentFolders.Add(new GetParentDirectoriesResponseItem
                {
                    DirectoryId = directoryExternalId,
                    ParentDirectoryId = new ExternalObjectId
                    {
                        ExternalId = folder.ParentID.ToString(),
                        ExternalType = ConstantItemTypes.Folder
                    }
                });
            }

            return Json(new GetParentFoldersResponse{Items = parentFolders });
        }

        [Route("files-by-id")]
        [HttpPost]
        public IHttpActionResult GetItemsById([FromBody] GetItemByIdRequest request)
        {
            var itemsData = new List<DataItem>();

            foreach (var itemId in request.ItemIds)
            {
                var item = _masterDb.GetItem(new ID(itemId.ExternalId));

                if (!item.IsHaveLayout())
                {
                    var response = ApiResponse.Error;
                    response.ErrorCode = 400;
                    response.ErrorMessage = $"Item with id: {itemId.ExternalId} is not page";
                    return Json(response);
                }

                var itemData = new DataItem
                {
                    Id = new ExternalObjectId
                        { ExternalId = item.ID.ToString(), ExternalType = ConstantItemTypes.Page },
                    ParentDirectoryIds = new List<ExternalObjectId>
                    {
                        new ExternalObjectId
                            { ExternalId = item.ParentID.ToString(), ExternalType = ConstantItemTypes.Folder }
                    },
                    Name = item.Name,
                    Locales = item.GetItemLocales(_masterDb)
                };

                itemsData.Add(itemData);
            }

            return Json(new GetItemByIdResponse{Items = itemsData});
        }
    }
}