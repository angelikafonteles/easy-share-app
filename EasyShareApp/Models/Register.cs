using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EasyShareApp.Models
{
    public class Register
    {
        public int Id { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Chave Identificadora")]
        public string Key { get; set; }

        [StringLength(int.MaxValue)]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Escolha uma senha")]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();

    }
}
