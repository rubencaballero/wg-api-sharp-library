﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WGSharpAPI.Entities;
using WGSharpAPI.Enums;
using WGSharpAPI.Interfaces;

namespace WGSharpAPI
{
    public class WGApplication : IWGApplication
    {
        private string _defaultApiURI = @"api.worldoftanks.eu/wot";
        private string _applicationId;
        private WGSettings _settings;

        #region Constructors

        public WGApplication(string applicationId)
        {
            _applicationId = applicationId;
            _settings = new WGSettings();
        }

        public WGApplication(string applicationId, WGSettings settings)
            : this(applicationId)
        {
            _settings = settings;
        }

        public WGApplication(string applicationId, WGSettings settings, string apiURI)
            : this(applicationId, settings)
        {
            _defaultApiURI = apiURI;
        }

        #endregion Constructors

        #region Search Players

        /// <summary>
        /// Method returns partial list of players. The list is filtered by initial characters of user name and sorted alphabetically.
        /// </summary>
        /// <param name="searchTerm">search string</param>
        /// <returns></returns>
        public WGResponse<List<Player>> SearchPlayers(string searchTerm)
        {
            return SearchPlayers(searchTerm, WGPlayerListField.All, WGLanguageField.EN, 100);
        }

        /// <summary>
        /// Method returns partial list of players. The list is filtered by initial characters of user name and sorted alphabetically.
        /// </summary>
        /// <param name="searchTerm">search string</param>
        /// <param name="limit">Maximum number of results to be returned. limit max value is 100</param>
        /// <returns></returns>
        public WGResponse<List<Player>> SearchPlayers(string searchTerm, int limit)
        {
            return SearchPlayers(searchTerm, WGPlayerListField.All, WGLanguageField.EN, limit);
        }

        /// <summary>
        /// Method returns partial list of players. The list is filtered by initial characters of user name and sorted alphabetically.
        /// </summary>
        /// <param name="searchTerm">search string</param>
        /// <param name="responseFields">fields to be returned.</param>
        /// <param name="language">language</param>
        /// <param name="limit">Maximum number of results to be returned. limit max value is 100</param>
        /// <returns></returns>
        public WGResponse<List<Player>> SearchPlayers(string searchTerm, WGPlayerListField responseFields, WGLanguageField language, int limit)
        {
            var fields = new StringBuilder();

            if (responseFields == WGPlayerListField.AccountId)
                fields.Append("account_id");
            else if (responseFields == WGPlayerListField.Nickname)
                fields.Append("nickname");
            else
                fields.Clear();

            var requestURI = CreatePlayerSearchRequestURI(searchTerm, language, fields.ToString(), limit);

            var output = GetRequestResponse(requestURI);

            var obj = JsonConvert.DeserializeObject<WGResponse<List<Player>>>(output);

            return obj;
        }

        private string CreatePlayerSearchRequestURI(string searchTerm, WGLanguageField language, string responseFields, int limit)
        {
            var target = "account/list";

            var generalUri = GetGeneralUri(target, language);

            var sb = new StringBuilder(generalUri);

            if (!string.IsNullOrWhiteSpace(responseFields))
                sb.AppendFormat("&fields={0}", responseFields);

            sb.AppendFormat("&search={0}&limit={1}", searchTerm, limit);

            var requestURI = sb.ToString();

            return requestURI;
        }

        #endregion Search Players

        #region Player Info

        /// <summary>
        /// Method returns player details.
        /// </summary>
        /// <param name="accountId">player account id</param>
        /// <returns></returns>
        public WGResponse<List<Player>> GetPlayerInfo(long accountId)
        {
            return GetPlayerInfo(new[] { accountId }, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Method returns player details.
        /// </summary>
        /// <param name="accountId">list of player account ids</param>
        /// <returns></returns>
        public WGResponse<List<Player>> GetPlayerInfo(long[] accountIds)
        {
            return GetPlayerInfo(accountIds, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Method returns player details.
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <param name="language">language</param>
        /// <param name="accessToken">access token</param>
        /// <param name="responseFields">fields to be returned. Null or string.Empty for all</param>
        /// <returns></returns>
        public WGResponse<List<Player>> GetPlayerInfo(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var requestURI = CreatePlayerDataRequestURI(accountIds, language, accessToken, responseFields);

            var output = GetRequestResponse(requestURI);

            var obj = JsonConvert.DeserializeObject<WGRawResponse>(output);

            var players = new List<Player>();

            foreach (var playerWrapper in ((JObject)obj.Data).Children())
            {
                players.Add(playerWrapper.First.ToObject(typeof(Player)) as Player);
            }

            var response = new WGResponse<List<Player>>()
            {
                Count = obj.Count,
                Status = obj.Status,
                Data = players
            };

            return response;
        }

        private string CreatePlayerDataRequestURI(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var target = "account/info";

            var generalUri = GetGeneralUri(target, language);

            var sb = new StringBuilder(generalUri);

            if (!string.IsNullOrWhiteSpace(responseFields))
                sb.AppendFormat("&fields={0}", responseFields);

            if (!string.IsNullOrWhiteSpace(accessToken))
                sb.AppendFormat("&access_token={0}", accessToken);

            sb.AppendFormat("&account_id={0}", string.Join(",", accountIds));

            var requestURI = sb.ToString();

            return requestURI;
        }

        #endregion Player Info

        #region Player Ratings

        /// <summary>
        /// Method returns details on player's ratings.
        /// </summary>
        /// <param name="accountId">player account id</param>
        /// <returns></returns>
        [Obsolete("Method is deprecated and will be removed soon.")]
        public WGResponse<object> GetPlayerRatings(long accountId)
        {
            return GetPlayerRatings(new[] { accountId });
        }

        /// <summary>
        /// Method returns details on player's ratings.
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <returns></returns>
        [Obsolete("Method is deprecated and will be removed soon.")]
        public WGResponse<object> GetPlayerRatings(long[] accountIds)
        {
            return GetPlayerRatings(accountIds, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Method returns details on player's ratings.
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <param name="language">language</param>
        /// <param name="accessToken">access token</param>
        /// <param name="responseFields">fields to be returned. Null or string.Empty for all</param>
        /// <returns></returns>
        [Obsolete("Method is deprecated and will be removed soon.")]
        public WGResponse<object> GetPlayerRatings(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            return null;
        }

        private string CreatePlayerRatingsRequestURI(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var target = "account/ratings";

            var generalUri = GetGeneralUri(target, language);

            var sb = new StringBuilder(generalUri);

            if (!string.IsNullOrWhiteSpace(responseFields))
                sb.AppendFormat("&fields={0}", responseFields);

            if (!string.IsNullOrWhiteSpace(accessToken))
                sb.AppendFormat("&access_token={0}", accessToken);

            sb.AppendFormat("&account_id={0}", string.Join(",", accountIds));

            var requestURI = sb.ToString();

            return requestURI;
        }

        #endregion Player Ratings

        #region Player Tanks

        /// <summary>
        /// Method returns details on player's vehicles.
        /// </summary>
        /// <param name="accountId">player account id</param>
        /// <returns></returns>
        public WGResponse<List<Tank>> GetPlayerVehicles(long accountId)
        {
            return GetPlayerVehicles(new[] { accountId }, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Method returns details on player's vehicles.
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <returns></returns>
        public WGResponse<List<Tank>> GetPlayerVehicles(long[] accountIds)
        {
            return GetPlayerVehicles(accountIds, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Method returns details on player's vehicles.
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <param name="language">language</param>
        /// <param name="accessToken">access token</param>
        /// <param name="responseFields">fields to be returned. Null or string.Empty for all</param>
        /// <returns></returns>
        public WGResponse<List<Tank>> GetPlayerVehicles(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var requestURI = CreatePlayerVehiclesRequestURI(accountIds, language, accessToken, responseFields);

            var output = GetRequestResponse(requestURI);

            var obj = JsonConvert.DeserializeObject<WGRawResponse>(output);

            var tanks = new List<Tank>();

            foreach (var accountId in accountIds)
            {
                var player = new Player { AccountId = accountId };

                var userTanks = ((JObject)obj.Data)[accountId.ToString()].ToObject(typeof(List<Tank>)) as List<Tank>;

                foreach (var tank in userTanks)
                {
                    tank.Player = player;
                    tanks.Add(tank);
                }
            }

            var response = new WGResponse<List<Tank>>()
            {
                Count = obj.Count,
                Status = obj.Status,
                Data = tanks
            };

            return response;
        }

        private string CreatePlayerVehiclesRequestURI(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var target = "account/tanks";

            var generalUri = GetGeneralUri(target, language);

            var sb = new StringBuilder(generalUri);

            if (!string.IsNullOrWhiteSpace(responseFields))
                sb.AppendFormat("&fields={0}", responseFields);

            if (!string.IsNullOrWhiteSpace(accessToken))
                sb.AppendFormat("&access_token={0}", accessToken);

            sb.AppendFormat("&account_id={0}", string.Join(",", accountIds));

            var requestURI = sb.ToString();

            return requestURI;
        }

        #endregion Player Tanks

        #region Player Achievements

        /// <summary>
        /// Warning. This method runs in test mode. Throws NotImplementedException
        /// </summary>
        /// <param name="accountId">player account id</param>
        /// <returns></returns>
        public WGResponse<object> GetPlayerAchievements(long accountId)
        {
            return GetPlayerAchievements(new[] { accountId });
        }

        /// <summary>
        /// Warning. This method runs in test mode. Throws NotImplementedException
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <returns></returns>
        public WGResponse<object> GetPlayerAchievements(long[] accountIds)
        {
            return GetPlayerAchievements(accountIds, WGLanguageField.EN, null, null);
        }

        /// <summary>
        /// Warning. This method runs in test mode. Throws NotImplementedException
        /// </summary>
        /// <param name="accountIds">list of player account ids</param>
        /// <param name="language">language</param>
        /// <param name="accessToken">access token</param>
        /// <param name="responseFields">fields to be returned. Null or string.Empty for all</param>
        /// <returns></returns>
        public WGResponse<object> GetPlayerAchievements(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var requestURI = CreatePlayerAchievementsRequestURI(accountIds, language, accessToken, responseFields);

            throw new NotImplementedException("Warning. This method runs in test mode.");
        }

        private string CreatePlayerAchievementsRequestURI(long[] accountIds, WGLanguageField language, string accessToken, string responseFields)
        {
            var target = "account/achievements";

            var generalUri = GetGeneralUri(target, language);

            var sb = new StringBuilder(generalUri);

            if (!string.IsNullOrWhiteSpace(responseFields))
                sb.AppendFormat("&fields={0}", responseFields);

            if (!string.IsNullOrWhiteSpace(accessToken))
                sb.AppendFormat("&access_token={0}", accessToken);

            sb.AppendFormat("&account_id={0}", string.Join(",", accountIds));

            var requestURI = sb.ToString();

            return requestURI;
        }

        #endregion Player Achievements

        #region General methods

        private string GetGeneralUri(string target, WGLanguageField language)
        {
            var languageField = Enum.GetName(typeof(WGLanguageField), language).ToLowerInvariant();

            var sb = new StringBuilder();

            if (_settings.RequestProtocol == WGRequestProtocol.HTTP)
                sb.Append("http://");
            else
                sb.Append("https://");

            sb.Append(_defaultApiURI);
            if (!_defaultApiURI.EndsWith("/"))
                sb.Append('/');

            sb.AppendFormat("{0}/?application_id={1}&language={2}", target, _applicationId, languageField);

            var result = sb.ToString();

            return result;
        }

        private string GetRequestResponse(string requestURI)
        {
            var webRequest = HttpWebRequest.Create(requestURI);
            var response = webRequest.GetResponse();

            var responseStream = response.GetResponseStream();
            string output = string.Empty;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                output = reader.ReadToEnd();
            }
            response.Close();

            return output;
        }

        #endregion
    }
}