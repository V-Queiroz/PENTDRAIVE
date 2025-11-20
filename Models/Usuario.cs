using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PENTDRIVEApi.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        public int Id { get; set; }

        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Column("CNPJ")]
        public string CNPJ { get; set; } = string.Empty;

        [Column("SENHA_HASH", TypeName = "VARBINARY(64)")]
        public byte[] SenhaHash { get; set; } = Array.Empty<byte>();

        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Column("ROLE")]
        public string Role { get; set; } = "Padrao";


    }
}