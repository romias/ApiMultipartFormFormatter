module.exports = (ngModule) => {
    ngModule.service('$upload', ($http, apiUrlConstant) => {

        return {
            //#region Methods

            basicUpload: (fullName, attachment) => {
                const fullUrl = `${apiUrlConstant.apiBaseUrl}/${apiUrlConstant.upload.basicUpload}`;

                let f = new FormData();
                f.append('author[fullName]', fullName);
                f.append('attachment', attachment);

                let options = {
                    headers: {
                        'Content-Type': undefined
                    }
                };

                return $http.post(fullUrl, f, options)
                    .then((uploadResponse) => {
                        return uploadResponse.data;
                    });
            },

            listUpload: (fullName, attachments) => {
                const fullUrl = `${apiUrlConstant.apiBaseUrl}/${apiUrlConstant.upload.listUpload}`;

                let f = new FormData();
                f.append('author[fullName]', fullName);

                angular.forEach(attachments, (attachment, iterator) => {
                    let p = `attachments[${iterator}]`;
                    f.append(p, attachment);
                });

                let options = {
                    headers: {
                        'Content-Type': undefined
                    }
                };

                return $http.post(fullUrl, f, options)
                    .then((basicUploadResponse) => {
                        return basicUploadResponse.data;
                    });
            },


            nestedInfoUpload: (attachment, profileName, profileAttachment) => {
                const fullUrl = `${apiUrlConstant.apiBaseUrl}/${apiUrlConstant.upload.nestedInfoUpload}`;

                let f = new FormData();
                f.append('attachment', attachment);
                f.append('profile[name]', profileName);
                f.append('profile[attachment]', profileAttachment);

                let options = {
                    headers: {
                        'Content-Type': undefined
                    }
                };

                return $http.post(fullUrl, f, options)
                    .then((uploadResponse) => {
                        return uploadResponse.data;
                    });
            }


            //#endregion
        }
    });
};