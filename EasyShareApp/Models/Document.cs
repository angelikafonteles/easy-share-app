using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using EasyShareApp.Models.Enums;

namespace EasyShareApp.Models
{
    public partial class Document
    {
        public int Id { get; set; }

        [Display(Name = "Nome do Arquivo")]
        public string Name { get; set; }

        [Display(Name = "Data/Hora de Criação")]
        [DataType(DataType.DateTime)]
        public DateTime InstantCreation { get; set; }

        [Required(ErrorMessage = "Informe a data da expiração")]
        [Display(Name = "Data/Hora de Expiração")]
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime InstantExpiration { get; set; }

        [Display(Name = "Anexo")]
        public byte[] Attachment { get; set; }

        public Extension Extension { get; set; }

        public Register Register { get; set; }
        public int RegisterId { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Adicione um arquivo")]
        public IFormFile File { get; set; }
    }
}
