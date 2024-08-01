using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using SmartcatPlugin.Constants;
using SmartcatPlugin.Extensions;
using SmartcatPlugin.Models.ApiResponse;
using SmartcatPlugin.Models.Smartcat;
using SmartcatPlugin.Models.Smartcat.Authorization;
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
    [RoutePrefix("smartcat")]
    public class PageController : ApiController
    {
        private readonly Database _masterDb = Database.GetDatabase("master");
        private readonly ILog _log = LogManager.GetLogger(LogNames.SmartcatApi);

        [Route("authorize")]
        [HttpPost]
        public IHttpActionResult Authorize([FromBody] AuthorizationRequest request)
        {

            if (string.IsNullOrEmpty(request.Token))
            {
                return Json(ApiResponse.Error(400, "Token can not be null or empty"));
            }

            var encryptedToken = request.Token;

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
                return Json(ApiResponse.Error(400, "SmartcatAuthKey can not be empty"));
            }

            var authenticateService = new AuthenticationService();

            authenticateService.SaveToken(token.SmartcatAuthKey);

            _log.Info("Authorization was success completed");

            return Json(ApiResponse.Error(200, "Authenticated"));
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
                return Json(ApiResponse.Error(404,
                    $"Parent folder with Id:{request.ParentDirectoryId} not found"));
            }

            var getDataDirectoriesResponse = new GetFolderListResponse
            {
                NextBatchKey = null,
                Directories = rootItem.GetChildDirectories()
            };

            _log.Info("SmartcatApi method \"directory-list\" was success completed");
            return Json(getDataDirectoriesResponse);
        }

        [Route("file-list")]
        [HttpPost]
        public IHttpActionResult GetItemList([FromBody] GetItemsListRequest request)
        {
            Item rootItem;

            if (request.ParentDirectoryId.ExternalType != ConstantItemTypes.Directory 
                && request.ParentDirectoryId.ExternalId.ToLower() != ConstantIds.Root)
            {
                return Json(ApiResponse.Error(400,
                    $"Item with Id:{request.ParentDirectoryId.ExternalId} is not directory"));
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
                return Json(ApiResponse.Error(404,
                    $"Parent folder with Id:{request.ParentDirectoryId.ExternalId} not found"));
            }

            var getDataItemsResponse = new GetItemListResponse
            {
                NextBatchKey = null,
                Items = rootItem.GetChildPages(request.SearchQuery, _masterDb)
            };

            _log.Info("SmartcatApi method \"file-list\" was success completed");
            return Json(getDataItemsResponse);
        }

        [Route("file-content")]
        [HttpPost]
        public IHttpActionResult GetItemContent([FromBody] GetItemContentRequest request)
        {
            var id = new ID(request.ItemId.ExternalId);
            var item = _masterDb.GetItem(id, Language.Parse(request.SourceLocale));

            if (!item.IsHasContentFields())
            {
                _log.Error($"Item {item.Name} with {item.ID} is not Item");  
                return Json(new Dictionary<string, LocJsonContent>());
            }

            var result = item.GetItemContent(_masterDb, request);

            _log.Info("SmartcatApi method \"file-content\" was success completed");
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
                    var fieldName = unit.Key;
                    var childrenPageId = new ID(request.ItemId.ExternalId);
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
                        newLanguageItem.Fields[fieldName].Value = string.Join(string.Empty, unit.Target);
                        newLanguageItem.Editing.EndEdit();

                        _log.Info($"{typeof(Item)} with Id {newLanguageItem.ID}," +
                                  $" locale {newLanguageItem.Language}," +
                                  $" new version {newLanguageItem.Version} was created");
                    }
                }
            }

            _log.Info("SmartcatApi method \"import-translation\" was success completed");
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
                    return Json(ApiResponse.Error(400,
                        $"Item with id: {directoryExternalId.ExternalId} is not folder"));
                }

                parentFolders.Add(new GetParentDirectoriesResponseItem
                {
                    DirectoryId = directoryExternalId,
                    ParentDirectoryId = new ExternalObjectId
                    {
                        ExternalId = folder.ParentID.ToString(),
                        ExternalType = ConstantItemTypes.Directory
                    }
                });
            }

            _log.Info("SmartcatApi method \"parent-directories-by-id\" was success completed");
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

                if (!item.IsHasContentFields())
                {
                    return Json(ApiResponse.Error(400,
                        $"Item with id: {itemId.ExternalId} is not page"));
                }

                var itemData = new DataItem
                {
                    Id = new ExternalObjectId
                    {
                        ExternalId = item.ID.ToString(),
                        ExternalType = ConstantItemTypes.Item
                    },
                    ParentDirectoryIds = new List<ExternalObjectId>
                    {
                        new ExternalObjectId
                        {
                            ExternalId = item.ParentID.ToString(),
                            ExternalType = ConstantItemTypes.Directory
                        }
                    },
                    Name = item.Name,
                    Locales = item.GetItemLocales(_masterDb)
                };

                itemsData.Add(itemData);
            }

            _log.Info("SmartcatApi method \"files-by-id\" was success completed");
            return Json(new GetItemByIdResponse{Items = itemsData});
        }
    }
}