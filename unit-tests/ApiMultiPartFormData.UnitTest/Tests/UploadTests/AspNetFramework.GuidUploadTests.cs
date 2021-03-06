﻿#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using ApiMultiPartFormData.UnitTest.Tests.Interfaces;
using ApiMultiPartFormData.UnitTest.ViewModels;
using Moq;
using NUnit.Framework;

namespace ApiMultiPartFormData.UnitTest.Tests.UploadTests
{
    [TestFixture]
    public class GuidUploadTests : IGuidUploadTests
    {
        #region Properties

        #endregion

        #region Installations



        #endregion


        [Test]
        public async Task UploadStudentWithId_Returns_StudentWithId()
        {
            var studentId = Guid.NewGuid().ToString("D");
            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            multipartFormContent.Add(new StringContent(studentId, Encoding.UTF8), nameof(StudentViewModel.Id));

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                    multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.Id.ToString("D"), studentId);
        }

        [Test]
        public async Task UploadStudentWithoutId_Returns_StudentWithGuidEmptyId()
        {
            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                    multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.Id, Guid.Empty);
        }

        [Test]
        public async Task UploadStudentWithoutParentId_Returns_StudentWithNullParentId()
        {
            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                    multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.ParentId, null);
        }

        [Test]
        public async Task UploadStudentWithParentId_Returns_StudentWithParentId()
        {
            var parentId = Guid.NewGuid().ToString("D");

            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");
            multipartFormContent.Add(new StringContent(parentId, Encoding.UTF8), nameof(StudentViewModel.ParentId));
            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                    multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.ParentId?.ToString("D"), parentId);
        }

        [Test]
        public async Task UploadStudentWithChildIds_Returns_StudentWithChildIds()
        {
            var childIds = new LinkedList<Guid>();
            childIds.AddLast(Guid.NewGuid());
            childIds.AddLast(Guid.NewGuid());

            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            var index = 0;
            foreach (var childId in childIds)
            {
                multipartFormContent.Add(new StringContent(childId.ToString("D"), Encoding.UTF8), $"{nameof(StudentViewModel.ChildIds)}[{index}]");
                index++;
            }

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            for (var childId = 0; childId < childIds.Count; childId++)
                Assert.AreEqual(childIds.ElementAt(childId), student.ChildIds[childId]);
        }

        [Test]
        public async Task UploadIdIntoProfile_Returns_StudentProfileWithId()
        {
            var profileId = Guid.NewGuid().ToString("D");

            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");
            multipartFormContent.Add(new StringContent(profileId, Encoding.UTF8), $"{nameof(StudentViewModel.Profile)}[{nameof(ProfileViewModel.Id)}]");

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.Profile.Id.ToString("D"), profileId);
        }

        [Test]
        public async Task UploadBlankIdIntoProfile_Returns_StudentProfileWithId()
        {
            var profileId = Guid.Empty.ToString("D");

            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");
            multipartFormContent.Add(new StringContent(profileId, Encoding.UTF8), $"{nameof(StudentViewModel.Profile)}[{nameof(ProfileViewModel.Id)}]");

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.AreEqual(student.Profile.Id.ToString("D"), profileId);
        }

        [Test]
        public async Task NoUploadIntoProfile_Returns_StudentNullProfile()
        {
            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.IsNull(student.Profile);
        }

        [Test]
        public async Task UploadWithNestedRelativeIds_Returns_StudentProfileWithRelativeIds()
        {
            var relativeIds = new Guid[]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
            };

            var logger = new Mock<IFormatterLogger>();
            logger.Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));

            var multipartFormDataFormatter = new MultipartFormDataFormatter();
            var multipartFormContent = new MultipartFormDataContent("---wwww-wwww-wwww-boundary-----");

            for (var i = 0; i < relativeIds.Length; i++)
            {
                var content = new StringContent(relativeIds[i].ToString("D"), Encoding.UTF8);
                multipartFormContent.Add(content, $"{nameof(StudentViewModel.Profile)}[{nameof(ProfileViewModel.RelativeIds)}][{i}]");
            }

            var uploadedModel = await multipartFormDataFormatter
                .ReadFromStreamAsync(typeof(StudentViewModel), new MemoryStream(),
                multipartFormContent, logger.Object);

            if (!(uploadedModel is StudentViewModel student))
            {
                Assert.IsInstanceOf<StudentViewModel>(uploadedModel);
                return;
            }

            Assert.NotNull(student.Profile);
            Assert.NotNull(student.Profile.RelativeIds);
            Assert.AreEqual(student.Profile.RelativeIds.Count, relativeIds.Length);

            for (var i = 0; i < relativeIds.Length; i++)
                Assert.AreEqual(relativeIds[i], student.Profile.RelativeIds[i]);
        }
    }
}
#endif