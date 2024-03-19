using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SampleWebApp.Models;

public class SampleProgram
{
    [Key]
    public int SampleProgramId {get;set;}
    
    [Required]
    [Display(Name = "Name")]
    [StringLength(60, MinimumLength = 2)]
    
    public string Name { get;set; } = "Name";

    
    public int CreatedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? CreatedOn {get;set;} = DateTime.Now;
    public int ModifiedBy {get;set;}
    [DataType(DataType.Date)]
    public DateTime? LastModifiedOn { get;set; } = DateTime.Now;
    [Display(Name="Is Active")]
    public Boolean IsActive {get; set;} = true;
}