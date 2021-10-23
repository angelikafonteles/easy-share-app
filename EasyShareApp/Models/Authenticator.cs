using System;
using System.ComponentModel.DataAnnotations;

namespace EasyShareApp.Models
{
    public class Authenticator
    {
        [Required(ErrorMessage = "Informe a chave")]
        [Display(Name = "Chave Identificadora")]
        public string Key { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Informe a senha")]
        [Display(Name = "Senha")]
        public string Password { get; set; }
    }
}
