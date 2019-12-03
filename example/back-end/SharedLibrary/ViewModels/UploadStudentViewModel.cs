using System;
using System.Collections.Generic;
using SharedLibrary.Enumerations;

namespace SharedLibrary.ViewModels
{
    public class UploadStudentViewModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public StudentTypes? Type { get; set; }

        public List<StudentViewModel> Students { get; set; }
    }
}