// DTOs/Course/CourseContentCreateRequest.cs
using System.ComponentModel.DataAnnotations;

public class CourseContentCreateRequest
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string ContentType { get; set; }
    public string ContentDescription { get; set; }
    public int ContentOrder { get; set; }
    public IFormFile File { get; set; }
}
