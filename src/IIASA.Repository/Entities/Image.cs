using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IIASA.Repository.Entities
{
    [Table("Image")]
    public class Image
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(512)]
        public string FileName { get; set; }

        [MaxLength(512)]
        public string FileType { get; set; }

        public string MetaData { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
