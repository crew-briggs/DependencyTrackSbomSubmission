using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DependencyTrack;
using System.Net.Http;
using System.Runtime;

namespace DependencyTrack
{
    public partial class AsyncProcessingToken
    {
        [JsonProperty("token", Required = Required.Always)]
        public string? Token { get; set; }
    }

    public partial class ProcessingStatus
    {
        [JsonProperty("processing", Required = Required.Always)]
        public bool? Processing { get; set; }
    }

    public partial class DependencyTrackClient
    {
        //protected HttpClient _httpClient;

        //public DependencyTrackClientExtended(HttpClient client) : base(client)
        //{
        //    this._httpClient = client ?? throw new ArgumentNullException("client");
        //}

        /// <summary>Upload a supported bill of material format document</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public Task<AsyncProcessingToken> UploadBomReturnAsyncProcessingToken(BomSubmitRequest body)
        {
            return UploadBomReturnAsyncProcessingToken(body, CancellationToken.None);
        }

        partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
        {
            settings.Error = (sender, error) => error.ErrorContext.Handled = true;
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Upload a supported bill of material format document</summary>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<AsyncProcessingToken> UploadBomReturnAsyncProcessingToken(BomSubmitRequest body, CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(this.BaseUrl != null ? this.BaseUrl.TrimEnd('/') : "").Append("/v1/bom");

            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body, this._settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;

                        }

                        ProcessResponse(client_, response_);

                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200" || status_ == "204")
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<AsyncProcessingToken>(response_, headers_).ConfigureAwait(false);
                            return objectResponse_.Object;
                        }

                        if (status_ == "401")
                        {
                            string responseText_ = (response_.Content == null) ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unauthorized", (int)response_.StatusCode, responseText_, headers_, null);
                        }
                        else if (status_ == "403")
                        {
                            string responseText_ = (response_.Content == null) ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Access to the specified project is forbidden", (int)response_.StatusCode, responseText_, headers_, null);
                        }
                        else if (status_ == "404")
                        {
                            string responseText_ = (response_.Content == null) ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The project could not be found", (int)response_.StatusCode, responseText_, headers_, null);
                        }
                        else if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }

                        return default(AsyncProcessingToken);
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }


        /// <summary>Determines if there are any tasks associated with the token that are being processed, or in the queue to be processed.</summary>
        /// <param name="uuid">The UUID of the token to query</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public Task<bool?> IsTokenBeingProcessed2Async(string uuid)
        {
            return IsTokenBeingProcessed2Async(uuid, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Determines if there are any tasks associated with the token that are being processed, or in the queue to be processed.</summary>
        /// <param name="uuid">The UUID of the token to query</param>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async Task<bool?> IsTokenBeingProcessed2Async(string uuid, CancellationToken cancellationToken)
        {
            if (uuid == null)
                throw new ArgumentNullException("uuid");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/v1/bom/token/{uuid}");
            urlBuilder_.Replace("{uuid}", System.Uri.EscapeDataString(ConvertToString(uuid, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = ((int)response_.StatusCode).ToString();

                        if (status_ == "200" || status_ == "204")
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ProcessingStatus>(response_, headers_).ConfigureAwait(false);
                            return objectResponse_.Object?.Processing;
                        }

                        if (status_ == "401")
                        {
                            string responseText_ = (response_.Content == null) ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unauthorized", (int)response_.StatusCode, responseText_, headers_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }

                        
                        return null;
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    }
}
