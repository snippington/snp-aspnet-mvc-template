using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SampleWebApp.Models;

public class Sample
{
    [Key]
    public int SampleId {get;set;}

    [Required]
    [Display(Name = "Program")]
    [ForeignKey("SampleProgramId")]
    public int SampleProgramId { get;set; }
    [Display(Name = "Program")]
    public virtual SampleProgram? SampleProgram {get;set;}
    
    [Required]
    [Display(Name = "Title")]
    [StringLength(60, MinimumLength = 2)]
    public string Title { get;set; } = "Name";

    [Required]
    [Display(Name = "Content")]
    [StringLength(512, MinimumLength = 2)]
    public string Content { get;set; } = "NA";

    
    public int CreatedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? CreatedOn {get;set;} = DateTime.Now;
    public int ModifiedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? LastModifiedOn { get;set; } = DateTime.Now;
    [Display(Name="Is Active")]
    public Boolean IsActive {get; set;} = true;
}