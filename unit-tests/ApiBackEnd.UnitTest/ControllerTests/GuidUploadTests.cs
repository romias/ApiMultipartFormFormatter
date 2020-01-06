﻿using ApiBackEnd.Controllers;
using ApiBackEnd.ViewModels.Requests;
using ApiBackEnd.ViewModels.Responses;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Autofac;

namespace ApiBackEnd.UnitTest.ControllerTests
{
    [TestFixture]
    public class GuidUploadTests
    {
        #region Installations

        [SetUp]
        public void Setup()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<ApiUploadController>()
                .OnActivating(instance =>
                {
                    var apiUploadController = new ApiUploadController();
                    apiUploadController.Request = new HttpRequestMessage
                    {
                        RequestUri = new Uri("http://localhost/api/basic-upload")
                    };

                    apiUploadController.Configuration = new HttpConfiguration();
                    apiUploadController.Configuration.Routes.MapHttpRoute(
                        "DefaultApi",
                        "api/{controller}/{id}",
                        new {id = RouteParameter.Optional});

                    apiUploadController.RequestContext.RouteData = new HttpRouteData(
                        new HttpRoute(),
                        new HttpRouteValueDictionary {{"controller", "products"}});

                    instance.ReplaceInstance(apiUploadController);
                })
                .InstancePerLifetimeScope();

            _container = containerBuilder.Build();
        }

        [TearDown]
        public void TearDown()
        {
            _container.Dispose();
        }

        #endregion

        #region Methods

        [Test]
        public async Task SubmitValidGuidToNonNullableId_Returns_ResultWithValidId()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.Id = Guid.NewGuid();

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.AreEqual(uploadModel.Id, uploadResponseModel.Id);
        }

        [Test]
        public async Task SubmitEmptyGuidToNonNullableId_Returns_ResultWithEmptyId()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.Id = Guid.Empty;

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.AreEqual(uploadModel.Id, uploadResponseModel.Id);
        }

        [Test]
        public async Task SubmitEmptyGuidToNullableId_Returns_ResultWithEmptyAttachmentId()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.AttachmentId = null;

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.IsNull(uploadResponseModel.AttachmentId);
        }

        [Test]
        public async Task SubmitValidGuidToNullableId_Returns_ResultWithValidAttachmentId()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.AttachmentId = Guid.NewGuid();

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.AreEqual(uploadModel.AttachmentId, uploadResponseModel.AttachmentId);
        }

        [Test]
        public async Task SubmitInvalidGuidsList_Returns_ResultWithNullGuidsList()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.Ids = null;

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.IsNull(uploadResponseModel.Ids);
        }

        [Test]
        public async Task SubmitValidGuidsList_Returns_ResultWithValidGuidsList()
        {
            var controller = _container.Resolve<ApiUploadController>();

            var uploadModel = new UploadRequestViewModel();
            uploadModel.Ids = new List<Guid>(){Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};

            var actionResult = controller.BasicUpload(uploadModel);
            var httpResponseMessage = await actionResult.ExecuteAsync(default);
            var uploadResponseModel = await httpResponseMessage.Content.ReadAsAsync<UploadResponseViewModel>();

            Assert.AreSame(uploadModel.Ids, uploadResponseModel.Ids);
        }

        #endregion

        #region Properties

        private IContainer _container;

        #endregion
    }
}