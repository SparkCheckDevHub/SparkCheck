using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SparkCheck.Models
{
    [Table("TInterests")]
    public class TInterests
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intInterestID { get; set; }

        [Required]
        [MaxLength(250)]
        public string strInterest { get; set; } = "";

        public int intInterestCategoryID { get; set; } 

        [ForeignKey("intInterestCategoryID")]
        public TInterestCategory? InterestCategory { get; set; }

        public int intInterestSubCategoryID { get; set; }

        [ForeignKey("intInterestSubCategoryID")]
        public TInterestSubCategory? InterestSubCategory { get; set; }
    }




    [Table("TInterestCategories")]
    public class TInterestCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intInterestCategoryID { get; set; }

        [Required]
        public string strInterestCategory { get; set; } = "";
    }





    [Table("TInterestSubCategories")]
    public class TInterestSubCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int intInterestSubCategoryID { get; set; }

        [Required]
        public string strInterestSubCategory { get; set; } = "";
    }




    public class InterestCategoryGroup
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<TInterests> Interests { get; set; } = new();
    }
}