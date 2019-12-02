﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;

using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Microsoft.AspNetCore.Mvc.Formatters;

using MultipartFormDataFormatter.Extensions;
using MultipartFormDataFormatter.Models;
using MultipartFormDataFormatter.Services.Implementations;
using MultipartFormDataFormatter.Services.Interfaces;

#if NETFRAMEWORK
using System.Net.Http.Headers;
#elif NETSTANDARD
using Microsoft.Net.Http.Headers;
#endif

namespace MultipartFormDataFormatter
{
    /// <summary>
    ///     Handler for content disposition name analyzer.
    /// </summary>
    /// <param name="contentDispositionName"></param>
    /// <returns></returns>
    public delegate List<string> FindContentDispositionParametersHandler(string contentDispositionName);

#if NETFRAMEWORK
    public class MultipartFormDataFormatter : MediaTypeFormatter
#elif NETSTANDARD
    public class MultipartFormDataFormatter : InputFormatter
#endif
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipartFormDataFormatter" /> class.
        /// </summary>
        public MultipartFormDataFormatter()
        {
            // Register multipart/form-data as the supported media type.
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(SupportedMediaType));
        }

        #endregion

        #region Properties

        private const string SupportedMediaType = "multipart/form-data";

        /// <summary>
        ///     Interceptor for handling content disposition content name.
        /// </summary>
        public FindContentDispositionParametersHandler FindContentDispositionParametersInterceptor { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Whether the instance can be read or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
#if NETFRAMEWORK
        public override bool CanReadType(Type type)
#elif NETSTANDARD
        protected override bool CanReadType(Type type)
#endif
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return true;
        }

#if NETFRAMEWORK
        /// <summary>
        ///     Whether the instance can be written or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override bool CanWriteType(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return false;
        }
#endif

#if NETFRAMEWORK
        /// <summary>
        ///     Read data from incoming stream.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stream"></param>
        /// <param name="content"></param>
        /// <param name="formatterLogger"></param>
        /// <returns></returns>
        public override async Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            // Type is invalid.
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Stream is invalid.
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // Find dependency resolver.
            var dependencyResolver = GlobalConfiguration.Configuration.DependencyResolver;
            if (dependencyResolver == null)
                throw new ArgumentException("Dependency resolver is required.");

            using (var dependencyScope = dependencyResolver.BeginScope())
            {
                try
                {
                    // load multipart data into memory 
                    var multipartProvider = await content.ReadAsMultipartAsync();
                    var httpContents = multipartProvider.Contents;

                    // Create an instance from specific type.
                    var instance = Activator.CreateInstance(type);

                    foreach (var httpContent in httpContents)
                    {
                        // Find parameter from content deposition.
                        var contentParameter = httpContent.Headers.ContentDisposition.Name.Trim('"');
                        var parameterParts = FindContentDispositionParameters(contentParameter);

                        // Content is a parameter, not a file.
                        if (string.IsNullOrEmpty(httpContent.Headers.ContentDisposition.FileName))
                        {
                            var value = await httpContent.ReadAsStringAsync();
                            await BuildRequestModelAsync(instance, parameterParts, value, dependencyScope);
                            continue;
                        }

                        // Content is a file.
                        // File retrieved from client-side.

                        HttpFileBase file = null;

                        // set null if no content was submitted to have support for [Required]
                        if (httpContent.Headers.ContentLength.GetValueOrDefault() > 0)
                            file = new HttpFile(
                                httpContent.Headers.ContentDisposition.FileName.Trim('"'),
                                httpContent.Headers.ContentType.MediaType,
                                await httpContent.ReadAsByteArrayAsync()
                            );
                        else
                            file = null;

                        await BuildRequestModelAsync(instance, parameterParts, file, dependencyScope);
                    }

                    return instance;
                }
                catch (Exception e)
                {
                    if (formatterLogger == null)
                        throw;
                    formatterLogger.LogError(string.Empty, e);
                    return GetDefaultValueForType(type);
                }
            }
        }

#endif

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            if (context.ModelType == null)
                throw new ArgumentNullException(nameof(context.ModelType));

            try
            {
                // load multipart data into memory 
                var httpContents = await context.HttpContext.Request.ReadFormAsync();

                // Create an instance from specific type.
                var instance = Activator.CreateInstance(context.ModelType);

                foreach (var httpContent in httpContents)
                {
                    // Find parameter from content deposition.
                    string contentParameter = null;
                    contentParameter = httpContent.Key.Trim();

                    var parameterParts = contentParameter.ToContentDispositionParameters(FindContentDispositionParametersInterceptor);
                    await BuildRequestModelAsync(instance, parameterParts, value, dependencyScope);
                    continue;

                    //// Content is a file.
                    //// File retrieved from client-side.

                    //HttpFileBase file = null;

                    //// set null if no content was submitted to have support for [Required]
                    //if (httpContent.Headers.ContentLength.GetValueOrDefault() > 0)
                    //    file = new HttpFile(
                    //        httpContent.Headers.ContentDisposition.FileName.Trim('"'),
                    //        httpContent.Headers.ContentType.MediaType,
                    //        await httpContent.ReadAsByteArrayAsync()
                    //    );
                    //else
                    //    file = null;

                    //await BuildRequestModelAsync(instance, parameterParts, file, dependencyScope);
                }

                return instance;
            }
            catch (Exception e)
            {
                return GetDefaultValueForType(context.ModelType);
            }
        }

        /// <summary>
        ///     Read parameters list and bind information to model.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="parameters"></param>
        /// <param name="value"></param>
        /// <param name="multipartFormDataModelBinderServices"></param>
        protected async Task BuildRequestModelAsync(object model, IList<string> parameters, object value, List<IMultiPartFormDataModelBinderService> multipartFormDataModelBinderServices)
        {
            // Initiate model pointer.
            var pointer = model;

            // Find the last key.
            //var lastKey = parameters[parameters.Count - 1];

            // Initiate property information.
            PropertyInfo propertyInfo = null;

            if (parameters == null || parameters.Count < 1)
                return;

            // Go through every part of parameters.
            // If the parameter name is : Items[0][list]. Parsed params will be : Items, 0, list.
            for (var index = 0; index < parameters.Count; index++)
            {
                // Find the next parameter index.
                // If the current parameter is : Item, the next param will be : 0
                var iNextIndex = index + 1;

                // Find parameter key.
                var key = parameters[index];

                // Numeric key is always about array.
                if (key.IsNumeric())
                {
                    // Invalid property info.
                    if (propertyInfo == null)
                        return;

                    // Current property information is not a list.
                    if (!IsList(propertyInfo.PropertyType))
                        return;

                    // Find the index of parameter.
                    if (!int.TryParse(key, out var iCollectionIndex))
                        iCollectionIndex = -1;

                    // Add new property into list.
                    object val = null;

                    // This is the last key.
                    if (iNextIndex >= parameters.Count)
                    {
                        AddArrayMember(pointer, iCollectionIndex, propertyInfo, value);
                        return;
                    }

                    val = AddArrayMember(pointer, iCollectionIndex, propertyInfo);
                    pointer = val;

                    // Find the property information of the next key.
                    var nextKey = parameters[iNextIndex];
                    propertyInfo = FindPropertyInfoFromPointer(pointer, nextKey);
                    continue;
                }

                // Find property of the current key.
                propertyInfo = FindPropertyInfoFromPointer(pointer, key);

                // Property doesn't exist.
                if (propertyInfo == null)
                    return;

                // This is the last parameter.
                if (iNextIndex >= parameters.Count)
                {
                    var modelValue =
                        await BuildRequestModelValueAsync(propertyInfo, value, multipartFormDataModelBinderServices);
                    propertyInfo.SetValue(pointer, modelValue);
                    return;
                }

                // Find targeted value.
                var targetedValue = propertyInfo.GetValue(pointer);

                // Value doesn't exist.
                if (targetedValue == null)
                {
                    // Initiate property value.
                    targetedValue =
                        await BuildRequestModelValueAsync(propertyInfo,
                            Activator.CreateInstance(propertyInfo.PropertyType), multipartFormDataModelBinderServices);

                    propertyInfo.SetValue(pointer, targetedValue);
                    pointer = targetedValue;
                    continue;
                }

                // Value is list.
                if (IsList(propertyInfo.PropertyType))
                {
                    pointer = propertyInfo.GetValue(pointer);
                    if (iNextIndex >= parameters.Count)
                        AddArrayMember(pointer, -1, propertyInfo, value);
                    continue;
                }

                // Go to next key
                pointer = targetedValue;
            }
        }

        /// <summary>
        ///     Find property value from property type and raw value.
        /// </summary>
        /// <returns></returns>
        protected Task<object> BuildRequestModelValueAsync(PropertyInfo propertyInfo, object value,
            IList<IMultiPartFormDataModelBinderService> services)
        {
            // Output property value.
            var outputPropertyValue = value;
            var hasModelBinderCalled = false;

            foreach (var availableService in services)
            {
                if (availableService == null ||
                    !(availableService is IMultiPartFormDataModelBinderService multiPartFormDataModelBinderService))
                    continue;

                outputPropertyValue = multiPartFormDataModelBinderService.BuildModel(propertyInfo, outputPropertyValue);
                hasModelBinderCalled = true;
            }

            // Model binder hasn't been called.
            if (!hasModelBinderCalled)
            {
                outputPropertyValue = new BaseMultiPartFormDataModelBinderService()
                    .BuildModel(propertyInfo, outputPropertyValue);
            }

            return Task.FromResult(outputPropertyValue);
        }

        /// <summary>
        ///     Add or update member of array.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="iCollectionIndex"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object AddArrayMember(object pointer, int iCollectionIndex, PropertyInfo propertyInfo,
            object value = null)
        {
            // Current member is an array, normally, it will have count property.
            var itemCountProperty = propertyInfo.PropertyType.GetProperty(nameof(Enumerable.Count));
            if (itemCountProperty == null)
                return null;

            // Find items number in the list.
            var itemCount = (int)itemCountProperty.GetValue(pointer, null);

            // Get generic arguments from property.
            var genericArguments = propertyInfo.PropertyType.GetGenericArguments();

            // No generic argument has been found.
            if (genericArguments.Length < 1)
                return null;

            // Get the first argument.
            var genericArgument = genericArguments[0];

            // No generic argument has been found.
            if (genericArgument == null)
                return null;

            // Current index is invalid to the array, this means we will add a new item to the list.
            // For example, the current array has 1 element, and the iCollectionIndex is 1.
            // The item at the index is invalid, therefore, new item will be created.
            if (iCollectionIndex < 0 || iCollectionIndex > itemCount - 1)
            {
                object listItem;
                if (value != null)
                    listItem = value;
                else
                    listItem = Activator.CreateInstance(genericArguments[0]);

                // Find the add method.
                var addProperty = propertyInfo.PropertyType.GetMethod(nameof(IList.Add));
                if (addProperty != null)
                    addProperty.Invoke(pointer, new[] { listItem });
                return listItem;
            }

            // If the collection index is valid.
            // For example, list contains 2 element, and we are accessing the first element. This is ok, we can do it by searching for that element and set the property value.
            var elementAtMethod = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ElementAt));

            if (elementAtMethod != null)
            {
                var item = elementAtMethod.MakeGenericMethod(genericArguments[0]);
                return item.Invoke(pointer, new[] { pointer, iCollectionIndex });
            }

            return null;
        }

        /// <summary>
        ///     Find property information of an instance by using property name.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private PropertyInfo FindPropertyInfoFromPointer(object instance, string name)
        {
            return
                instance.GetType()
                    .GetProperties()
                    .FirstOrDefault(x => name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Whether instance is a collection or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsList(Type type)
        {
            if (!type.IsGenericType)
                return false;

            return type.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

#endregion
    }
}