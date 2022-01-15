using System.ComponentModel.DataAnnotations;

namespace MovieNight.Common.Authorization
{
    public enum Policies
    {
        [Display(Name = "IsEmployee")]
        IsEmployee = 0,

        [Display(Name = "IsAdmin")]
        IsAdmin = 1
    }
}
